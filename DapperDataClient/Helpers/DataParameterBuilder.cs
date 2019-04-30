using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DapperDataClient.Models;
using static Dapper.SqlMapper;

namespace DapperDataClient.Helpers
{
    /// <summary>
    /// The dynamic parameters passed to stored proecedures.
    /// </summary>
    public class DataParameterBuilder : IDynamicParameters
    {
        private readonly IEnumerable<DataParameterInfo> _inParams;
        private readonly IEnumerable<DataParameterInfo> _outParams;
        private IDbCommand _dataBaseCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataParameterBuilder"/> class.
        /// </summary>
        /// <param name="inParams">The input parameters object.</param>
        /// <param name="outParams">The output parameters object.</param>
        public DataParameterBuilder(object inParams, object outParams)
        {
            _inParams = GetParamsFromObject(inParams);
            _outParams = GetParamsFromObject(outParams);
        }

        /// <inheritdoc />
        public void AddParameters(IDbCommand command, Identity identity)
        {
            _dataBaseCommand = command;

            foreach (DataParameterInfo param in _inParams)
            {
                command.Parameters.Add(GetDbParam(param));
            }

            foreach (DataParameterInfo param in _outParams)
            {
                command.Parameters.Add(GetDbParam(param, true));
            }
        }

        /// <summary>
        /// Fills the output parameters.
        /// </summary>
        /// <param name="dataParams">The data parameters.</param>
        public void FillOutputParams(object dataParams)
        {
            if (dataParams == null)
            {
                return;
            }

            PropertyInfo[] properties = dataParams.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanRead || property.GetCustomAttribute<NotMappedAttribute>() != null)
                {
                    continue;
                }

                ColumnAttribute column = property.GetCustomAttribute<ColumnAttribute>();
                DataParameterInfo paramInfo = _outParams.Single(pi => pi.Name == (column == null ? property.Name : column.Name));

                object dataBaseValue = ((DbParameter)_dataBaseCommand.Parameters[paramInfo.Name]).Value;
                object propertyValue = paramInfo.Value is bool ? Convert.ChangeType(dataBaseValue, TypeCode.Boolean) : dataBaseValue;

                if (property.CanWrite)
                {
                    property.SetValue(dataParams, propertyValue);
                }
                else
                {
                    string fieldName = $"<{paramInfo.Name}>i__Field";
                    FieldInfo field = dataParams.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field != null)
                    {
                        field.SetValue(dataParams, propertyValue);
                    }
                }
            }
        }

        private static bool IsSimpleType(Type type)
            => type.IsPrimitive
            || type.IsEnum
            || type == typeof(decimal)
            || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            || type == typeof(DateTime)
            || type == typeof(Guid)
            || type == typeof(string);

        private static IList<DataParameterInfo> GetParamsFromObject(object dataParams)
        {
            if (dataParams == null)
            {
                return new List<DataParameterInfo>();
            }

            IList<DataParameterInfo> dataParameters = new List<DataParameterInfo>();

            PropertyInfo[] properties = dataParams.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanRead || property.GetCustomAttribute<NotMappedAttribute>() != null)
                {
                    continue;
                }

                DataParameterInfo dataParamInfo = new DataParameterInfo();
                ColumnAttribute column = property.GetCustomAttribute<ColumnAttribute>();

                dataParamInfo.Name = column != null ? column.Name : property.Name;

                object propertyValue = property.GetValue(dataParams);
                dataParamInfo.Value = IsEnum(property) ? GetEnumValue(propertyValue) : propertyValue;

                dataParamInfo.IsStructured = !IsSimpleType(property.PropertyType);
                dataParamInfo.DataType = string.Empty;
                dataParameters.Add(dataParamInfo);
            }

            return dataParameters;
        }

        private static bool IsEnum(PropertyInfo property) =>
            property.PropertyType.IsEnum ||
            (Nullable.GetUnderlyingType(property.PropertyType)?.IsEnum ?? false);

        private static int? GetEnumValue(object enumerator) =>
            enumerator != null ?
                (int?)Convert.ToInt32((Enum)enumerator, CultureInfo.InvariantCulture) :
                null;

        private IDbDataParameter GetDbParam(DataParameterInfo paramInfo, bool isOutput = false)
        {
            SqlParameter parameter = new SqlParameter { ParameterName = paramInfo.Name };

            if (paramInfo.Value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else if (paramInfo.Value is bool)
            {
                parameter.Value = (bool)paramInfo.Value ? 1 : 0;
            }
            else if (paramInfo.Value is Enum)
            {
                parameter.Value = paramInfo.Value.ToString();
            }
            else
            {
                parameter.Value = paramInfo.Value;
            }

            if (paramInfo.Value is string)
            {
                parameter.Size = int.MaxValue;
            }

            if (paramInfo.IsStructured)
            {
                parameter.SqlDbType = SqlDbType.Structured;
                parameter.TypeName = paramInfo.DataType;
                parameter.Value = GetDataTable(paramInfo.Value);
            }

            if (isOutput)
            {
                parameter.Direction = ParameterDirection.Output;
            }

            return parameter;
        }

        private DataTable GetDataTable(object data)
        {
            IEnumerable<object> list = data as IEnumerable<object>;
            object firstItem = list?.FirstOrDefault();

            if (firstItem == null)
            {
                return null;
            }

            Type dataType = firstItem.GetType();
            PropertyInfo[] properties = dataType.GetProperties();
            DataTable dataTable = new DataTable();

            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (object item in list)
            {
                object[] values = new object[properties.Length];

                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item) ?? DBNull.Value;
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}

using System;
using System.Reflection;
using static Dapper.SqlMapper;

namespace DapperDataClient.Helpers
{
    /// <summary>
    /// Class that provides custom member mapping.
    /// </summary>
    /// <seealso cref="IMemberMap" />
    internal class MemberMap : IMemberMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberMap"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="property">The property.</param>
        /// <exception cref="ArgumentNullException">
        /// columnName or property.
        /// </exception>
        public MemberMap(string columnName, PropertyInfo property)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberMap"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="field">The field.</param>
        /// <exception cref="ArgumentNullException">
        /// columnName or field.
        /// </exception>
        public MemberMap(string columnName, FieldInfo field)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberMap"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentNullException">
        /// columnName or parameter.
        /// </exception>
        public MemberMap(string columnName, ParameterInfo parameter)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        /// <inheritdoc />
        public string ColumnName { get; }

        /// <inheritdoc />
        public Type MemberType => Field?.FieldType ?? Property?.PropertyType ?? Parameter?.ParameterType;

        /// <inheritdoc />
        public PropertyInfo Property { get; }

        /// <inheritdoc />
        public FieldInfo Field { get; }

        /// <inheritdoc />
        public ParameterInfo Parameter { get; }
    }
}

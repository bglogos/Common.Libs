using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;
using static Dapper.SqlMapper;

namespace DapperDataClient.Helpers
{
    /// <summary>
    /// Type mapper for attribute mapping.
    /// </summary>
    /// <seealso cref="ITypeMap" />
    public class TypeMapper : ITypeMap
    {
        private const string LinqBinary = "System.Data.Linq.Binary";
        private readonly List<FieldInfo> _fields;
        private readonly Type _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMapper"/> class.
        /// </summary>
        /// <param name="type">Entity type.</param>
        public TypeMapper(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _fields = GetSettableFields(type);
            Properties = GetSettableProps(type);
            _type = type;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public List<PropertyInfo> Properties { get; }

        /// <summary>
        /// Finds best constructor.
        /// </summary>
        /// <param name="names">DataReader column names.</param>
        /// <param name="types">DataReader column types.</param>
        /// <returns>Matching constructor or default one.</returns>
        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            ConstructorInfo[] constructors = _type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (ConstructorInfo constructor in constructors.OrderBy(c => c.IsPublic ? 0 : (c.IsPrivate ? 2 : 1)).ThenByDescending(c => c.GetParameters().Length))
            {
                ParameterInfo[] constructorParameters = constructor.GetParameters();

                if (constructorParameters.Length == 0)
                {
                    return constructor;
                }

                if (constructorParameters.Length != types.Length)
                {
                    continue;
                }

                int idx = 0;
                for (; idx < constructorParameters.Length; idx++)
                {
                    if (!string.Equals(constructorParameters[idx].Name, names[idx], StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (types[idx] == typeof(byte[]) && constructorParameters[idx].ParameterType.FullName == LinqBinary)
                    {
                        continue;
                    }

                    Type unboxedType = Nullable.GetUnderlyingType(constructorParameters[idx].ParameterType) ?? constructorParameters[idx].ParameterType;

                    if (unboxedType != types[idx]
                        && !(unboxedType.IsEnum && Enum.GetUnderlyingType(unboxedType) == types[idx])
                        && !(unboxedType == typeof(char) && types[idx] == typeof(string))
                        && !(unboxedType.IsEnum && types[idx] == typeof(string)))
                    {
                        break;
                    }
                }

                if (idx == constructorParameters.Length)
                {
                    return constructor;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the constructor, if any, that has the ExplicitCtorAttribute on it.
        /// </summary>
        /// <returns>The constructor information.</returns>
        public ConstructorInfo FindExplicitConstructor()
        {
            ConstructorInfo[] constructors = _type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<ConstructorInfo> withAttr = constructors.Where(c => c.GetCustomAttributes(typeof(ExplicitConstructorAttribute), true).Length > 0).ToList();
            return withAttr.Count == 1 ? withAttr.First() : null;
        }

        /// <summary>
        /// Gets mapping for constructor parameter.
        /// </summary>
        /// <param name="constructor">Constructor to resolve.</param>
        /// <param name="columnName">DataReader column name.</param>
        /// <returns>Mapping implementation.</returns>
        public IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            return new MemberMap(columnName, parameters.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Gets member mapping for column.
        /// </summary>
        /// <param name="columnName">DataReader column name.</param>
        /// <returns>Mapping implementation.</returns>
        public IMemberMap GetMember(string columnName)
        {
            PropertyInfo property = Properties.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.Ordinal)) ??
                Properties.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                return new MemberMap(columnName, property);
            }

            // Roslyn automatically implemented properties, in particular for get-only properties: <{Name}>k__BackingField.
            string backingFieldName = $"<{columnName}>k__BackingField";

            FieldInfo field = _fields.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.Ordinal)) ??
                _fields.FirstOrDefault(p => string.Equals(p.Name, backingFieldName, StringComparison.Ordinal)) ??
                _fields.FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase)) ??
                _fields.FirstOrDefault(p => string.Equals(p.Name, backingFieldName, StringComparison.OrdinalIgnoreCase));

            return field == null ? null : new MemberMap(columnName, field);
        }

        /// <summary>
        /// Gets the property setter.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="type">The type.</param>
        /// <returns>The property setters.</returns>
        internal static MethodInfo GetPropertySetter(PropertyInfo propertyInfo, Type type)
        {
            if (propertyInfo.DeclaringType == type)
            {
                return propertyInfo.GetSetMethod(true);
            }

            return propertyInfo.DeclaringType
                .GetProperty(
                    propertyInfo.Name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    Type.DefaultBinder,
                    propertyInfo.PropertyType,
                    propertyInfo.GetIndexParameters().Select(p => p.ParameterType).ToArray(),
                    null)
                .GetSetMethod(true);
        }

        /// <summary>
        /// Gets the settable props.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Collection of settable properties.</returns>
        internal static List<PropertyInfo> GetSettableProps(Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => GetPropertySetter(p, type) != null)
                .ToList();

        /// <summary>
        /// Gets the settable fields.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Collection of settable fields.</returns>
        internal static List<FieldInfo> GetSettableFields(Type type) =>
            type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
    }
}

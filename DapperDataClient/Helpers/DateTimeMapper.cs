using System;
using System.Data;
using System.Data.SqlTypes;
using static Dapper.SqlMapper;

namespace DapperDataClient.Helpers
{
    /// <summary>
    /// <see cref="DateTime"/> type handler.
    /// </summary>
    /// <seealso cref="TypeHandler{DateTime}" />
    public class DateTimeMapper : TypeHandler<DateTime>
    {
        /// <inheritdoc />
        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        }

        /// <inheritdoc />
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = new SqlDateTime(value);
        }
    }
}

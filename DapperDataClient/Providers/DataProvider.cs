using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using DapperDataClient.Helpers;
using static Dapper.SqlMapper;

namespace DapperDataClient.Providers
{
    /// <summary>
    /// The data provider.
    /// </summary>
    public class DataProvider : IDataProvider
    {
        private readonly HashSet<string> _mappedTypes = new HashSet<string>();
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProvider" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DataProvider(string connectionString)
        {
            _connectionString = connectionString;
            AddTypeHandler(new DateTimeMapper());
        }

        /// <inheritdoc />
        public int Execute(string query, object inParams = null, object outParams = null, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                CommandDefinition commandDefinition = GetCommand(query, inParams, outParams, commandTimeout);
                int affectedRows = connection.Execute(commandDefinition);
                FillOutParams(commandDefinition, outParams);
                return affectedRows;
            }
        }

        /// <inheritdoc />
        public async Task<int> ExecuteAsync(string query, object inParams = null, object outParams = null, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                CommandDefinition commandDefinition = GetCommand(query, inParams, outParams, commandTimeout);
                int executeTask = await connection.ExecuteAsync(commandDefinition);
                int affectedRows = executeTask;
                FillOutParams(commandDefinition, outParams);
                return affectedRows;
            }
        }

        /// <inheritdoc />
        public T GetFirstOrDefault<T>(string query, object parameters = null, int? commandTimeout = null) =>
            GetCollection<T>(query, parameters, null, commandTimeout).FirstOrDefault();

        /// <inheritdoc />
        public Task<T> GetSingleOrDefaultAsync<T>(string query, object parameters = null, int? commandTimeout = null) =>
            GetCollectionAsync<T>(query, parameters, null, commandTimeout)
                .ContinueWith(task => task.Result.SingleOrDefault(), TaskScheduler.Default);

        /// <inheritdoc />
        public T ExecuteScalar<T>(string query, object inParams = null, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                CommandDefinition commandDefinition = GetCommand(query, inParams, null, commandTimeout);
                T result = connection.ExecuteScalar<T>(commandDefinition);
                return result;
            }
        }

        /// <inheritdoc />
        public async Task<T> ExecuteScalarAsync<T>(string query, object inParams = null, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                CommandDefinition commandDefinition = GetCommand(query, inParams, null, commandTimeout);
                T scalarTask = await connection.ExecuteScalarAsync<T>(commandDefinition);
                return scalarTask;
            }
        }

        /// <inheritdoc />
        public void UsingTransactionScope(
            Action<IDataProvider> dataAction,
            TransactionScopeOption transactionScopeOption,
            System.Transactions.IsolationLevel isolationLevel)
        {
            TransactionOptions transactionOptions = new TransactionOptions
            {
                IsolationLevel = isolationLevel
            };

            using (TransactionScope transactionScope = new TransactionScope(transactionScopeOption, transactionOptions))
            {
                dataAction(this);
                transactionScope.Complete();
            }
        }

        /// <inheritdoc />
        public IEnumerable<T> GetCollection<T>(string query, object inParams = null, object outParams = null, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                RegisterType<T>();
                CommandDefinition commandDefinition = GetCommand(query, inParams, outParams, commandTimeout);
                IEnumerable<T> list = connection.Query<T>(commandDefinition);
                FillOutParams(commandDefinition, outParams);
                return list;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetCollectionAsync<T>(string query, object inParams = null, object outParams = null, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                RegisterType<T>();
                CommandDefinition commandDefinition = GetCommand(query, inParams, outParams, commandTimeout);
                IEnumerable<T> list = await connection.QueryAsync<T>(commandDefinition);
                FillOutParams(commandDefinition, outParams);
                return list;
            }
        }

        /// <inheritdoc />
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> GetMultipleCollections<T1, T2>(string query, object parameters, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                RegisterType<T1>();
                RegisterType<T2>();
                CommandDefinition commandDefinition = GetCommand(query, parameters, null, commandTimeout);
                IEnumerable<T1> item1;
                IEnumerable<T2> item2;
                using (GridReader gridReader = connection.QueryMultiple(commandDefinition))
                {
                    item1 = gridReader.Read<T1>();
                    item2 = gridReader.Read<T2>();
                }

                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }
        }

        /// <inheritdoc />
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> GetMultipleCollectionsAsync<T1, T2>(string query, object parameters, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                RegisterType<T1>();
                RegisterType<T2>();
                CommandDefinition commandDefinition = GetCommand(query, parameters, null, commandTimeout);
                IEnumerable<T1> item1;
                IEnumerable<T2> item2;
                using (GridReader gridReader = await connection.QueryMultipleAsync(commandDefinition))
                {
                    item1 = gridReader.Read<T1>();
                    item2 = gridReader.Read<T2>();
                }

                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }
        }

        /// <inheritdoc />
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> GetMultipleCollections<T1, T2, T3>(string query, object parameters, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                RegisterType<T1>();
                RegisterType<T2>();
                RegisterType<T3>();
                CommandDefinition commandDefinition = GetCommand(query, parameters, null, commandTimeout);
                IEnumerable<T1> item1;
                IEnumerable<T2> item2;
                IEnumerable<T3> item3;
                using (GridReader gridReader = connection.QueryMultiple(commandDefinition))
                {
                    item1 = gridReader.Read<T1>();
                    item2 = gridReader.Read<T2>();
                    item3 = gridReader.Read<T3>();
                }

                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(item1, item2, item3);
            }
        }

        /// <inheritdoc />
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> GetMultipleCollections<T1, T2, T3, T4>(string query, object parameters, int? commandTimeout = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                RegisterType<T1>();
                RegisterType<T2>();
                RegisterType<T3>();
                RegisterType<T4>();

                CommandDefinition commandDefinition = GetCommand(query, parameters, null, commandTimeout);
                IEnumerable<T1> item1;
                IEnumerable<T2> item2;
                IEnumerable<T3> item3;
                IEnumerable<T4> item4;
                using (GridReader gridReader = connection.QueryMultiple(commandDefinition))
                {
                    item1 = gridReader.Read<T1>();
                    item2 = gridReader.Read<T2>();
                    item3 = gridReader.Read<T3>();
                    item4 = gridReader.Read<T4>();
                }

                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(item1, item2, item3, item4);
            }
        }

        private static CommandDefinition GetCommand(string query, object inParams = null, object outParams = null, int? commandTimeout = null) =>
            new CommandDefinition(
                query,
                new DataParameterBuilder(inParams, outParams),
                null,
                commandTimeout,
                CommandType.StoredProcedure);

        private static void FillOutParams(CommandDefinition commandDefinition, object outParams = null)
        {
            if (outParams == null)
            {
                return;
            }

            DataParameterBuilder parameters = commandDefinition.Parameters as DataParameterBuilder;
            parameters?.FillOutputParams(outParams);
        }

        private void RegisterType<T>()
        {
            Type type = typeof(T);
            bool isAdded = _mappedTypes.Add(type.ToString());

            if (isAdded)
            {
                SetTypeMap(type, new TypeMapper(type));
            }
        }
    }
}

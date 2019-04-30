using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace DapperDataClient.Providers
{
    /// <summary>
    /// The data provider.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Performs a query using a transactions scope.
        /// </summary>
        /// <param name="dataAction">The data action.</param>
        /// <param name="transactionScopeOption">The transaction scope option.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        void UsingTransactionScope(Action<IDataProvider> dataAction, TransactionScopeOption transactionScopeOption, IsolationLevel isolationLevel);

        /// <summary>
        /// Executes a the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="outParams">The out parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>Number of rows affected.</returns>
        int Execute(string query, object inParams = null, object outParams = null, int? commandTimeout = null);

        /// <summary>
        /// Executes a the given query asynchronously.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="outParams">The out parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>Number of rows affected.</returns>
        Task<int> ExecuteAsync(string query, object inParams = null, object outParams = null, int? commandTimeout = null);

        /// <summary>
        /// Gets the first result from data server or returns its default value.
        /// </summary>
        /// <typeparam name="T">The type of the DTO coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// The first value of the required type.
        /// </returns>
        T GetFirstOrDefault<T>(string query, object parameters = null, int? commandTimeout = null);

        /// <summary>
        /// Gets single result from data server or returns its default value asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the DTO coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Signle value of the required type.
        /// </returns>
        Task<T> GetSingleOrDefaultAsync<T>(string query, object parameters = null, int? commandTimeout = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type of the scalar coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Signle value of the required type.
        /// </returns>
        T ExecuteScalar<T>(string query, object inParams = null, int? commandTimeout = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the scalar coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Signle value of the required type.
        /// </returns>
        Task<T> ExecuteScalarAsync<T>(string query, object inParams = null, int? commandTimeout = null);

        /// <summary>
        /// Gets collection of objects from data server asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the result coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="outParams">The out parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Collection of objects.
        /// </returns>
        IEnumerable<T> GetCollection<T>(string query, object inParams = null, object outParams = null, int? commandTimeout = null);

        /// <summary>
        /// Gets collection of objects from data server asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the DTO coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="inParams">The in parameters.</param>
        /// <param name="outParams">The out parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Collection of objects.
        /// </returns>
        Task<IEnumerable<T>> GetCollectionAsync<T>(string query, object inParams = null, object outParams = null, int? commandTimeout = null);

        /// <summary>
        /// Gets multiple collections of objects from data server.
        /// </summary>
        /// <typeparam name="T1">The type of the first object coming back from SQL Server.</typeparam>
        /// <typeparam name="T2">The type of the second object coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Multiple collections of objects.
        /// </returns>
        Tuple<IEnumerable<T1>, IEnumerable<T2>> GetMultipleCollections<T1, T2>(string query, object parameters, int? commandTimeout = null);

        /// <summary>
        /// Gets multiple collections of objects from data server asynchronously.
        /// </summary>
        /// <typeparam name="T1">The 1st type of the DTO coming back from SQL Server.</typeparam>
        /// <typeparam name="T2">The 2nd type of the DTO coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Multiple collections of objects.
        /// </returns>
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> GetMultipleCollectionsAsync<T1, T2>(string query, object parameters, int? commandTimeout = null);

        /// <summary>
        /// Gets multiple collections of objects from data server.
        /// </summary>
        /// <typeparam name="T1">The 1st type of the DTO coming back from SQL Server.</typeparam>
        /// <typeparam name="T2">The 2nd type of the DTO coming back from SQL Server.</typeparam>
        /// <typeparam name="T3">The 3rd type of the DTO coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Multiple collections of objects.
        /// </returns>
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> GetMultipleCollections<T1, T2, T3>(string query, object parameters, int? commandTimeout = null);

        /// <summary>
        /// Gets multiple collections of objects from data server asynchronously.
        /// </summary>
        /// <typeparam name="T1">The 1st type of the DTO coming back from SQL Server.</typeparam>
        /// <typeparam name="T2">The 2nd type of the DTO coming back from SQL Server.</typeparam>
        /// <typeparam name="T3">The 3rd type of the DTO coming back from SQL Server.</typeparam>
        /// <typeparam name="T4">The 4th type of the DTO coming back from SQL Server.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns>
        /// Multiple collections of objects.
        /// </returns>
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> GetMultipleCollections<T1, T2, T3, T4>(string query, object parameters, int? commandTimeout = null);
    }
}

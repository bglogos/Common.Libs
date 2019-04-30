using System.Threading.Tasks;
using Google.Apis.Http;
using Microsoft.Extensions.Logging;

namespace GoogleApis.Client.Handlers
{
    /// <summary>
    /// Handles HTTP exceptions from Google APIs.
    /// </summary>
    /// <seealso cref="IHttpExceptionHandler" />
    internal class HttpExceptionHandler : IHttpExceptionHandler
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpExceptionHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public HttpExceptionHandler(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public Task<bool> HandleExceptionAsync(HandleExceptionArgs args)
        {
            logger.LogError(args.Exception, args.Exception.Message, args);
            return Task.FromResult(false);
        }
    }
}

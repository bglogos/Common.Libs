using System.Threading.Tasks;
using Google.Apis.Http;
using Microsoft.Extensions.Logging;

namespace GoogleApis.Client.Handlers
{
    /// <summary>
    /// The Google APIs HTTP unsuccessful reposne handler.
    /// </summary>
    /// <seealso cref="IHttpUnsuccessfulResponseHandler" />
    public class HttpUnsuccessfulResponseHandler : IHttpUnsuccessfulResponseHandler
    {
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpUnsuccessfulResponseHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public HttpUnsuccessfulResponseHandler(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public Task<bool> HandleResponseAsync(HandleUnsuccessfulResponseArgs args)
        {
            logger.LogWarning($"{args.Request.RequestUri} returned status code {args.Response.StatusCode}", args);
            return Task.FromResult(false);
        }
    }
}

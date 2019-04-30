using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using GoogleApis.Client.Builders;
using GoogleApis.Client.Models;
using GoogleApis.Client.Providers.Interfaces;

namespace GoogleApis.Client.Providers
{
    /// <summary>
    /// The Google Analytics provider.
    /// </summary>
    /// <seealso cref="IAnalyticsProvider" />
    public class AnalyticsProvider : IAnalyticsProvider
    {
        private readonly IHttpExceptionHandler exceptionHandler;
        private readonly IHttpUnsuccessfulResponseHandler unsuccessfulResponseHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsProvider" /> class.
        /// </summary>
        public AnalyticsProvider()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticsProvider" /> class.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="unsuccessfulResponseHandler">The unsuccessful response handler.</param>
        public AnalyticsProvider(IHttpExceptionHandler exceptionHandler, IHttpUnsuccessfulResponseHandler unsuccessfulResponseHandler)
        {
            this.exceptionHandler = exceptionHandler;
            this.unsuccessfulResponseHandler = unsuccessfulResponseHandler;
        }

        /// <inheritdoc />
        public int GetTotalPageViews(PageViewsRequest request)
        {
            if (!File.Exists(request.ServiceAccountCredentialFile))
            {
                throw new ArgumentException($"Invalid credential file path {request.ServiceAccountCredentialFile}", nameof(request.ServiceAccountCredentialFile));
            }

            GoogleCredential credential = GoogleCredential
                .FromFile(request.ServiceAccountCredentialFile)
                .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly);

            BaseClientService.Initializer initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            };

            GetReportsResponse response = GetAnalyticsResponse(initializer, request);

            return response.Reports.FirstOrDefault()?.Data.Totals.Sum(t => int.Parse(t.Values.FirstOrDefault() ?? "0")) ?? 0;
        }

        private GetReportsResponse GetAnalyticsResponse(BaseClientService.Initializer initializer, PageViewsRequest request)
        {
            AnalyticsReportingService service = new AnalyticsReportingService(initializer);

            GetReportsRequest reportRequest = new GetReportsRequest
            {
                ReportRequests = new List<ReportRequest> { RequestBuilders.GetReportRequest(request) }
            };

            ReportsResource.BatchGetRequest batchGetRequest = service.Reports.BatchGet(reportRequest);

            if (exceptionHandler != null)
            {
                batchGetRequest.AddExceptionHandler(exceptionHandler);
            }

            if (unsuccessfulResponseHandler != null)
            {
                batchGetRequest.AddUnsuccessfulResponseHandler(unsuccessfulResponseHandler);
            }

            return batchGetRequest.Execute();
        }
    }
}

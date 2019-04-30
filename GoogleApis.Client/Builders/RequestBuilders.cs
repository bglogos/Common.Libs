using System;
using System.Collections.Generic;
using Google.Apis.AnalyticsReporting.v4.Data;
using GoogleApis.Client.Models;

namespace GoogleApis.Client.Builders
{
    /// <summary>
    /// Provides methods for building request objects.
    /// </summary>
    public static class RequestBuilders
    {
        /// <summary>
        /// Gets the report request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// New instance of <see cref="ReportRequest"/> object.
        /// </returns>
        public static ReportRequest GetReportRequest(PageViewsRequest request) =>
            new ReportRequest
            {
                DateRanges = new List<DateRange>
                {
                    new DateRange
                    {
                        StartDate = request.StartDateString,
                        EndDate = request.EndDateString
                    }
                },
                Metrics = new List<Metric>
                {
                    new Metric
                    {
                        Expression = Constants.PageVisitsExpression,
                        FormattingType = Constants.MetricsFormattingType
                    }
                },
                ViewId = request.ViewId
            };

        /// <summary>
        /// Gets the page views request.
        /// </summary>
        /// <param name="credentialFile">The credential file.</param>
        /// <param name="viewId">The view identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>
        /// New instance of  <see cref="PageViewsRequest" /> object.
        /// </returns>
        public static PageViewsRequest GetPageViewsRequest(string credentialFile, string viewId, DateTime startDate, DateTime endDate) =>
             new PageViewsRequest
             {
                 ViewId = viewId,
                 ServiceAccountCredentialFile = credentialFile,
                 StartDate = startDate,
                 EndDate = endDate
             };
    }
}

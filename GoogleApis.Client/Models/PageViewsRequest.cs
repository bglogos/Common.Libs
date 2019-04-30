using System;

namespace GoogleApis.Client.Models
{
    /// <summary>
    /// The data needed to get total page views from Google Analytics.
    /// </summary>
    public class PageViewsRequest
    {
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets the start date string.
        /// </summary>
        /// <value>
        /// The start date string.
        /// </value>
        public string StartDateString => StartDate.ToString(Constants.DateFormat);

        /// <summary>
        /// Gets the end date string.
        /// </summary>
        /// <value>
        /// The end date string.
        /// </value>
        public string EndDateString => EndDate.ToString(Constants.DateFormat);

        /// <summary>
        /// Gets or sets the view identifier.
        /// </summary>
        /// <value>
        /// The view identifier.
        /// </value>
        public string ViewId { get; set; }

        /// <summary>
        /// Gets or sets the service account credential file.
        /// </summary>
        /// <value>
        /// The service account credential file.
        /// </value>
        public string ServiceAccountCredentialFile { get; set; }
    }
}

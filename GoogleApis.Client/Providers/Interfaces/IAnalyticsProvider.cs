using GoogleApis.Client.Models;

namespace GoogleApis.Client.Providers.Interfaces
{
    /// <summary>
    /// The Google Analytics provider.
    /// </summary>
    public interface IAnalyticsProvider
    {
        /// <summary>
        /// Gets the total page views.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Total page view count.
        /// </returns>
        int GetTotalPageViews(PageViewsRequest request);
    }
}

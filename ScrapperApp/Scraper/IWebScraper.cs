using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public interface IWebScraper
{
    Task<IScrapResult> ScrapPath(RelativeUriPath path);
}
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public interface IWebScraper
{
    Task<IWebEntity> ScrapPath(RelativeUriPath path);
}
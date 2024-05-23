using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public interface IWebScraper
{
    Task<Maybe<IWebEntity>> ScrapPath(RelativeUriPath path);
}
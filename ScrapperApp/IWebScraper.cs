namespace ScrapperApp;

public interface IWebScraper
{
    Task<IScrapResult> ScrapPath(RelativeUriPath path);
}
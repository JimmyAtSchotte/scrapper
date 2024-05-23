using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public interface IScrapResult
{
    string GetFileName();
    IEnumerable<RelativeUriPath> GetLinkedFiles();
    byte[] GetContent();
}
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public interface IWebEntity
{
    string GetFileName();
    IEnumerable<RelativeUriPath> GetLinkedFiles();
    byte[] GetContent();
}
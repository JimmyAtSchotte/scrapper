namespace ScrapperApp;

public interface IScrapResult
{
    string GetFileName();
    IEnumerable<RelativeUriPath> GetLinkedFiles();
    byte[] GetContent();
}
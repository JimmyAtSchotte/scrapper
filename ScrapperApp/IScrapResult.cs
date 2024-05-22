namespace ScrapperApp;

public interface IScrapResult
{
    string GetFileName();
    IEnumerable<string> GetLinkedFiles();
    byte[] GetContent();
}
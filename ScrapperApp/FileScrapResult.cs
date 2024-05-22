namespace ScrapperApp;

public class FileScrapResult : IScrapResult
{
    private Uri _uri;
    private byte[] _content;

    private FileScrapResult(byte[] content, Uri uri)
    {
        _uri = uri;
        _content = content;
    }

    public IEnumerable<string> GetLinkedFiles() => Enumerable.Empty<string>();

    public string GetFileName()
    {
        return _uri.AbsolutePath.Substring(1);
    }

    public byte[] GetContent() => _content;

    public static FileScrapResult Create(byte[] content, Uri uri)
    {
        return new FileScrapResult(content, uri);
    }
    
}
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public class FileWebEntity : IWebEntity
{
    private Uri _uri;
    private byte[] _content;

    private FileWebEntity(byte[] content, Uri uri)
    {
        _uri = uri;
        _content = content;
    }

    public IEnumerable<RelativeUriPath> GetLinkedFiles() => Enumerable.Empty<RelativeUriPath>();

    public string GetFileName()
    {
        return _uri.AbsolutePath.Substring(1);
    }

    public byte[] GetContent() => _content;

    public static FileWebEntity Create(byte[] content, Uri uri)
    {
        return new FileWebEntity(content, uri);
    }
    
}
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public class CssWebEntity : IWebEntity
{
    private readonly Uri _uri;
    private readonly byte[] _content;

    private CssWebEntity(byte[] content, Uri uri)
    {
        _uri = uri;
        _content = content;
    }

    public IEnumerable<RelativeUriPath> GetLinkedFiles()
    {
        var text = Encoding.UTF8.GetString(_content);
        var urlRegex = new Regex(@"url\(['""]?(.*?)['""]?\)", RegexOptions.IgnoreCase);
        
        var matches = urlRegex.Matches(text);
        foreach (Match match in matches)
            if (match.Success)
                yield return new RelativeUriPath(HttpUtility.UrlDecode(new Uri(_uri, match.Groups[1].Value).PathAndQuery.Substring(1)));
    }
    public string GetFileName()
    {
        return _uri.AbsolutePath.Substring(1);
    }

    public byte[] GetContent() => _content;

    public static CssWebEntity Create(byte[] content, Uri uri)
    {
        return new CssWebEntity(content, uri);
    }
    
}
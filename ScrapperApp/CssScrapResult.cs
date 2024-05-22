using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ScrapperApp;

public class CssScrapResult : IScrapResult
{
    private readonly Uri _uri;
    private readonly byte[] _content;

    private CssScrapResult(byte[] content, Uri uri)
    {
        _uri = uri;
        _content = content;
    }

    public IEnumerable<string> GetLinkedFiles()
    {
        var text = Encoding.UTF8.GetString(_content);
        var urlRegex = new Regex(@"url\(['""]?(.*?)['""]?\)", RegexOptions.IgnoreCase);
        
        var matches = urlRegex.Matches(text);
        foreach (Match match in matches)
            if (match.Success)
                yield return HttpUtility.UrlDecode(new Uri(_uri, match.Groups[1].Value).PathAndQuery.Substring(1));
    }
    public string GetFileName()
    {
        return _uri.AbsolutePath.Substring(1);
    }

    public byte[] GetContent() => _content;

    public static CssScrapResult Create(byte[] content, Uri uri)
    {
        return new CssScrapResult(content, uri);
    }
    
}
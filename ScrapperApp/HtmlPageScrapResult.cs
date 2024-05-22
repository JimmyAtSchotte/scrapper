using System.Text;
using HtmlAgilityPack;

namespace ScrapperApp;

public class HtmlPageScrapResult : IScrapResult
{
    private readonly byte[] _bytes;
    private readonly HtmlDocument _htmlDocument;
    private readonly Uri _uri;
    private HtmlPageScrapResult(byte[] bytes, HtmlDocument htmlDocument, Uri uri)
    {
        _bytes = bytes;
        _htmlDocument = htmlDocument;
        _uri = uri;
    }

    public IEnumerable<string> GetLinkedFiles()
    {
        foreach (var link in GetHrefLinks())
            yield return link;
        
        foreach (var link in GetImages())
            yield return link;
        
        foreach (var link in GetCssFiles())
            yield return link;
        
        foreach (var link in GetJsFiles())
            yield return link;
    }

    private IEnumerable<string> GetHrefLinks()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//a[@href]")?
            .Select(linkNode => linkNode.GetAttributeValue("href", ""))
            .Where(link => !link.StartsWith("http") || link.Contains(_uri.Host))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .ToArray() ?? [];
    }

    public static HtmlPageScrapResult Create(byte[] bytes, Uri uri)
    {
        var html = Encoding.UTF8.GetString(bytes);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        
        return new HtmlPageScrapResult(bytes, htmlDocument, uri);
    }

    public string GetFileName()
    {
        if (_uri.AbsolutePath == "/")
            return "index.html";
        
        if (_uri.AbsolutePath.Contains("."))
            return _uri.AbsolutePath.Substring(1);
        
        return _uri.AbsolutePath.EndsWith("/") 
            ? $"{_uri.AbsolutePath}index.html".Substring(1) 
            : $"{_uri.AbsolutePath}/index.html".Substring(1);
    }

    private IEnumerable<string> GetImages()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//img[@src]")?
            .Select(linkNode => linkNode.GetAttributeValue("src", ""))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .ToArray() ?? [];
    }

    private IEnumerable<string> GetCssFiles()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//link[@href]")?
            .Select(linkNode => linkNode.GetAttributeValue("href", ""))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .ToArray() ?? [];
    }

    private IEnumerable<string> GetJsFiles()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//script[@src]")?
            .Select(linkNode => linkNode.GetAttributeValue("src", ""))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .ToArray() ?? [];
    }

    public byte[] GetContent() => _bytes;
}


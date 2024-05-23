using System.Text;
using HtmlAgilityPack;
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public class HtmlWebEntity : IWebEntity
{
    private readonly byte[] _bytes;
    private readonly HtmlDocument _htmlDocument;
    private readonly Uri _uri;
    private HtmlWebEntity(byte[] bytes, HtmlDocument htmlDocument, Uri uri)
    {
        _bytes = bytes;
        _htmlDocument = htmlDocument;
        _uri = uri;
    }

    public IEnumerable<RelativeUriPath> GetLinkedFiles()
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

    private IEnumerable<RelativeUriPath> GetHrefLinks()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//a[@href]")?
            .Select(linkNode => linkNode.GetAttributeValue("href", ""))
            .Where(link => !link.StartsWith("http") || link.Contains(_uri.Host))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .Select(link => new RelativeUriPath(link))
            .ToArray() ?? [];
    }

    public static HtmlWebEntity Create(byte[] bytes, Uri uri)
    {
        var html = Encoding.UTF8.GetString(bytes);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        
        return new HtmlWebEntity(bytes, htmlDocument, uri);
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

    private IEnumerable<RelativeUriPath> GetImages()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//img[@src]")?
            .Select(linkNode => linkNode.GetAttributeValue("src", ""))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .Select(link => new RelativeUriPath(link))
            .ToArray() ?? [];
    }

    private IEnumerable<RelativeUriPath> GetCssFiles()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//link[@href]")?
            .Select(linkNode => linkNode.GetAttributeValue("href", ""))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .Select(link => new RelativeUriPath(link))
            .ToArray() ?? [];
    }

    private IEnumerable<RelativeUriPath> GetJsFiles()
    {
        return _htmlDocument.DocumentNode.SelectNodes("//script[@src]")?
            .Select(linkNode => linkNode.GetAttributeValue("src", ""))
            .Select(link => new Uri(_uri, link).PathAndQuery.Substring(1))
            .Distinct()
            .Select(link => new RelativeUriPath(link))
            .ToArray() ?? [];
    }

    public byte[] GetContent() => _bytes;
}


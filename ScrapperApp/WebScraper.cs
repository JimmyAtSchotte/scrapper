using Microsoft.Extensions.Logging;

namespace ScrapperApp;

public interface IWebScraper
{
    Task<IScrapResult> ScrapPath(string path);
}

public class WebScraper : IWebScraper
{
    private readonly HttpClient _httpClient;

    private static string[] _defaultDocuments = new[]
    {
        "index.html"
    };
    
    public WebScraper(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }
    

    public async Task<IScrapResult> ScrapPath(string path)
    {
        var uri = new Uri(_httpClient.BaseAddress, DefaultDocumentToFolderPath(path));
        
        var response = await _httpClient.GetAsync(uri.AbsolutePath);
        var content = await response.Content.ReadAsByteArrayAsync();
        
        if (response.Content.Headers.ContentType?.MediaType == "text/html")
            return HtmlPageScrapResult.Create(content, uri);
        
        if(response.Content.Headers.ContentType?.MediaType == "text/css")
            return CssScrapResult.Create(content, uri);
        
        return FileScrapResult.Create(content, uri);
    }
   
    private static string DefaultDocumentToFolderPath(string path)
    {
        foreach (var defaultDocument in _defaultDocuments)
            if (path.EndsWith(defaultDocument)) 
                return path[..^defaultDocument.Length];
        
        return path;
    }
}
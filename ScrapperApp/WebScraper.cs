using Microsoft.Extensions.Logging;

namespace ScrapperApp;

public interface IWebScraper
{
    Task<IScrapResult> ScrapPath(string path);
}

public class WebScraper : IWebScraper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebScraper> _logger;

    private static string[] _defaultDocuments = new[]
    {
        "index.html"
    };
    
    public WebScraper(IHttpClientFactory httpClientFactory, ILogger<WebScraper> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }
    

    public async Task<IScrapResult> ScrapPath(string path)
    {
        var uri = new Uri(_httpClient.BaseAddress, DefaultDocumentToFolderPath(path));
        
        _logger.LogTrace("Begin request {Uri}", uri);
        
        var response = await _httpClient.GetAsync(uri.AbsolutePath);
        var content = await response.Content.ReadAsByteArrayAsync();
        
        _logger.LogTrace("Complete request {Uri}, status: {HttpStatus}", uri, response.StatusCode);

        
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
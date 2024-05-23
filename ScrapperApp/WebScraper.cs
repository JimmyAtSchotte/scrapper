using Microsoft.Extensions.Logging;

namespace ScrapperApp;

public interface IWebScraper
{
    Task<IScrapResult> ScrapPath(RelativeUriPath path);
}

public class WebScraper : IWebScraper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebScraper> _logger;

    public WebScraper(IHttpClientFactory httpClientFactory, ILogger<WebScraper> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }
    
    public async Task<IScrapResult> ScrapPath(RelativeUriPath path)
    {
        var uri = path.CreateRelativeUri(_httpClient.BaseAddress);
        
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
}
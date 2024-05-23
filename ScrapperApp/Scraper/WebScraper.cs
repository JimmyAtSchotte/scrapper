using Microsoft.Extensions.Logging;
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Scraper;

public class WebScraper : IWebScraper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebScraper> _logger;

    public WebScraper(IHttpClientFactory httpClientFactory, ILogger<WebScraper> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }
    
    public async Task<IWebEntity> ScrapPath(RelativeUriPath path)
    {
        var uri = path.CreateRelativeUri(_httpClient.BaseAddress);
        
        _logger.LogTrace("Begin request {Uri}", uri);
        
        var response = await _httpClient.GetAsync(uri.AbsolutePath);
        var content = await response.Content.ReadAsByteArrayAsync();
        
        _logger.LogTrace("Complete request {Uri}, status: {HttpStatus}", uri, response.StatusCode);
        
        if (response.Content.Headers.ContentType?.MediaType == "text/html")
            return HtmlWebEntity.Create(content, uri);
        
        if(response.Content.Headers.ContentType?.MediaType == "text/css")
            return CssWebEntity.Create(content, uri);
        
        return FileWebEntity.Create(content, uri);
    }
}
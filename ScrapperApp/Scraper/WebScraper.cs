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
    
    public async Task<Maybe<IWebEntity>> ScrapPath(RelativeUriPath path)
    {
        try
        {
            var uri = path.CreateRelativeUri(_httpClient.BaseAddress);
        
            _logger.LogTrace("Begin request {Uri}", uri);
        
            var response = await _httpClient.GetAsync(uri.AbsolutePath);
            _logger.LogTrace("Complete request {Uri}, status: {HttpStatus}", uri, response.StatusCode);

            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsByteArrayAsync();
            
            return response.Content.Headers.ContentType?.MediaType switch
            {
                "text/html" => Maybe<IWebEntity>.WithValue(HtmlWebEntity.Create(content, uri)),
                "text/css" => Maybe<IWebEntity>.WithValue(CssWebEntity.Create(content, uri)),
                _ => Maybe<IWebEntity>.WithValue(FileWebEntity.Create(content, uri))
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
        
        return Maybe<IWebEntity>.WithoutValue();
    }
}
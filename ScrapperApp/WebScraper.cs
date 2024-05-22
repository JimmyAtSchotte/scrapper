using Microsoft.Extensions.Logging;

namespace ScrapperApp;

public interface IWebScraper
{
    Task<IEnumerable<IScrapResult>> StartScrapping(params string[] paths);
}

public class WebScraper : IWebScraper
{
    private readonly HttpClient _httpClient;
    private readonly List<Uri> _scrappedUrls;
    private readonly ILogger<WebScraper> _logger;

    private static string[] _defaultDocuments = new[]
    {
        "index.html"
    };
    
    public WebScraper(IHttpClientFactory httpClientFactory, ILogger<WebScraper> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _scrappedUrls = new List<Uri>();
    }
    
    public async Task<IEnumerable<IScrapResult>> StartScrapping(params string[] paths)
    {
        if (!paths.Any())
            paths = new[] { "" };
        
        var uris = paths.Select(GetPathUri).Where(uri => !_scrappedUrls.Contains(uri)).ToList();
        var tasks = uris.Select(ScrapUri);
        _scrappedUrls.AddRange(uris);
        return await Task.WhenAll(tasks);
    }

    private async Task<IScrapResult> ScrapUri(Uri uri)
    {
        var response = await _httpClient.GetAsync(uri.AbsolutePath);
        var content = await response.Content.ReadAsByteArrayAsync();
        
        if (response.Content.Headers.ContentType?.MediaType == "text/html")
            return HtmlPageScrapResult.Create(content, uri);
        
        if(response.Content.Headers.ContentType?.MediaType == "text/css")
            return CssScrapResult.Create(content, uri);
        
        return FileScrapResult.Create(content, uri);
    }

    private Uri GetPathUri(string path) =>  new Uri(_httpClient.BaseAddress, DefaultDocumentToFolderPath(path));
    
    private static string DefaultDocumentToFolderPath(string path)
    {
        foreach (var defaultDocument in _defaultDocuments)
            if (path.EndsWith(defaultDocument)) 
                return path[..^defaultDocument.Length];
        
        return path;
    }
}
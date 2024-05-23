using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScrapperApp;

public class WebSiteCrawler : IWebSiteCrawler
{
    private const string DEFAULT_DOCUMENT = "";
    
    private readonly IWebScraper _webScraper;
    private readonly IWebSiteStore _webSiteStore;
    private readonly CrawlerOptions _crawlerOptions;
    private readonly ILogger<WebSiteCrawler> _logger;
    
    public WebSiteCrawler(IWebScraper webScraper, IWebSiteStore webSiteStore, IOptions<CrawlerOptions> crawlerOptions, ILogger<WebSiteCrawler> logger)
    {
        _webScraper = webScraper;
        _webSiteStore = webSiteStore;
        _logger = logger;
        _crawlerOptions = crawlerOptions.Value;
    }

    public async Task StartCrawling()
    {
        var crawlingQueue = new CrawlingQueue();
        crawlingQueue.Enqueue(DEFAULT_DOCUMENT);
        
        while (crawlingQueue.HasQueue)
        {
            _logger.LogInformation("Requests in queue: {QueueCount}", crawlingQueue.Length);
            var batch = crawlingQueue.GetBatch(_crawlerOptions.BatchSize).ToArray();
            
            _logger.LogInformation("Took {BatchSize} requests from queue. Processing them async.", batch.Length);
            
            var tasks = batch.Select(path => _webScraper.ScrapPath(path));
            
            var scrapped = await Task.WhenAll(tasks);
    
            foreach (var scrapResult in scrapped)
            {
                await _webSiteStore.Save(scrapResult.GetFileName(), scrapResult.GetContent());
                
                foreach (var link in scrapResult.GetLinkedFiles())
                    crawlingQueue.Enqueue(link);
            }
        }
    }
}
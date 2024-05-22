using ScrapperApp;

public interface IWebSiteCrawler
{
    Task StartCrawling();
}

public class WebSiteCrawler : IWebSiteCrawler
{
    private readonly IWebScraper _webScraper;
    private readonly IWebSiteStore _webSiteStore;
    
    public WebSiteCrawler(IWebScraper webScraper, IWebSiteStore webSiteStore)
    {
        _webScraper = webScraper;
        _webSiteStore = webSiteStore;
    }

    public async Task StartCrawling()
    {
        var paths = new List<string>();
        paths.Add("");
        
        while (true)
        {
            var scrapped = (await _webScraper.StartScrapping(paths.ToArray())).ToList();
            var nextToScrap = new List<string>();
    
            if(!scrapped.Any())
                break;
    
            foreach (var scrapResult in scrapped)
            {
                await _webSiteStore.Save(scrapResult.GetFileName(), scrapResult.GetContent());
                nextToScrap.AddRange(scrapResult.GetLinkedFiles());
            }

            paths = nextToScrap.Distinct().ToList();
        }
    }
}
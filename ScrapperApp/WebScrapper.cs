namespace ScrapperApp;

public class WebScrapper
{
    public Task<IEnumerable<ScrapResult>> StartScrapping()
    {
        return Task.FromResult(Enumerable.Empty<ScrapResult>());
    }
}
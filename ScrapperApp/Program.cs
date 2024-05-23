using System.IO.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScrapperApp.Crawler;
using ScrapperApp.Scraper;
using ScrapperApp.Storage;

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new List<KeyValuePair<string, string>>
    {
        new ("Store:Path", "Scraped"),
        new ("Url", "https://books.toscrape.com/"),
        new ("Crawler:BatchSize", "100")
    }!)
    .AddJsonFile("appsettings.json")
    .AddCommandLine(args)
    .Build();

var serviceCollection = new ServiceCollection();
serviceCollection.AddOptions<StoreOptions>().Bind(configuration.GetSection("Store"));
serviceCollection.AddOptions<CrawlerOptions>().Bind(configuration.GetSection("Crawler"));
serviceCollection.AddHttpClient("", client => client.BaseAddress = new Uri(configuration.GetValue<string>("Url")));
serviceCollection.AddLogging(logger =>
{
    logger.AddConfiguration(configuration.GetSection("Logging"));
    logger.AddConsole();
});
serviceCollection.AddScoped<IWebScraper, WebScraper>();
serviceCollection.AddScoped<IWebSiteCrawler, WebSiteCrawler>();
serviceCollection.AddScoped<IWebSiteStore, LocalWebSiteStore>();
serviceCollection.AddScoped<IFileSystem, FileSystem>();

var services = serviceCollection.BuildServiceProvider();
var webSiteCrawler = services.GetService<IWebSiteCrawler>();
await webSiteCrawler.StartCrawling();
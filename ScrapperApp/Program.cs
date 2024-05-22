using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScrapperApp;

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new List<KeyValuePair<string, string>>
    {
        new ("Store:Path", "Scraped"),
        new ("Url", "https://books.toscrape.com/")
    }!)
    .AddCommandLine(args)
    .Build();

var serviceCollection = new ServiceCollection();
serviceCollection.AddOptions<StoreOptions>().Bind(configuration.GetSection("Store"));
serviceCollection.AddHttpClient("", client => client.BaseAddress = new Uri(configuration.GetValue<string>("Url")));
serviceCollection.AddLogging(logger => logger.AddConsole());
serviceCollection.AddScoped<IWebScraper, WebScraper>();
serviceCollection.AddScoped<IWebSiteCrawler, WebSiteCrawler>();
serviceCollection.AddScoped<IWebSiteStore, WebSiteStore>();

var services = serviceCollection.BuildServiceProvider();
var webSiteCrawler = services.GetService<IWebSiteCrawler>();
await webSiteCrawler.StartCrawling();
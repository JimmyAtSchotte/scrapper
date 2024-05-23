using System.Text;
using ArrangeDependencies.Autofac;
using ArrangeDependencies.Autofac.Extensions;
using ArrangeDependencies.Autofac.HttpClient;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using ScrapperApp.Crawler;
using ScrapperApp.Scraper;
using ScrapperApp.SharedKernel;
using ScrapperApp.Storage;

namespace ScrapperApp.Tests.Crawler;

[TestFixture]
public class WebCrawlerTests
{
    private readonly CrawlerOptions _crawlerOptions = new CrawlerOptions()
    {
        BatchSize = 10
    };
    
    [Test]
    public async Task StoreLinkedPage()
    {
        var baseUri = new Uri("http://localhost/");
        var pageUri = new Uri(baseUri, "page");

        var indexHtml = "<html><body><a href=\"page\">LINK</a></body></html>";
        var pageHtml = "<html><body></body></html>";
        var storePaths = new List<string>();
      
        
        var arrange = Arrange.Dependencies<IWebSiteCrawler, WebSiteCrawler>(dependencies =>
        {
            dependencies.UseImplementation<IWebScraper, WebScraper>();
            dependencies.UseMock<IOptions<CrawlerOptions>>(mock => mock.SetupGet(x => x.Value).Returns(_crawlerOptions));
            dependencies.UseHttpClientFactory(client => client.BaseAddress = baseUri, 
                HttpClientConfig.Create(baseUri, response => response.Content = new StringContent(indexHtml, Encoding.UTF8, "text/html")),
                HttpClientConfig.Create(pageUri, response => response.Content = new StringContent(pageHtml,  Encoding.UTF8, "text/html"))
            );
            dependencies.UseMock<IWebSiteStore>(mock => mock.Setup(x => x.Save(Capture.In(storePaths), It.IsAny<byte[]>())));
        });

        var crawler = arrange.Resolve<IWebSiteCrawler>();
        await crawler.StartCrawling();

        storePaths.Should().Contain("index.html");
        storePaths.Should().Contain("page/index.html");
    }
    
    
    [Test]
    public async Task SkipFailedScrapes()
    {
        var storePaths = new List<string>();
        
        var arrange = Arrange.Dependencies<IWebSiteCrawler, WebSiteCrawler>(dependencies =>
        {
            dependencies.UseMock<IOptions<CrawlerOptions>>(mock => mock.SetupGet(x => x.Value).Returns(_crawlerOptions));
            dependencies.UseMock<IWebScraper>(mock => mock.Setup(x => x.ScrapPath(It.IsAny<RelativeUriPath>())).ReturnsAsync(Maybe<IWebEntity>.WithoutValue));
            dependencies.UseMock<IWebSiteStore>(mock => mock.Setup(x => x.Save(Capture.In(storePaths), It.IsAny<byte[]>())));
        });

        var crawler = arrange.Resolve<IWebSiteCrawler>();
        await crawler.StartCrawling();

        storePaths.Should().BeEmpty();
    }
}
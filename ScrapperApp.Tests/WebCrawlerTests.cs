using System.Text;
using ArrangeDependencies.Autofac;
using ArrangeDependencies.Autofac.Extensions;
using ArrangeDependencies.Autofac.HttpClient;
using FluentAssertions;
using Moq;

namespace ScrapperApp.Tests;

[TestFixture]
public class WebCrawlerTests
{
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
}
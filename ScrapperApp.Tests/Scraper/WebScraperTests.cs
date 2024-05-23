using System.Text;
using ArrangeDependencies.Autofac;
using ArrangeDependencies.Autofac.HttpClient;
using FluentAssertions;
using ScrapperApp.Scraper;
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Tests.Scraper;

public class WebScraperTests
{
    
    [Test]
    public async Task ScrapHtmlPage()
    {
        var html = "<html><body></body></html>";
        
        var arrange = Arrange.Dependencies<WebScraper, WebScraper>(dependencies =>
        {
            dependencies.UseHttpClientFactory(client => client.BaseAddress = new Uri("http://localhost/"), 
            HttpClientConfig.Create(new Uri("http://localhost/"), response => response.Content = new StringContent(html, Encoding.UTF8, "text/html")));
        });

        var scrapper = arrange.Resolve<WebScraper>();
        var scrapResult = await scrapper.ScrapPath(new RelativeUriPath(""));

        Encoding.UTF8.GetString(scrapResult.GetContent()).Should().Be(html);
    }
    
    [Test]
    public async Task ScrapHrefLink()
    {
        var indexHtml = "<html><body><a href=\"pageA.html\">LINK</a></body></html>";
        
        var arrange = Arrange.Dependencies<WebScraper, WebScraper>(dependencies =>
        {
            dependencies.UseHttpClientFactory(client => client.BaseAddress = new Uri("http://localhost/"), 
                HttpClientConfig.Create(new Uri("http://localhost/"), response => response.Content = new StringContent(indexHtml, Encoding.UTF8, "text/html"))
            );
        });

        var scrapper = arrange.Resolve<WebScraper>();
        var scrapResult = await scrapper.ScrapPath(new RelativeUriPath(""));

        scrapResult.GetLinkedFiles().Should().Contain(x => x.ToString() == "pageA.html");
    }
    
    [Test]
    public async Task ScrapSubFolderHrefLink()
    {
        var indexHtml = "<html><body><a href=\"pageA\">LINK</a></body></html>";
        
        var arrange = Arrange.Dependencies<WebScraper, WebScraper>(dependencies =>
        {
            dependencies.UseHttpClientFactory(client => client.BaseAddress = new Uri("http://localhost/"), 
            HttpClientConfig.Create(new Uri("http://localhost/"), response => response.Content = new StringContent(indexHtml, Encoding.UTF8, "text/html"))
            );
        });

        var scrapper = arrange.Resolve<WebScraper>();
        var scrapResult = await scrapper.ScrapPath(new RelativeUriPath(""));
        
        scrapResult.GetLinkedFiles().Should().Contain(x => x.ToString() == "pageA");
    }
    
    [Test]
    public async Task RelativeUrls()
    {
        var pageAHtml = "<html><body><a href=\"../\">LINK</a></body></html>";
        
        var arrange = Arrange.Dependencies<WebScraper, WebScraper>(dependencies =>
        {
            dependencies.UseHttpClientFactory(client => client.BaseAddress = new Uri("http://localhost/"), 
            HttpClientConfig.Create(new Uri("http://localhost/pageA"), response => response.Content = new StringContent(pageAHtml, Encoding.UTF8, "text/html"))
            );
        });

        var scrapper = arrange.Resolve<WebScraper>();
        var scrapResult = await scrapper.ScrapPath(new RelativeUriPath("pageA"));

        scrapResult.GetLinkedFiles().Should().Contain(x => x.ToString() =="");
    }
    
    [Test]
    public async Task SkipExternalLink()
    {
        var indexHtml = "<html><body><a href=\"http://external.com/pageA/page.html\">LINK</a></body></html>";
   
        var arrange = Arrange.Dependencies<WebScraper, WebScraper>(dependencies =>
        {
            dependencies.UseHttpClientFactory(client => client.BaseAddress = new Uri("http://localhost/"), 
            HttpClientConfig.Create(new Uri("http://localhost/"), response => response.Content = new StringContent(indexHtml, Encoding.UTF8, "text/html"))
            );
        });

        var scrapper = arrange.Resolve<WebScraper>();
        var scrapResult = await scrapper.ScrapPath(new RelativeUriPath(""));

        scrapResult.GetLinkedFiles().Should().HaveCount(0);
    }
    
    [Test]
    public async Task FullInternalUrl()
    {
        var indexHtml = "<html><body><a href=\"http://localhost/page.html\">LINK</a></body></html>";
   
        var arrange = Arrange.Dependencies<WebScraper, WebScraper>(dependencies =>
        {
            dependencies.UseHttpClientFactory(client => client.BaseAddress = new Uri("http://localhost/"), 
            HttpClientConfig.Create(new Uri("http://localhost/"), response => response.Content = new StringContent(indexHtml, Encoding.UTF8, "text/html"))
            );
        });

        var scrapper = arrange.Resolve<WebScraper>();
        var scrapResult = await scrapper.ScrapPath(new RelativeUriPath(""));

        scrapResult.GetLinkedFiles().Should().HaveCount(1);
    }
    
    
    [Test]
    public async Task CssFileLinks()
    {
        var css = "@font-face {\n  font-family: 'FontAwesome';\n  src: url('../fonts/fontawesome-webfont.eot%3Fv=3.2.1');\n  src: url('../fonts/fontawesome-webfont.eot%3F') format('embedded-opentype'), url('../fonts/fontawesome-webfont.woff%3Fv=3.2.1') format('woff'), url('../fonts/fontawesome-webfont.ttf%3Fv=3.2.1') format('truetype'), url('../fonts/fontawesome-webfont.svg') format('svg');\n  font-weight: normal;\n  font-style: normal;\n}";
   
        var arrange = Arrange.Dependencies<WebScraper, WebScraper>(dependencies =>
        {
            dependencies.UseHttpClientFactory(client => client.BaseAddress = new Uri("http://localhost/"), 
            HttpClientConfig.Create(new Uri("http://localhost/"), response => response.Content = new StringContent(css, Encoding.UTF8, "text/css"))
            );
        });

        var scrapper = arrange.Resolve<WebScraper>();
        var scrapResult = await scrapper.ScrapPath(new RelativeUriPath(""));

        scrapResult.GetLinkedFiles().Should().HaveCount(5);
    }
}
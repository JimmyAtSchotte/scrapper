using System.Text;
using FluentAssertions;
using ScrapperApp.Scraper;

namespace ScrapperApp.Tests.Scraper;

[TestFixture]
public class HtmlWebEntityTests
{
    [Test]
    public void HtmlPageNoLinks()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><body></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);

        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var links = htmlPage.GetLinkedFiles();

        links.Should().BeEmpty();
    }
    
    [Test]
    public void HasHrefLink()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><body><a href=\"page.html\"</body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var links = htmlPage.GetLinkedFiles();

        links.Should().HaveCount(1);
    }
    
    [Test]
    public void HasRelativeHrefLink()
    {
        var baseAddress = new Uri("https://localhost/");
        var uri = new Uri(baseAddress, "pages/page1");
        var content = "<html><body><a href=\"../pages/page2\"</body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var links = htmlPage.GetLinkedFiles();

        links.FirstOrDefault().CreateRelativeUri(baseAddress).PathAndQuery.Should().Be("/pages/page2");
    }
    
    [Test]
    public void HasExternalHrefLink()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><body><a href=\"https://exernal/page.html\"</body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var links = htmlPage.GetLinkedFiles();

        links.Should().BeEmpty();
    }
    
    [Test]
    public void HasInternalFullUrl()
    {
        var uri = new Uri("https://localhost/");
        var content = $"<html><body><a href=\"https://{uri.Host}/page.html\"</body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var links = htmlPage.GetLinkedFiles();

        links.Should().HaveCount(1);
    }
    
    [Test]
    public void EmptyHrefAttribute()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><body><a href></a></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var links = htmlPage.GetLinkedFiles();

        links.Should().HaveCount(0);
    }
    
    [Test]
    public void GetFileNameOfRoot()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><body><a href=\"page.html\"</body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var fileName = htmlPage.GetFileName();

        fileName.Should().Be("index.html");
    }
    
    [Test]
    public void GetFileNameOfPage()
    {
        var uri = new Uri("https://localhost/page.html");
        var content = "<html><body><a href=\"page.html\"</body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var fileName = htmlPage.GetFileName();

        fileName.Should().Be("page.html");
    }
    
    [TestCase("dir")]
    [TestCase("dir/")]
    public void GetFileNameOfSubDirectory(string dir)
    {
        var uri = new Uri($"https://localhost/{dir}");
        var content = "<html><body><a href=\"page.html\"</body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var fileName = htmlPage.GetFileName();

        fileName.Should().Be("dir/index.html");
    }
    
    [Test]
    public void HasImagesLink()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><body><img src=\"image.jpg\"></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var images = htmlPage.GetLinkedFiles();

        images.Should().HaveCount(1);
    }
    
    [Test]
    public void EmptyImgSrcAttribute()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><body><img src></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var images = htmlPage.GetLinkedFiles();

        images.Should().HaveCount(0);
    }
    
    [Test]
    public void HasCssFiles()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><link href=\"css.css\" /><body></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var cssFiles = htmlPage.GetLinkedFiles();

        cssFiles.Should().HaveCount(1);
    }
    
        
    [Test]
    public void EmptyCssAttribute()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><link href/><body></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var images = htmlPage.GetLinkedFiles();

        images.Should().HaveCount(0);
    }
    
    [Test]
    public void HasJavascriptFiles()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><script src=\"js.js\" /><body></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var jsFiles = htmlPage.GetLinkedFiles();

        jsFiles.Should().HaveCount(1);
    }
    
    [Test]
    public void EmptyJavascriptSrcAttribute()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><script src /><body></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var images = htmlPage.GetLinkedFiles();

        images.Should().HaveCount(0);
    }
    
    [Test]
    public void GetContent()
    {
        var uri = new Uri("https://localhost/");
        var content = "<html><script src=\"js.js\" /><body></body></html>";
        var bytes = Encoding.UTF8.GetBytes(content);
        var htmlPage = HtmlWebEntity.Create(bytes, uri);
        var contentByte = htmlPage.GetContent();

        contentByte.Should().BeEquivalentTo(bytes, o => o.WithoutStrictOrdering());
    }
}
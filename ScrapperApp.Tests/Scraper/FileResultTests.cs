using System.Text;
using FluentAssertions;
using ScrapperApp.Scraper;

namespace ScrapperApp.Tests.Scraper;

[TestFixture]
public class FileResultTests
{
    [Test]
    public void RemoveQueryStringFromFileName()
    {
        var uri = new Uri("https://localhost/fonts/fontawesome-webfont.eot?v=3.2.1");
        var bytes = Array.Empty<byte>();

        var file = FileWebEntity.Create(bytes, uri);
        
        file.GetFileName().Should().Be("fonts/fontawesome-webfont.eot");
    }
    
    [Test]
    public void NoLinks()
    {
        var uri = new Uri("https://localhost/fonts/fontawesome-webfont.eot");
        var bytes = Array.Empty<byte>();
        var file = FileWebEntity.Create(bytes, uri);

        file.GetLinkedFiles().Should().BeEmpty();
    }
    
    [Test]
    public void OriginalContent()
    {
        var originalContent = "Hello World";
        var uri = new Uri("https://localhost/fonts/fontawesome-webfont.eot");
        var bytes = Encoding.UTF8.GetBytes(originalContent);
        var file = FileWebEntity.Create(bytes, uri);

        var content = Encoding.UTF8.GetString(file.GetContent());
        content.Should().Be(originalContent);
    }
    
    [Test]
    public void Filename()
    {
        var uri = new Uri("https://localhost/fonts/fontawesome-webfont.eot");
        var bytes = Array.Empty<byte>();
        var file = FileWebEntity.Create(bytes, uri);

        file.GetFileName().Should().Be("fonts/fontawesome-webfont.eot");
    }
}
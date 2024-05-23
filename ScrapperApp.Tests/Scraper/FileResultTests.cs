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

        var file = CssScrapResult.Create(bytes, uri);
        
        file.GetFileName().Should().Be("fonts/fontawesome-webfont.eot");
    }
}
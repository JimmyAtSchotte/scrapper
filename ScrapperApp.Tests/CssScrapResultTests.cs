using System.Text;
using FluentAssertions;
using ScrapperApp.Scraper;

namespace ScrapperApp.Tests;

[TestFixture]
public class CssScrapResultTests
{
    [Test]
    public void CssWithLinks()
    {
        var uri = new Uri("https://localhost/");
        var content = "@font-face {\n  font-family: 'FontAwesome';\n  src: url('../fonts/fontawesome-webfont.eot%3Fv=3.2.1');\n  src: url('../fonts/fontawesome-webfont.eot%3F') format('embedded-opentype'), url('../fonts/fontawesome-webfont.woff%3Fv=3.2.1') format('woff'), url('../fonts/fontawesome-webfont.ttf%3Fv=3.2.1') format('truetype'), url('../fonts/fontawesome-webfont.svg') format('svg');\n  font-weight: normal;\n  font-style: normal;\n}";
        var bytes = Encoding.UTF8.GetBytes(content);

        var cssFile = CssScrapResult.Create(bytes, uri);
        var links = cssFile.GetLinkedFiles().ToList();

        links.Should().HaveCount(5);
        links.Should().Contain(x => x.ToString() == "fonts/fontawesome-webfont.eot?v=3.2.1");
        links.Should().Contain(x => x.ToString() == "fonts/fontawesome-webfont.eot?");
        links.Should().Contain(x => x.ToString() == "fonts/fontawesome-webfont.woff?v=3.2.1");
        links.Should().Contain(x => x.ToString() == "fonts/fontawesome-webfont.ttf?v=3.2.1");
        links.Should().Contain(x => x.ToString() == "fonts/fontawesome-webfont.svg");
    }
}
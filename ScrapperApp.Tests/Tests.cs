using FluentAssertions;

namespace ScrapperApp.Tests;

public class Tests
{
    [Test]
    public async Task NothingToFetch()
    {
        var scrapper = new WebScrapper();
        var scrapResult = await scrapper.StartScrapping();

        scrapResult.Should().BeEmpty();
    }
}
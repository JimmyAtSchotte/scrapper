using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using ArrangeDependencies.Autofac;
using ArrangeDependencies.Autofac.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Options;
using ScrapperApp.Storage;

namespace ScrapperApp.Tests.Storage;

[TestFixture]
public class LocalWebSiteStoreTests
{
    [Test]
    public async Task Save()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var options = new StoreOptions()
        {
            Path = "Scraped"
        };
        var filename = "file.html";
        var fileContent = "<html>";
        var expectedPath = fileSystem.Path.Combine(fileSystem.Directory.GetCurrentDirectory(), options.Path, filename);
        
        var arrange = Arrange.Dependencies<IWebSiteStore, LocalWebSiteStore>(dependencies =>
        {
            dependencies.UseImplementation<IFileSystem, MockFileSystem>(fileSystem);
            dependencies.UseMock<IOptions<StoreOptions>>(mock => mock.Setup(x => x.Value).Returns(options));
        });

        var store = arrange.Resolve<IWebSiteStore>();
        var bytes = Encoding.UTF8.GetBytes(fileContent);
        await store.Save("file.html", bytes);
        
        fileSystem.AllFiles.Should().HaveCount(1);
        fileSystem.AllFiles.FirstOrDefault().Should().Be(expectedPath);
        (await fileSystem.File.ReadAllBytesAsync(expectedPath)).Should().BeEquivalentTo(bytes,  o => o.WithStrictOrdering());
    }
}
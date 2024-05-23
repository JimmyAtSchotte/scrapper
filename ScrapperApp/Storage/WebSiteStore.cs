using Microsoft.Extensions.Options;

namespace ScrapperApp.Storage;

public class WebSiteStore : IWebSiteStore
{
    private readonly StoreOptions _storeOptions;

    public WebSiteStore(IOptions<StoreOptions> storeOptions)
    {
        _storeOptions = storeOptions.Value;
    }


    public async Task Save(string filename, byte[] bytes)
    {
        var targetPath = Path.Combine(_storeOptions.Path, filename);
        var directory = Path.GetDirectoryName(targetPath);
    
        if(!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    
        await File.WriteAllBytesAsync(targetPath, bytes);
    }
}
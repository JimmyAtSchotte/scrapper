using System.IO.Abstractions;
using Microsoft.Extensions.Options;

namespace ScrapperApp.Storage;

public class LocalWebSiteStore : IWebSiteStore
{
    private readonly StoreOptions _storeOptions;
    private readonly IFileSystem _fileSystem;

    public LocalWebSiteStore(IOptions<StoreOptions> storeOptions, IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _storeOptions = storeOptions.Value;
    }

    public async Task Save(string filename, byte[] bytes)
    {
        var targetPath = _fileSystem.Path.Combine(_storeOptions.Path, filename);
        var directory =  _fileSystem.Path.GetDirectoryName(targetPath);
    
        if(!string.IsNullOrEmpty(directory))
            _fileSystem.Directory.CreateDirectory(directory);
    
        await _fileSystem.File.WriteAllBytesAsync(targetPath, bytes);
    }
}
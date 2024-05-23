namespace ScrapperApp.SharedKernel;

public class RelativeUriPath
{
    private static string[] _defaultDocuments = new[]
    {
        "index.html"
    };
    
    private readonly string _path;
    
    public RelativeUriPath(string path)
    {
        _path = path;
    }
    
    public Uri CreateRelativeUri(Uri? baseAddress)
    {
        return new Uri(baseAddress, _path);
    }
    
    private static string DefaultDocumentToFolderPath(string path)
    {
        foreach (var defaultDocument in _defaultDocuments)
            if (path.EndsWith(defaultDocument)) 
                return path[..^defaultDocument.Length];
        
        return path;
    }

    public override int GetHashCode()
    {
        var pathIdentifier = DefaultDocumentToFolderPath(_path);

        return RemoveTrailingSlash(pathIdentifier).GetHashCode();
    }
    
    private static string RemoveTrailingSlash(string path)
    {
        return path.EndsWith("/") ? path[..^1] : path;
    }
}

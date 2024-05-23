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
        return new Uri(baseAddress, ToString());
    }

    public override string ToString()
    {
        var path = _path;
        
        path = DefaultDocumentToFolderPath(path);
        path = RemoveTrailingSlash(path);
        
        return path;
    }
    
    private static string DefaultDocumentToFolderPath(string path)
    {
        foreach (var defaultDocument in _defaultDocuments)
            if (path.EndsWith(defaultDocument)) 
                return path[..^defaultDocument.Length];
        
        return path;
    }
    
    private static string RemoveTrailingSlash(string path)
    {
        return path.EndsWith("/") ? path[..^1] : path;
    }
}

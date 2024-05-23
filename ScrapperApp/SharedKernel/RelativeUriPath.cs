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

    public Uri CreateRelativeUri(Uri baseAddress)
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

    public override bool Equals(object obj)
    {
        if (obj is RelativeUriPath other)
            return ToString() == other.ToString();
        
        if (obj is string otherString)
            return ToString() == otherString;

        return false;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public static bool operator ==(RelativeUriPath left, string right)
    {
        if (left is null)
            return right is null;

        return left.ToString() == right;
    }

    public static bool operator !=(RelativeUriPath left, string right)
    {
        return !(left == right);
    }

    public static bool operator ==(string left, RelativeUriPath right)
    {
        if (right is null)
            return left is null;

        return right.ToString() == left;
    }

    public static bool operator !=(string left, RelativeUriPath right)
    {
        return !(left == right);
    }

    public static implicit operator string(RelativeUriPath relativeUriPath)
    {
        return relativeUriPath?.ToString();
    }
}

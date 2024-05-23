namespace ScrapperApp;

public class CrawlingQueue
{
    private readonly Queue<string> _queue;
    private readonly HashSet<string> _memory;
    
    private static string[] _defaultDocuments = new[]
    {
        "index.html"
    };
    
    public CrawlingQueue()
    {
        _queue = new Queue<string>();
        _memory = new HashSet<string>();
    }

    public bool HasQueue => _queue.Any();
    public int Length => _queue.Count;

    public void Enqueue(string path)
    {
        var queuePath = DefaultDocumentToFolderPath(path);
        queuePath = RemoveTrailingSlash(queuePath);
        
        if(_memory.Contains(queuePath))
            return;
        
        _queue.Enqueue(queuePath);
        _memory.Add(queuePath);
    }

    public IEnumerable<string> GetBatch(int batchSize)
    {
        for (var i = 0; i < batchSize; i++)
        {
            if(!_queue.Any())
                yield break;
            
            yield return _queue.Dequeue();  
        }
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
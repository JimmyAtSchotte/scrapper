using ScrapperApp.SharedKernel;

namespace ScrapperApp.Crawler;

public class CrawlingQueue
{
    private readonly Queue<RelativeUriPath> _queue;
    private readonly HashSet<string> _memory;
    
    private static string[] _defaultDocuments = new[]
    {
        "index.html"
    };
    
    public CrawlingQueue()
    {
        _queue = new Queue<RelativeUriPath>();
        _memory = new HashSet<string>();
    }

    public bool HasQueue => _queue.Any();
    public int Length => _queue.Count;


    public void Enqueue(string path)
    {
        Enqueue(new RelativeUriPath(path));
    }
    
    public void Enqueue(RelativeUriPath path)
    {
        if(_memory.Contains(path.ToString()))
            return;
        
        _queue.Enqueue(path);
        _memory.Add(path.ToString());
    }

    public IEnumerable<RelativeUriPath> GetBatch(int batchSize)
    {
        for (var i = 0; i < batchSize; i++)
        {
            if(!_queue.Any())
                yield break;
            
            yield return _queue.Dequeue();  
        }
    }
}
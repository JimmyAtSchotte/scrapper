using ScrapperApp.SharedKernel;

namespace ScrapperApp.Crawler;

public class CrawlingQueue
{
    private readonly Queue<RelativeUriPath> _queue;
    private readonly HashSet<int> _memory;
    
    public CrawlingQueue()
    {
        _queue = new Queue<RelativeUriPath>();
        _memory = new HashSet<int>();
    }

    public bool HasQueue => _queue.Any();
    public int Length => _queue.Count;


    public void Enqueue(string path)
    {
        Enqueue(new RelativeUriPath(path));
    }
    
    public void Enqueue(RelativeUriPath path)
    {
        var hashCode = path.GetHashCode();
        
        if(_memory.Contains(hashCode))
            return;
        
        _queue.Enqueue(path);
        _memory.Add(hashCode);
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
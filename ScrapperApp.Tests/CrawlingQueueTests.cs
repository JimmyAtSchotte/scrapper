using FluentAssertions;

namespace ScrapperApp.Tests;

[TestFixture]
public class CrawlingQueueTests
{
    [Test]
    public void AddToQueue()
    {
        var queue = new CrawlingQueue();
        queue.Enqueue("");
        var batch = queue.GetBatch(10);
        batch.Should().HaveCount(1);
    }
    
    [Test]
    public void TakeBatch()
    {
        var queue = new CrawlingQueue();

        for (int i = 0; i < 15; i++)
            queue.Enqueue(i.ToString());
        
        var batch1 = queue.GetBatch(10).ToList();;
        var batch2 = queue.GetBatch(10).ToList();;
        
        batch1.Should().HaveCount(10);
        batch2.Should().HaveCount(5);
    }
    
    [Test]
    public void SkipEnqueueSameItem()
    {
        var queue = new CrawlingQueue();

        for (int i = 0; i < 15; i++)
            queue.Enqueue("hello");
        
        var batch = queue.GetBatch(10);
        
        batch.Should().HaveCount(1);
    }
    
    [Test]
    public void SkipEnqueuePreviouslyQueued()
    {
        var queue = new CrawlingQueue();

        queue.Enqueue("hello");
        var batch1 = queue.GetBatch(10).ToList();
        queue.Enqueue("hello");
        var batch2 = queue.GetBatch(10).ToList();;
        
        batch1.Should().HaveCount(1);
        batch2.Should().HaveCount(0);
    }

    [Test]
    public void RequeueDefaultDocuments()
    {
        var queue = new CrawlingQueue();
        queue.Enqueue("");
        queue.Enqueue("index.html");
        queue.Enqueue("page");
        queue.Enqueue("page/");
        queue.Enqueue("page/index.html");

        queue.GetBatch(10).Should().HaveCount(2);

    }
}
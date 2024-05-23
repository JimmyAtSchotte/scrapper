using FluentAssertions;
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Tests.SharedKernel;

public class MaybeTests
{
    [Test]
    public void WithoutValue()
    {
        var maybe = Maybe<string>.WithoutValue();

        maybe.TryGetValue(out var res).Should().BeFalse();
        res.Should().BeNull();
    }
    
    [Test]
    public void WithValue()
    {
        var maybe = Maybe<string>.WithValue("hello");

        maybe.TryGetValue(out var res).Should().BeTrue();
        res.Should().Be("hello");
    }
}
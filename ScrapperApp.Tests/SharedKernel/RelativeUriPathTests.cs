using FluentAssertions;
using ScrapperApp.SharedKernel;

namespace ScrapperApp.Tests.SharedKernel;

[TestFixture]
public class RelativeUriPathTests
{
    [Test]
    public void BaseAddressIsNull()
    {
        var relativeUriPath = new RelativeUriPath("");

        relativeUriPath.Invoking(x => x.CreateRelativeUri(null)).Should().Throw<ArgumentNullException>();
    }
}
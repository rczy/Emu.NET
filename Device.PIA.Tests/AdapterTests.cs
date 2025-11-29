using Xunit;

namespace Device.PIA.Tests;

[Trait("Category", "Unit")]
public class AdapterTests
{
    [Fact]
    public void SampleTest_ShouldRun_Successfully()
    {
        Adapter pia = new();
        Assert.Throws<NotImplementedException>(() => pia.Reset());
        Assert.Throws<NotImplementedException>(() => pia.Read(0x0000));
        Assert.Throws<NotImplementedException>(() => pia.Write(0x0000, 0xAB));
    }
}
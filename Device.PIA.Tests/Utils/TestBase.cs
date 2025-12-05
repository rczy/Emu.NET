using Device.PIA.Internals;

namespace Device.PIA.Tests.Utils;

[Trait("Category", "Unit")]
public class TestBase
{
    protected readonly Adapter pia;
    protected readonly TestPeripheral pA;
    protected readonly TestPeripheral pB;

    protected TestBase()
    {
        pia = new Adapter();
        pA = new TestPeripheral();
        pB = new TestPeripheral();
        
        pia.Connect(PortSection.A, pA);
        pia.Connect(PortSection.B, pB);
    }
}
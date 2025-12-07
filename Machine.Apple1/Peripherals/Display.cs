using Device.PIA;
using Device.PIA.Internals;

namespace Machine.Apple1.Peripherals;

public class Display : IPeripheral
{
    public void Connect(Port port)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public byte OnRead()
    {
        throw new NotImplementedException();
    }

    public void OnWrite(byte data)
    {
        throw new NotImplementedException();
    }
}
using Device.PIA.Internals;

namespace Device.PIA;

public interface IPeripheral
{
    public void Connect(Port port);
    public void Reset();
    public byte OnRead();
    public void OnWrite(byte data);
}
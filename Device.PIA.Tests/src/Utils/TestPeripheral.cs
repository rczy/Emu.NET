using Device.PIA.Internals;

namespace Device.PIA.Tests.Utils;

public class TestPeripheral : IPeripheral
{
    public byte Data { get; set; }
    
    public Port? Port { get; private set; }
    
    public void Connect(Port port)
    {
        Port = port;
    }
    
    public void Reset()
    {
    }
    
    public byte OnRead()
    {
        return Data;
    }
    
    public void OnWrite(byte data)
    {
        Data = data;
    }
    
    public void SetC1(bool level)
    {
        if (Port != null) Port.InterruptControl.C1 = level;
    }
    
    public void SetC2(bool level)
    {
        if (Port != null) Port.InterruptControl.C2 = level;
    }
}
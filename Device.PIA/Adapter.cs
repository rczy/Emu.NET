namespace Device.PIA;

using CPU.MOS6502;

public class Adapter : IDevice
{
    public void Reset()
    {
        throw new NotImplementedException();
    }

    public byte Read(ushort address)
    {
        throw new NotImplementedException();
    }

    public void Write(ushort address, byte data)
    {
        throw new NotImplementedException();
    }
}
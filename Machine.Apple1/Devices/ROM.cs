using CPU.MOS6502;

namespace Machine.Apple1.Devices;

public class ROM: IDevice
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
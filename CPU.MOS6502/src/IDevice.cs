namespace CPU.MOS6502;

public interface IDevice
{
    public void Reset();
    public byte Read(ushort address);
    public void Write(ushort address, byte data);
}
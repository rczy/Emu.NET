namespace CPU.MOS6502;

public abstract class Device
{
    public required string Name { get; init; }

    abstract public void Boot(Bus bus);
    abstract public void Reset();
    abstract public byte Read(ushort address);
    abstract public void Write(ushort address, byte data);
}
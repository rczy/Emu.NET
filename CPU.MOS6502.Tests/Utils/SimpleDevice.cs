namespace CPU.MOS6502.Tests.Utils;

public class SimpleDevice(int storageSize) : Device
{
    private readonly byte[] _storage = new byte[storageSize];
    public byte ReadCount { get; private set; }
    public byte WriteCount { get; private set; }
    public ushort LastReadAddress { get; private set; }
    public ushort LastWriteAddress { get; private set; }

    public override void Boot(Bus bus)
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        Array.Clear(_storage);
        ReadCount = WriteCount = 0;
        LastReadAddress = LastWriteAddress = 0;
    }

    public override byte Read(ushort address)
    {
        ReadCount++;
        LastReadAddress = address;
        return _storage[address];
    }

    public override void Write(ushort address, byte data)
    {
        WriteCount++;
        LastWriteAddress = address;
        _storage[address] = data;
    }

    public void LoadData(byte[] data)
    {
        Array.Copy(data, _storage, data.Length);
    }
    
    public void LoadData(byte[] data, int startAddress)
    {
        Array.Copy(data, 0, _storage, startAddress, data.Length);
    }

    public byte PeekAt(ushort address)
    {
        return _storage[address];
    }
}
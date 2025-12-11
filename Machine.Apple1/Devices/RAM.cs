using CPU.MOS6502;

namespace Machine.Apple1.Devices;

public class RAM : IDevice
{
    private readonly byte[] _storage = new byte[0x8000]; // 32K
    
    public void Reset()
    {
        Array.Clear(_storage, 0, _storage.Length);
    }
    
    public byte Read(ushort address)
    {
        return _storage[address];
    }
    
    public void Write(ushort address, byte data)
    {
        _storage[address] = data;
    }
    
    public void LoadData(byte[] data, int startAddress)
    {
        Array.Copy(data, 0, _storage, startAddress, data.Length);
    }
}
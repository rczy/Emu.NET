using CPU.MOS6502;

namespace Machine.Apple1.Devices;

public class ROM : IDevice
{
    private readonly byte[] _storage = new byte[0x2000]; // 8K

    public ROM()
    {
        LoadData(File.ReadAllBytes(@"binaries\basic.bin"), 0); // Apple BASIC
        LoadData(File.ReadAllBytes(@"binaries\monitor.bin"), 0x1F00); // Woz Monitor
    }
    
    public void Reset()
    {
        // do nothing
    }
    
    public byte Read(ushort address)
    {
        return _storage[address];
    }
    
    public void Write(ushort address, byte data)
    {
        // do nothing
    }
    
    private void LoadData(byte[] data, int startAddress)
    {
        Array.Copy(data, 0, _storage, startAddress, data.Length);
    }
}
namespace CPU.MOS6502;

public class Bus
{
    private byte _latchedData;

    private readonly DeviceConnection[] _addressSpace = new DeviceConnection[0xFFFF + 1];

    private readonly struct DeviceConnection
    {
        public ushort Mask { get; init; }
        public IDevice Device { get; init; }
        public bool IsOpen => Device == null;
    }

    public record AddressRange(ushort Start, ushort End);

    public void Connect(IDevice device, AddressRange range, ushort mask = 0xFFFF)
    {
        var connection = new DeviceConnection { Mask = mask, Device = device };
        for (int i = range.Start; i <= range.End; i++)
            _addressSpace[i] = connection;
    }

    public void Connect(IDevice device, AddressRange[] ranges, ushort mask = 0xFFFF)
    {
        foreach (AddressRange range in ranges)
            Connect(device, range, mask);
    }

    public byte Read(ushort address)
    {
        var connection = _addressSpace[address];

        if (connection.IsOpen)
            return _latchedData;

        var offset = (ushort)(address & connection.Mask);
        return _latchedData = connection.Device.Read(offset);
    }

    public void Write(ushort address, byte data)
    {
        var connection = _addressSpace[address];
        _latchedData = data;

        if (connection.IsOpen)
            return;

        var offset = (ushort)(address & connection.Mask);
        connection.Device.Write(offset, data);
    }
}
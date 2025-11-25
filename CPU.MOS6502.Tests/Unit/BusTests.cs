using CPU.MOS6502.Tests.Utils;

namespace CPU.MOS6502.Tests.Unit;

/*

Devices on the bus:
- DEVICE A: 32K storage (in fact 48K, but DEVICE #2 address space overlap renders 16K unused)
- DEVICE B: 16K storage (address offset with mask)
- DEVICE C: 8K storage (in fact 4K, but mirrored with mask)

Memory map of the connected devices:

+-----------------+ 0x0000
|                 |
|    DEVICE A     |
|                 | 0x3FFF
+-----------------+ 0x4000
|                 |
|    DEVICE B     |
|                 | 0x7FFF
+-----------------+ 0x8000
|                 |
|    DEVICE A     |
|                 | 0xBFFF
+-----------------+ 0xC000
|                 |
|    DEVICE C     |
|                 | 0xDFFF
+-----------------+ 0xE000
|                 |
|  NOT CONNECTED  |
|                 | 0xFFFF
+-----------------+ 

*/
[Trait("Category", "Unit")]
public class BusTests : IDisposable
{
    static readonly Bus _bus = new();

    private static readonly IDevice deviceA = new SimpleDevice(0xC000); // DEVICE A
    private static readonly IDevice deviceB = new SimpleDevice(0x4000); // DEVICE B
    private static readonly IDevice deviceC = new SimpleDevice(0x1000); // DEVICE C

    public BusTests()
    {
        _bus.Connect(deviceA, [new Bus.AddressRange(0x0000, 0x3FFF), new Bus.AddressRange(0x8000, 0xBFFF)]);
        _bus.Connect(deviceB, new Bus.AddressRange(0x4000, 0x7FFF), 0x3FFF);
        _bus.Connect(deviceC, new Bus.AddressRange(0xC000, 0xDFFF), 0x0FFF);
    }

    public void Dispose()
    {
        deviceA.Reset();
        deviceB.Reset();
        deviceC.Reset();
    }

    public static IEnumerable<object[]> ReadWriteTestData()
    {
        return
        [
            [0x0000, deviceA, 0x0000],
            [0x3FFF, deviceA, 0x3FFF],
            [0x4000, deviceB, 0x0000],
            [0x7FFF, deviceB, 0x3FFF],
            [0x8000, deviceA, 0x8000],
            [0xBFFF, deviceA, 0xBFFF],
            [0xC000, deviceC, 0x0000],
            [0xD000, deviceC, 0x0000],
            [0xCFFF, deviceC, 0x0FFF],
            [0xDFFF, deviceC, 0x0FFF],
        ];
    }

    [Theory]
    [MemberData(nameof(ReadWriteTestData))]
    public void WhenReadFromAddress_DataComesFromCorrectDevice(ushort address, SimpleDevice device, ushort offset)
    {
        byte data = 0xAB;
        device.Write(offset, data);
        Assert.Equal(data, _bus.Read(address));
    }

    [Theory]
    [MemberData(nameof(ReadWriteTestData))]
    public void WhenWriteToAddress_DataReachesCorrectDevice(ushort address, SimpleDevice device, ushort offset)
    {
        byte data = 0xAB;
        _bus.Write(address, data);
        Assert.Equal(data, device.Read(offset));
    }

    public static IEnumerable<object[]> OpenBusTestData()
    {
        return
        [
            [0x0000, 0xE000],
            [0x0000, 0xFFFF],
        ];
    }

    [Theory]
    [MemberData(nameof(OpenBusTestData))]
    public void WhenReadFromOpenBus_ReturnsLatchedData(ushort validAddress, ushort openAddress)
    {
        byte data = 0xAB;
        _bus.Write(validAddress, data);
        Assert.Equal(_bus.Read(validAddress), _bus.Read(openAddress));
    }

    [Theory]
    [MemberData(nameof(OpenBusTestData))]
    public void WhenWriteToAddress_LatchesData(ushort validAddress, ushort openAddress)
    {
        byte data = 0xAB;
        _bus.Write(validAddress, data);
        Assert.Equal(data, _bus.Read(openAddress));
    }
}
namespace CPU.MOS6502.Tests.Utils;

public class SimpleSystem
{
    private Bus Bus { get; }
    public Core CPU { get; private set; }
    public SimpleDevice RAM { get; }

    public SimpleSystem()
    {
        Bus = new Bus();
        CPU = new Core(Bus);
        RAM = new SimpleDevice(0xFFFF + 1) { Name = "RAM" };
        Bus.Connect(RAM, new Bus.AddressRange(0x0000, 0xFFFF));
    }
}
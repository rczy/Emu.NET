namespace CPU.MOS6502.Tests.Utils;

public class SimpleSystem
{
    public Bus Bus { get; private set; }
    public Core CPU { get; private set; }
    public SimpleDevice RAM { get; private set; }

    public SimpleSystem()
    {
        Bus = new();
        CPU = new(Bus);
        RAM = new(0xFFFF + 1) { Name = "RAM" };
        Bus.Connect(RAM, new Bus.AddressRange(0x0000, 0xFFFF));
    }

    public void Reset()
    {
        RAM.Reset();
        CPU = new(Bus); // until CPU has a reset method
    }
}
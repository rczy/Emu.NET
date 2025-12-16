using CPU.MOS6502.Tests.Utils;

namespace CPU.MOS6502.Tests.Integration;

[Trait("Category", "Integration")]
public class TestRunner
{
    [Fact]
    public void FunctionalTest_Runs_Successfully()
    {
        const string testBinary = @"binaries\6502_functional_test.bin";
        const ushort successPc = 0x3469;

        ushort lastPc = 0x0000;
        var system = new SimpleSystem();
        system.RAM.LoadData(File.ReadAllBytes(testBinary), 0x000A);
        system.CPU.Registers.PC = 0x0400;

        for (;;)
        {
            if (system.CPU.Signals.SYNC)
            {
                if (system.CPU.Registers.PC == lastPc) break; // loop on PC detected (trapped)
                lastPc = system.CPU.Registers.PC;
            }
            system.CPU.Tick();
        }
        Assert.Equal(successPc, system.CPU.Registers.PC);
    }
    
    [Fact]
    public void InterruptTest_Runs_Successfully()
    {
        const string testBinary = @"binaries\6502_interrupt_test.bin";
        const ushort successPc = 0x06F5;
        const ushort feedbackAddress = 0xBFFC;
        const byte irqBit = 0b01;
        const byte nmiBit = 0b10;

        ushort lastPc = 0x0000;
        var system = new SimpleSystem();
        system.RAM.LoadData(File.ReadAllBytes(testBinary), 0x000A);
        system.CPU.Registers.PC = 0x0400;
        system.CPU.Bus.Write(feedbackAddress, 0x00);

        for (;;)
        {
            system.CPU.Signals.IRQ = (system.RAM.PeekAt(feedbackAddress) & irqBit) != 0;
            system.CPU.Signals.NMI = (system.RAM.PeekAt(feedbackAddress) & nmiBit) != 0;
            
            if (system.CPU.Signals.SYNC)
            {
                if (system.CPU.Registers.PC == lastPc) break; // loop on PC detected (trapped)
                lastPc = system.CPU.Registers.PC;
            }
            system.CPU.Tick();
        }
        Assert.Equal(successPc, system.CPU.Registers.PC);
    }
    
    [Fact]
    public void DecimalTest_Runs_Successfully()
    {
        const string testBinary = @"binaries\6502_decimal_test.bin";
        const ushort doneAddress = 0x024B;
        const ushort errorAddress = 0x000B;

        var system = new SimpleSystem();
        system.RAM.LoadData(File.ReadAllBytes(testBinary), 0x0200);
        system.CPU.Registers.PC = 0x0200;

        for (;;)
        {
            if (system.CPU.Signals.SYNC && system.CPU.Registers.PC == doneAddress) break;
            system.CPU.Tick();
        }
        Assert.Equal(0, system.RAM.PeekAt(errorAddress));
    }
}
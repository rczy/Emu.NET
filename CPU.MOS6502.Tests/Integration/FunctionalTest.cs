using CPU.MOS6502.Tests.Utils;

namespace CPU.MOS6502.Tests.Integration;

public class FunctionalTest
{
    [Fact]
    public void FunctionalTest_Runs_Successfully()
    {
        const string testBinary = @"Binaries\6502_functional_test.bin";
        const ushort successPc = 0x3469;

        var system = new SimpleSystem();
        system.RAM.LoadData(File.ReadAllBytes(testBinary));
        system.CPU.Registers.PC = 0x0400;

        ushort lastPc = 0x0000;

        for (;;)
        {
            if (system.CPU.Signals.SYNC)
            {
                if (system.CPU.Registers.PC == lastPc) break; // loop on PC detected (trap hit)
                lastPc = system.CPU.Registers.PC;
            }
            system.CPU.Tick();
        }
        Assert.Equal(successPc, system.CPU.Registers.PC);
    }
}
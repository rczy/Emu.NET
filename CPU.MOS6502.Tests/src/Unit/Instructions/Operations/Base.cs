namespace CPU.MOS6502.Tests.Unit.Instructions.Operations;

using Utils;

public class Base
{
    protected readonly SimpleSystem system = new();

    protected void CheckFlags(bool negative, bool zero, bool carry, bool interrupt, bool @decimal, bool overflow)
    {
        Assert.Equal(negative, system.CPU.Registers.P.Negative);
        Assert.Equal(zero, system.CPU.Registers.P.Zero);
        Assert.Equal(carry, system.CPU.Registers.P.Carry);
        Assert.Equal(interrupt, system.CPU.Registers.P.Interrupt);
        Assert.Equal(@decimal, system.CPU.Registers.P.Decimal);
        Assert.Equal(overflow, system.CPU.Registers.P.Overflow);
    }
}
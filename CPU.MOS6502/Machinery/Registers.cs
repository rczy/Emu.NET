namespace CPU.MOS6502.Machinery;

public class Registers
{
    public ushort PC { get; internal set; } // program counter
    public byte A { get; internal set; } // accumulator
    public byte X { get; internal set; } // X register
    public byte Y { get; internal set; } // Y register
    public byte SP { get; internal set; } // stack pointer
    private StatusFlags P; // processor status register

    [Flags]
    public enum StatusFlags : byte
    {
        Negative = 1 << 7,
        Overflow = 1 << 6,
        Unused = 1 << 5,
        Break = 1 << 4,
        Decimal = 1 << 3,
        Interrupt = 1 << 2,
        Zero = 1 << 1,
        Carry = 1 << 0
    }

    public bool GetFlag(StatusFlags flag)
    {
        return P.HasFlag(flag);
    }

    internal void SetFlag(StatusFlags flag, bool set)
    {
        if (set)
            P |= flag;
        else
            P &= ~flag;
    }
}
namespace CPU.MOS6502.Machinery;

public class Registers
{
    public ushort PC { get; internal set; } // program counter
    public byte A { get; internal set; } // accumulator
    public byte X { get; internal set; } // X register
    public byte Y { get; internal set; } // Y register
    public byte SP { get; internal set; } // stack pointer
    public Flags P { get; internal set; } = new(); // processor status register
}
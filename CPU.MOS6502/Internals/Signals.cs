namespace CPU.MOS6502.Internals;

public class Signals
{
    public bool RDY { get; set; } // ready signal
    public bool SYNC { get; internal set; } = true; // opcode fetch signal
}
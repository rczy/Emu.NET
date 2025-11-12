namespace CPU.MOS6502.Machinery;

public class Signals
{
    public bool RDY { get; set; } // ready
    public bool SYNC { get; internal set; } = true; // opcode fetch
    public bool RES { get; set; } // reset
    public bool IRQ { get; set; } // maskable interrupt
    public bool NMI { get; set; } // non-maskable interrupt
}
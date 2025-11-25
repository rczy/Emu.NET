namespace CPU.MOS6502.Machinery;

public class Signals
{
    public bool RDY { get; set; } = true; // ready
    public bool RW { get; internal set; } = true; // read / (not) write in the next cycle
    public bool SYNC { get; internal set; } = true; // opcode fetch
    public bool RES { get; set; } // reset
    public bool IRQ { get; set; } // maskable interrupt
    public bool NMI { get; set; } // non-maskable interrupt
}
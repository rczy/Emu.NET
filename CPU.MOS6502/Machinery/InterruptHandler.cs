namespace CPU.MOS6502.Machinery;

using Flags = Registers.StatusFlags;

public class InterruptHandler(Core cpu)
{
    private Core Cpu { get; } = cpu;
    private Signals LastState { get; set; } = new();
    public bool PendingNMI { get; internal set; }
    
    public enum Interrupts { None, RES, IRQ, NMI }
    public Interrupts InterruptSequence { get; private set; }

    public bool Poll()
    {
        PendingNMI = !LastState.NMI && Cpu.Signals.NMI;
        LastState = Cpu.Signals;

        if (Cpu.Signals.RES)
        {
            InterruptSequence = Interrupts.RES;
            return true;
        }
        if (PendingNMI)
        {
            InterruptSequence = Interrupts.NMI;
            return true;
        }
        if (Cpu.Signals.IRQ && !Cpu.Registers.GetFlag(Flags.Interrupt))
        {
            InterruptSequence = Interrupts.IRQ;
            return true;
        }

        InterruptSequence = Interrupts.None;
        return false;
    }
}
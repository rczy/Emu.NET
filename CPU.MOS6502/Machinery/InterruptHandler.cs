namespace CPU.MOS6502.Machinery;

public class InterruptHandler(Core cpu)
{
    private Core Cpu { get; } = cpu;
    private Signals LastState { get; set; } = new();
    public bool PendingNMI { get; internal set; }
    
    public enum Interrupts { None, RES, IRQ, NMI }
    public Interrupts Sequence { get; private set; }

    public void DetectNMI()
    {
        if (!PendingNMI)
        {
            PendingNMI = !LastState.NMI && Cpu.Signals.NMI;            
        }
        LastState = Cpu.Signals;
    }

    public bool Poll()
    {
        if (Cpu.Signals.RES)
        {
            Sequence = Interrupts.RES;
            return true;
        }
        if (PendingNMI)
        {
            Sequence = Interrupts.NMI;
            return true;
        }
        if (Cpu.Signals.IRQ && !Cpu.Registers.P.Interrupt)
        {
            Sequence = Interrupts.IRQ;
            return true;
        }

        Sequence = Interrupts.None;
        return false;
    }
}
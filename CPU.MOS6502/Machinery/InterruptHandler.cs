namespace CPU.MOS6502.Machinery;

public class InterruptHandler(Core cpu)
{
    private Core Cpu { get; } = cpu;
    private bool LastNMI { get; set; }
    public bool PendingNMI { get; internal set; }
    
    public enum Interrupts { None, RES, IRQ, NMI }
    public Interrupts Sequence { get; private set; }

    public void DetectNMI()
    {
        if (!PendingNMI)
        {
            PendingNMI = !LastNMI && Cpu.Signals.NMI;            
        }
        LastNMI = Cpu.Signals.NMI;
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
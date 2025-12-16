namespace CPU.MOS6502.Tests.Unit;

using Utils;
using Operations = Machinery.Instructions.FlowAndStack.Operations;
using Execution = Machinery.Instructions.FlowAndStack.Execution;
using Sequence = Machinery.InterruptHandler.Interrupts;

[Trait("Category", "Unit")]
public class InterruptTests
{
    private readonly SimpleSystem _system = new();

    public InterruptTests()
    {
        _system.CPU.Decoder.AddInstruction(0x00, Operations.BRK, Execution.Break);
        _system.CPU.Decoder.AddInstruction(0xFF, Operations.BRK, Execution.Break); // fake BRK
        _system.RAM.LoadData([0xFF]); // load fake BRK
    }

    private void Tick(int cycles = 1)
    {
        for (int i = 0; i < cycles; i++)
            _system.CPU.Tick();
    }
    
    [Theory]
    [InlineData(true, true, true, Sequence.RES, 0x00, 0x0000)] // reset (1)
    [InlineData(false, true, true, Sequence.NMI, 0x00, 0x0000)] // non-maskable interrupt (2)
    [InlineData(false, false, true, Sequence.IRQ, 0x00, 0x0000)] // interrupt request (3)
    [InlineData(false, false, false, Sequence.None, 0xFF, 0x0001)] // software interrupt (4)
    public void Interrupt_TriggeredWith_CorrectPriority(bool res, bool nmi, bool irq, Sequence seq, byte op, ushort pc)
    {
        _system.CPU.Signals.RES = res;
        _system.CPU.Signals.NMI = nmi;
        _system.CPU.Signals.IRQ = irq;
        Tick();
        
        Assert.Equal(seq, _system.CPU.InterruptHandler.Sequence);
        Assert.Equal(op, _system.CPU.Decoder.OpCode);
        Assert.Equal(pc, _system.CPU.Registers.PC);
    }

    [Theory]
    [InlineData(true, false, false, true, false)] // reset
    [InlineData(false, true, false, true, false)] // non-maskable interrupt
    [InlineData(false, false, true, true, false)] // interrupt request
    [InlineData(false, false, false, true, true)] // software interrupt
    public void HardwareInterrupt_SetsFlagsCorrectly(bool res, bool nmi, bool irq, bool interruptFlag, bool breakFlag)
    {
        _system.CPU.Signals.RES = res;
        _system.CPU.Signals.NMI = nmi;
        _system.CPU.Signals.IRQ = irq;
        Tick(7); // end of interrupt handling sequence

        Assert.Equal(interruptFlag, _system.CPU.Registers.P.Interrupt);
        Assert.Equal(breakFlag, _system.CPU.Registers.P.Break);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, true, false)]
    public void NonMaskableInterrupt_EdgeDetected(bool oldState, bool newState, bool latched)
    {
        Tick(); // start software interrupt sequence
        _system.CPU.Signals.NMI = oldState;
        Tick();
        _system.CPU.InterruptHandler.PendingNMI = false; // clear latch to detect change
        Tick();
        _system.CPU.Signals.NMI = newState;
        Tick(5); // next instruction cycle

        Assert.Equal(latched, _system.CPU.InterruptHandler.PendingNMI);
        Assert.Equal(latched, Sequence.NMI == _system.CPU.InterruptHandler.Sequence); 
    }

    [Fact]
    public void NonMaskableInterrupt_LatchCleared_AfterSequence()
    {
        _system.CPU.Signals.NMI = true;
        Tick();
        Assert.True(_system.CPU.InterruptHandler.PendingNMI); // NMI detected
        Assert.Equal(Sequence.NMI, _system.CPU.InterruptHandler.Sequence); // NMI sequence started
        Tick(7); // next instruction cycle
        Assert.False(_system.CPU.InterruptHandler.PendingNMI); // latch cleared
        Assert.Equal(Sequence.None, _system.CPU.InterruptHandler.Sequence); // no new NMI detected
    }

    [Theory]
    [InlineData(true, false, false, Sequence.None)]
    [InlineData(true, false, true, Sequence.RES)]
    [InlineData(false, true, false, Sequence.None)]
    [InlineData(false, true, true, Sequence.IRQ)]
    public void Reset_And_InterruptRequest_LevelDetected(bool res, bool irq, bool hold, Sequence seq)
    {
        Tick(); // start software interrupt sequence
        _system.CPU.Signals.RES = res;
        _system.CPU.Signals.IRQ = irq;
        Tick(6); // last cycle of the current instruction
        _system.CPU.Signals.RES &= hold;
        _system.CPU.Signals.IRQ &= hold;
        _system.CPU.Registers.P.Interrupt = false;
        Tick(); // next instruction cycle
        Assert.Equal(seq, _system.CPU.InterruptHandler.Sequence);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void InterruptRequest_CanBe_Masked(bool masked, bool detected)
    {
        _system.CPU.Signals.IRQ = true;
        _system.CPU.Registers.P.Interrupt = masked;
        Tick();
        Assert.Equal(detected, _system.CPU.InterruptHandler.Sequence == Sequence.IRQ);
    }
}
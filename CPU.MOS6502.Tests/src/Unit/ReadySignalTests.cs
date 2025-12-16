namespace CPU.MOS6502.Tests.Unit;

using Utils;
using CPU.MOS6502.Machinery.Instructions;

using FST = Machinery.Instructions.FlowAndStack;
using RMW = Machinery.Instructions.ReadModifyWrite;
using SBT = Machinery.Instructions.SingleByte;
using STR = Machinery.Instructions.Store;

[Trait("Category", "Unit")]
public class ReadySignalTests
{
    private readonly StallChecker _stallChecker = new();

    [Theory]
    [InlineData(1, 2, true)]
    [InlineData(2, 3, false)]
    [InlineData(2, 4, true)]
    [InlineData(3, 4, true)]
    public void WhenPushToStack_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(FST.Execution.Push, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(1, 2, true)]
    [InlineData(2, 3, true)]
    [InlineData(3, 4, false)]
    [InlineData(3, 5, false)]
    [InlineData(3, 6, true)]
    [InlineData(4, 5, false)]
    [InlineData(4, 6, true)]
    [InlineData(5, 6, true)]
    public void WhenJumpToSubroutine_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(FST.Execution.JumpToSubroutine, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(1, 2, true)]
    [InlineData(2, 3, false)]
    [InlineData(2, 4, false)]
    [InlineData(2, 5, false)]
    [InlineData(2, 6, true)]
    [InlineData(3, 4, false)]
    [InlineData(3, 5, false)]
    [InlineData(3, 6, true)]
    [InlineData(4, 5, false)]
    [InlineData(4, 6, true)]
    public void WhenBreak_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(FST.Execution.Break, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }

    [Theory]
    [InlineData(2, 3, true)]
    [InlineData(3, 4, false)]
    [InlineData(3, 5, false)]
    [InlineData(3, 6, true)]
    [InlineData(4, 5, false)]
    [InlineData(4, 6, true)]
    [InlineData(5, 6, true)]
    public void WhenReadModifyWrite_InZeroPageAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(RMW.Execution.ZeroPage, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(3, 4, true)]
    [InlineData(4, 5, false)]
    [InlineData(4, 6, false)]
    [InlineData(4, 7, true)]
    [InlineData(5, 6, false)]
    [InlineData(5, 7, true)]
    [InlineData(6, 7, true)]
    public void WhenReadModifyWrite_InAbsoluteAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(RMW.Execution.Absolute, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(3, 4, true)]
    [InlineData(4, 5, false)]
    [InlineData(4, 6, false)]
    [InlineData(4, 7, true)]
    [InlineData(5, 6, false)]
    [InlineData(5, 7, true)]
    [InlineData(6, 7, true)]
    public void WhenReadModifyWrite_InZeroPageXAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(RMW.Execution.ZeroPageX, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(4, 5, true)]
    [InlineData(5, 6, false)]
    [InlineData(5, 7, false)]
    [InlineData(5, 8, true)]
    [InlineData(6, 7, false)]
    [InlineData(6, 8, true)]
    [InlineData(7, 8, true)]
    public void WhenReadModifyWrite_InAbsoluteXAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(RMW.Execution.AbsoluteX, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(1, 2, true)]
    [InlineData(2, 3, false)]
    [InlineData(2, 4, true)]
    [InlineData(3, 4, true)]
    public void WhenStore_InZeroPageAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.ZeroPage, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(2, 3, true)]
    [InlineData(3, 4, false)]
    [InlineData(3, 5, true)]
    [InlineData(4, 5, true)]
    public void WhenStore_InAbsoluteAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.Absolute, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(4, 5, true)]
    [InlineData(5, 6, false)]
    [InlineData(5, 7, true)]
    [InlineData(6, 7, true)]
    public void WhenStore_InIndirectXAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.IndirectX, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(3, 4, true)]
    [InlineData(4, 5, false)]
    [InlineData(4, 6, true)]
    [InlineData(5, 6, true)]
    public void WhenStore_InAbsoluteXAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.AbsoluteX, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(3, 4, true)]
    [InlineData(4, 5, false)]
    [InlineData(4, 6, true)]
    [InlineData(5, 6, true)]
    public void WhenStore_InAbsoluteYAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.AbsoluteY, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(2, 3, true)]
    [InlineData(3, 4, false)]
    [InlineData(3, 5, true)]
    [InlineData(4, 5, true)]
    public void WhenStore_InZeroPageXAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.ZeroPageX, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(2, 3, true)]
    [InlineData(3, 4, false)]
    [InlineData(3, 5, true)]
    [InlineData(4, 5, true)]
    public void WhenStore_InZeroPageYAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.ZeroPageY, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    [Theory]
    [InlineData(4, 5, true)]
    [InlineData(5, 6, false)]
    [InlineData(5, 7, true)]
    [InlineData(6, 7, true)]
    public void WhenStore_InIndirectYAddressing_RdyStallsCpu_Correctly(int rdySetCycle, int stallCycle, bool stalled)
    {
        _stallChecker.SetInstruction(STR.Execution.IndirectY, SBT.Operations.NOP);
        _stallChecker.StallsCorrectly(rdySetCycle, stallCycle, stalled);
    }
    
    private class StallChecker
    {
        private readonly SimpleSystem _system = new();
        private int _lastRwCount;

        private void Tick(int cycles = 1)
        {
            for (var i = 0; i < cycles; i++)
            {
                _lastRwCount = _system.RAM.ReadCount + _system.RAM.WriteCount;
                _system.CPU.Tick();
            }
        }

        private bool IsStalled()
        {
            return _system.RAM.ReadCount + _system.RAM.WriteCount == _lastRwCount && _system.CPU.Stalled;
        }

        public void SetInstruction(Steps step, Operation op)
        {
            _system.CPU.Decoder.AddInstruction(0x00, op, step);
        }

        public void StallsCorrectly(int rdySetCycle, int stallCycle, bool predicted)
        {
            Tick(rdySetCycle);
            Assert.False(IsStalled());
            _system.CPU.Signals.RDY = false;

            Tick(stallCycle - rdySetCycle);
            Assert.Equal(predicted, IsStalled());
        }
    }
}
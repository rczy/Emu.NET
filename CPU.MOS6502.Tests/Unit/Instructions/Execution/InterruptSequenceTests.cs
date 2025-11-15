namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

using Machinery;
using CPU.MOS6502.Machinery.Instructions.FlowAndStack;

public class InterruptSequenceTests : Base
{
    public class Software : SequenceTestBase
    {
        protected override ushort VectorLow => 0xFFFE;
        protected override ushort VectorHigh => 0xFFFF;
        protected override InterruptHandler.Interrupts Sequence => InterruptHandler.Interrupts.None;

        protected override void TriggerInterrupt()
        {
        }
    }
    
    public class Reset : SequenceTestBase
    {
        protected override ushort VectorLow => 0xFFFC;
        protected override ushort VectorHigh => 0xFFFD;
        protected override InterruptHandler.Interrupts Sequence => InterruptHandler.Interrupts.RES;
        protected override bool RW => false; // only read occurs in the whole sequence (stack push is disabled)

        protected override void TriggerInterrupt()
        {
            system.CPU.Signals.RES = true;
        }
    }
    
    public class InterruptRequest : SequenceTestBase
    {
        protected override ushort VectorLow => 0xFFFE;
        protected override ushort VectorHigh => 0xFFFF;
        protected override InterruptHandler.Interrupts Sequence => InterruptHandler.Interrupts.IRQ;

        protected override void TriggerInterrupt()
        {
            system.CPU.Signals.IRQ = true;
        }
    }
    
    public class NonMaskableInterrupt : SequenceTestBase
    {
        protected override ushort VectorLow => 0xFFFA;
        protected override ushort VectorHigh => 0xFFFB;
        protected override InterruptHandler.Interrupts Sequence => InterruptHandler.Interrupts.NMI;

        protected override void TriggerInterrupt()
        {
            system.CPU.Signals.NMI = true;
        }
    }
    
    public abstract class SequenceTestBase : Base
    {
        protected abstract ushort VectorLow { get; }
        protected abstract ushort VectorHigh { get; }
        protected abstract InterruptHandler.Interrupts Sequence { get; }

        protected ushort Pc => (ushort)(Sequence == InterruptHandler.Interrupts.None ? 0x0102 : 0x0101);
        protected virtual bool RW => true; // true: read and write occurs, false: just read occurs

        public SequenceTestBase()
        {
            system.CPU.Registers.PC = 0x0101;
            system.CPU.Registers.SP = 0xFF;
            opCode = 0xAB; // dummy opcode for BRK (same as 0x00)
            adh = 0xCD; // interrupt vector high
            adl = 0xEF; // interrupt vector low
            AddInstruction(0x00, Operations.BRK, Execution.Break);
            AddInstruction(opCode, Operations.BRK, Execution.Break);
            
            program = new byte[0xFFFF + 1];
            program[0x0101] = opCode;
            program[0xCDEF] = opCode;
            program[0xFFFA] = adl;
            program[0xFFFB] = adh;
            program[0xFFFC] = adl;
            program[0xFFFD] = adh;
            program[0xFFFE] = adl;
            program[0xFFFF] = adh;
            LoadData(program);
        }
        
        protected abstract void TriggerInterrupt();

        protected void ClearSignals()
        {
            system.CPU.Signals.RES = false;
            system.CPU.Signals.IRQ = false;
            system.CPU.Signals.NMI = false;
        }
        
        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0x0101);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
            Assert.Equal(InterruptHandler.Interrupts.None, system.CPU.InterruptHandler.Sequence);
        }

        [Fact]
        public void T0_IsCorrect() // fetch BRK opcode (or force BRK)
        {
            TriggerInterrupt();
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: Pc);

            Assert.Equal(Sequence == InterruptHandler.Interrupts.None ? opCode : 0x00, system.CPU.Decoder.OpCode);
            Assert.Equal(Sequence, system.CPU.InterruptHandler.Sequence);
        }

        [Fact]
        public void T1_IsCorrect() // dummy read
        {
            TriggerInterrupt();
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: Pc);

            Assert.Equal(Pc, system.RAM.LastReadAddress);
        }
        
        [Fact]
        public void T2_IsCorrect() // push high order byte of program counter to stack
        {
            TriggerInterrupt();
            Tick(3);
            CheckSystem(readCount: (byte)(RW ? 2 : 3), writeCount: (byte)(RW ? 1 : 0), cycles: 3, pc: Pc);

            if (RW)
            {
                Assert.Equal(0x01FF, system.RAM.LastWriteAddress);
                Assert.Equal((byte)(Pc >>> 8), system.RAM.PeekAt(0x01FF));
            }
            else
            {
                Assert.Equal(0x01FF, system.RAM.LastReadAddress);
            }
            Assert.Equal(0xFE, system.CPU.Registers.SP);
        }
        
        [Fact]
        public void T3_IsCorrect() // push low order byte of program counter to stack
        {
            TriggerInterrupt();
            Tick(4);
            CheckSystem(readCount: (byte)(RW ? 2 : 4), writeCount: (byte)(RW ? 2 : 0), cycles: 4, pc: Pc);

            Assert.False(system.CPU.Registers.P.Break);
            if (RW)
            {
                Assert.Equal(0x01FE, system.RAM.LastWriteAddress);
                Assert.Equal((byte)Pc, system.RAM.PeekAt(0x01FE));
            }
            else
            {
                Assert.Equal(0x01FE, system.RAM.LastReadAddress);
            }
            Assert.Equal(0xFD, system.CPU.Registers.SP);
        }
        
        [Fact]
        public void T4_IsCorrect() // push status register to stack
        {
            TriggerInterrupt();
            Flags p = (byte)system.CPU.Registers.P; // copy object via type casting
            p.Break = Sequence == InterruptHandler.Interrupts.None;
            Tick(5);
            CheckSystem(readCount: (byte)(RW ? 2 : 5), writeCount: (byte)(RW ? 3 : 0), cycles: 5, pc: Pc);

            Assert.True(system.CPU.Registers.P.Unused);
            Assert.Equal(Sequence == InterruptHandler.Interrupts.None, system.CPU.Registers.P.Break);
            Assert.False(system.CPU.Registers.P.Interrupt);
            if (RW)
            {
                Assert.Equal(0x01FD, system.RAM.LastWriteAddress);
                Assert.Equal((byte)p, system.RAM.PeekAt(0x01FD));
            }
            else
            {
                Assert.Equal(0x01FD, system.RAM.LastReadAddress);
            }
            Assert.Equal(0xFC, system.CPU.Registers.SP);
        }
        
        [Fact]
        public void T5_IsCorrect() // fetch low order byte of interrupt vector
        {
            TriggerInterrupt();
            Tick(6);
            CheckSystem(readCount: (byte)(RW ? 3 : 6), writeCount: (byte)(RW ? 3 : 0), cycles: 6, pc: Pc);

            Assert.True(system.CPU.Registers.P.Interrupt);
            Assert.Equal(VectorLow, system.RAM.LastReadAddress);
            Assert.Equal(adl, system.CPU.Address.Low);
        }
        
        [Fact]
        public void T6_IsCorrect() // fetch high order byte of interrupt vector
        {
            TriggerInterrupt();
            Tick(7);
            CheckSystem(readCount: (byte)(RW ? 4 : 7), writeCount: (byte)(RW ? 3 : 0), cycles: 0, pc: 0xCDEF);

            Assert.Equal(VectorHigh, system.RAM.LastReadAddress);
            Assert.Equal(adh, system.CPU.Address.High);
        }
        
        [Fact]
        public void After_IsCorrect() // next instruction
        {
            TriggerInterrupt();
            Tick(7);
            ClearSignals();
            Tick(1);
            CheckSystem(readCount: (byte)(RW ? 5 : 8), writeCount: (byte)(RW ? 3 : 0), cycles: 1, pc: 0xCDEF + 1);

            Assert.Equal(0xCDEF, system.RAM.LastReadAddress); // interrupt vector
            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }
    }
}
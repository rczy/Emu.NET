namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

using Operations = Machinery.Instructions.FlowAndStack.Operations;
using ExecSteps = Machinery.Instructions.FlowAndStack.Execution;

public class FlowAndStackTests
{
    public class Push : Base
    {
        public Push() : base()
        {
            system.CPU.Registers.SP = 0xFF;

            opCode = 0xAB;
            AddWriteInstruction(opCode, ExecSteps.Push);
            LoadData([opCode]);
        }

        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 1);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
            Assert.False(opCalled);
        }

        [Fact]
        public void T1_IsCorrect() // fetch next opcode (discarded)
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 1);

            Assert.Equal(system.RAM.LastReadAddress, system.CPU.Registers.PC);
            Assert.False(opCalled);
        }

        [Fact]
        public void T2_IsCorrect() // write register to stack
        {
            Tick(3);
            CheckSystem(readCount: 2, writeCount: 1, cycles: 0, pc: 1);

            Assert.Equal(0x01FF, system.CPU.Address);
            Assert.Equal(0x01FF, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(0x01FF));
            Assert.Equal(0xFE, system.CPU.Registers.SP);
            Assert.True(opCalled);
        }

        [Fact]
        public void T2_StackPointer_WrapsAround() // write register to stack (and wrap around)
        {
            system.CPU.Registers.SP = 0x00;
            Tick(3);
            CheckSystem(readCount: 2, writeCount: 1, cycles: 0, pc: 1);

            Assert.Equal(0x0100, system.CPU.Address);
            Assert.Equal(0x0100, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(0x0100));
            Assert.Equal(0xFF, system.CPU.Registers.SP);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(4);
            CheckSystem(readCount: 3, writeCount: 1, cycles: 1, pc: 2);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class Pull : Base
    {
        public Pull() : base()
        {
            system.CPU.Registers.SP = 0xFE;

            opCode = 0xAB;
            data = 0xCD;
            AddDummyInstruction(opCode, ExecSteps.Pull);

            program = new byte[0x200];
            program[0] = opCode;
            program[0x01FF] = data;
            LoadData(program);
        }

        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 1);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
            Assert.False(opCalled);
        }

        [Fact]
        public void T1_IsCorrect() // fetch next opcode (discarded)
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 1);

            Assert.Equal(system.RAM.LastReadAddress, system.CPU.Registers.PC);
        }

        [Fact]
        public void T2_IsCorrect() // fetch data from stack (discarded)
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 1);

            Assert.Equal(0x01FE, system.RAM.LastReadAddress);
            Assert.False(opCalled);
        }

        [Fact]
        public void T3_IsCorrect() // fetch data from stack
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 0, pc: 1);

            Assert.Equal(0x01FF, system.CPU.Address);
            Assert.Equal(0x01FF, system.RAM.LastReadAddress);
            Assert.Equal(data, system.CPU.Data);
            Assert.Equal(0xFF, system.CPU.Registers.SP);
            Assert.True(opCalled);
        }

        [Fact]
        public void T3_StackPointer_WrapsAround() // fetch data from stack (and wrap around)
        {
            system.CPU.Registers.SP = 0xFF;
            program[0x0100] = data;
            LoadData(program);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 0, pc: 1);

            Assert.Equal(0x0100, system.CPU.Address);
            Assert.Equal(0x0100, system.RAM.LastReadAddress);
            Assert.Equal(data, system.CPU.Data);
            Assert.Equal(0x00, system.CPU.Registers.SP);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 1, pc: 2);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class JumpToSubroutine : Base
    {
        public JumpToSubroutine() : base()
        {
            system.CPU.Registers.SP = 0xFF;
            system.CPU.Registers.PC = 0x0100;

            opCode = 0xAB;
            adh = 0x02;
            adl = 0xCD;
            AddInstruction(opCode, Operations.JSR, ExecSteps.JumpToSubroutine);

            program = new byte[0x300];
            program[0x0100] = opCode;
            program[0x0101] = adl;
            program[0x0102] = adh;
            LoadData(program);
        }

        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0x0100);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 0x0101);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T1_IsCorrect() // fetch low order byte of subroutine address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 0x0102);

            Assert.Equal(adl, system.CPU.Address.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch data from stack (discarded)
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 0x0102);

            Assert.Equal(0x01FF, system.RAM.LastReadAddress);
        }

        [Fact]
        public void T3_IsCorrect() // push high order byte of program counter to stack
        {
            Tick(4);
            CheckSystem(readCount: 3, writeCount: 1, cycles: 4, pc: 0x0102);

            Assert.Equal(0x01FF, system.RAM.LastWriteAddress);
            Assert.Equal(0x01, system.RAM.PeekAt(0x01FF));
            Assert.Equal(0xFE, system.CPU.Registers.SP);
        }

        [Fact]
        public void T4_IsCorrect() // push low order byte of program counter to stack
        {
            Tick(5);
            CheckSystem(readCount: 3, writeCount: 2, cycles: 5, pc: 0x0102);

            Assert.Equal(0x01FE, system.RAM.LastWriteAddress);
            Assert.Equal(0x02, system.RAM.PeekAt(0x01FE));
            Assert.Equal(0xFD, system.CPU.Registers.SP);
            Assert.NotEqual(adh, system.CPU.Address.High);
        }

        [Fact]
        public void T5_IsCorrect() // fetch high order byte of subroutine address
        {
            Tick(6);
            CheckSystem(readCount: 4, writeCount: 2, cycles: 0, pc: (ushort)((adh << 8) | adl));

            Assert.Equal(adh, system.CPU.Address.High);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(7);
            CheckSystem(readCount: 5, writeCount: 2, cycles: 1, pc: (ushort)(((adh << 8) | adl) + 1));

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class ReturnFromSubroutine : Base
    {
        public ReturnFromSubroutine() : base()
        {
            system.CPU.Registers.SP = 0xFD;
            system.CPU.Registers.PC = 0x02CD;

            opCode = 0xAB;
            adh = 0x01; // PC high
            adl = 0x02; // PC low
            AddInstruction(opCode, Operations.RTS, ExecSteps.ReturnFromSubroutine);

            program = new byte[0x300];
            program[0x02CD] = opCode;
            program[0x01FE] = adl;
            program[0x01FF] = adh;
            LoadData(program);
        }

        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0x02CD);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 0x02CE);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T1_IsCorrect() // dummy read
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 0x02CE);

            Assert.Equal(system.CPU.Registers.PC, system.RAM.LastReadAddress);
        }

        [Fact]
        public void T2_IsCorrect() // fetch data from stack (discarded)
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 0x02CE);

            Assert.Equal(0x01FD, system.RAM.LastReadAddress);
        }

        [Fact]
        public void T3_IsCorrect() // pull PCL from stack
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 0x0202);

            Assert.Equal(0x01FE, system.RAM.LastReadAddress);
            Assert.Equal(0xFE, system.CPU.Registers.SP);
        }

        [Fact]
        public void T4_IsCorrect() // pull PCH from stack
        {
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 5, pc: 0x0102);

            Assert.Equal(0x01FF, system.RAM.LastReadAddress);
            Assert.Equal(0xFF, system.CPU.Registers.SP);
        }

        [Fact]
        public void T5_IsCorrect() // dummy read and increment PC
        {
            Tick(6);
            CheckSystem(readCount: 6, writeCount: 0, cycles: 0, pc: 0x0103);

            Assert.Equal(0x0102, system.RAM.LastReadAddress);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(7);
            CheckSystem(readCount: 7, writeCount: 0, cycles: 1, pc: 0x0104);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class JumpAbsolute : Base
    {
        public JumpAbsolute() : base()
        {
            opCode = 0xAB;
            adh = 0x01;
            adl = 0x02;
            AddInstruction(opCode, Operations.JMP, ExecSteps.JumpAbsolute);

            program = new byte[0x200];
            program[0] = opCode;
            program[1] = adl;
            program[2] = adh;
            LoadData(program);
        }

        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 1);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T1_IsCorrect() // fetch low order byte of jump address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch high order byte of jump address
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 0, pc: 0x0102);

            Assert.Equal(adh, system.CPU.Address.High);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 1, pc: 0x0103);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class JumpImplied : Base
    {
        public JumpImplied() : base()
        {
            opCode = 0xAB;
            iah = 0x01;
            ial = 0x02;
            adh = 0x02;
            adl = 0xCD;
            AddInstruction(opCode, Operations.JMP, ExecSteps.JumpIndirect);

            program = new byte[0x300];
            program[0] = opCode;
            program[1] = ial;
            program[2] = iah;
            program[(iah << 8) | ial] = adl;
            program[((iah << 8) | ial) + 1] = adh;
            LoadData(program);
        }

        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 1);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T1_IsCorrect() // fetch low order byte of indirect address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(ial, system.CPU.IndirectAddress.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch high order byte of indirect address
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 2);

            Assert.Equal(iah, system.CPU.IndirectAddress.High);
        }

        [Fact]
        public void T3_IsCorrect() // fetch low order byte of jump address
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Low);
        }

        [Fact]
        public void T4_IsCorrect() // fetch high order byte of jump address
        {
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 0, pc: 0x02CD);

            Assert.Equal(adh, system.CPU.Address.High);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(6);
            CheckSystem(readCount: 6, writeCount: 0, cycles: 1, pc: 0x02CE);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class Branch : Base
    {
        public Branch() : base()
        {
            opCode = 0xAB;
            program = new byte[0x400];
        }

        protected void ArrangeBranching(ushort initialPc, byte offset, bool takeBranch)
        {
            AddInstruction(opCode, cpu => cpu.Data = (byte)(takeBranch ? 1 : 0), ExecSteps.Branch);
            program[initialPc] = opCode;
            program[initialPc + 1] = offset;
            LoadData(program);

            system.CPU.Registers.PC = initialPc;
        }

        [Fact]
        public void Before_IsCorrect()
        {
            ArrangeBranching(0x0080, 0x00, false);
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0x80);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            ArrangeBranching(0x0080, 0x00, false);
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 0x81);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }

        [Theory]
        [InlineData(false, 0)] // branch is not taken
        [InlineData(true, 2)] // branch is taken
        public void T1_IsCorrect(bool takeBranch, int cycles) // fetch branch offset
        {
            ArrangeBranching(0x0080, 0x00, takeBranch);
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: cycles, pc: 0x82);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }

        [Theory]
        [InlineData(0x0200, 0x40, 0, 0x0242)] // no page crossing, forward
        [InlineData(0x0240, 0xE0, 0, 0x0222)] // no page crossing, backward
        [InlineData(0x02EF, 0x0F, 3, 0x0200)] // with page crossing, forward
        [InlineData(0x0200, 0xFD, 3, 0x02FF)] // with page crossing, backward
        public void T2_IsCorrect(ushort initialPc, byte offset, int cycles, ushort pc) // offset added to program counter
        {
            ArrangeBranching(initialPc, offset, true);
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: cycles, pc: pc);
        }

        [Theory]
        [InlineData(0x02EF, 0x0F, 0x0300)]
        [InlineData(0x0200, 0xFD, 0x01FF)]
        public void T3_IsCorrect(ushort initialPc, byte offset, ushort pc) // page boundary crossed
        {
            ArrangeBranching(initialPc, offset, true);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 0, pc: pc);
        }
    }
    
    public class ReturnFromInterrupt : Base
    {
        public ReturnFromInterrupt() : base()
        {
            system.CPU.Registers.SP = 0xFC;
            system.CPU.Registers.PC = 0x02CD;
            system.CPU.Registers.P = 0b1010_1010;

            opCode = 0xAB;
            data = 0b0101_0101; // P (status) register; Unused and Break flags (bit 4 and 5) are not real registers
            adh = 0x01; // PC high
            adl = 0x02; // PC low
            AddInstruction(opCode, Operations.RTI, ExecSteps.ReturnFromInterrupt);

            program = new byte[0x300];
            program[0x02CD] = opCode;
            program[0x01FD] = data;
            program[0x01FE] = adl;
            program[0x01FF] = adh;
            LoadData(program);
        }

        [Fact]
        public void Before_IsCorrect()
        {
            CheckSystem(readCount: 0, writeCount: 0, cycles: 0, pc: 0x02CD);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T0_IsCorrect() // fetch opcode
        {
            Tick(1);
            CheckSystem(readCount: 1, writeCount: 0, cycles: 1, pc: 0x02CE);

            Assert.Equal(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T1_IsCorrect() // dummy read
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 0x02CE);

            Assert.Equal(system.CPU.Registers.PC, system.RAM.LastReadAddress);
        }

        [Fact]
        public void T2_IsCorrect() // fetch data from stack (discarded)
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 0x02CE);

            Assert.Equal(0x01FC, system.RAM.LastReadAddress);
        }

        [Fact]
        public void T3_IsCorrect() // pull P from stack
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 0x02CE);
            
            Assert.Equal(data & 0xEF | 0x20, (byte)system.CPU.Registers.P); // bit 5 is always set, bit 4 is cleared
            Assert.False(system.CPU.Registers.P.Negative);
            Assert.True(system.CPU.Registers.P.Overflow);
            Assert.True(system.CPU.Registers.P.Unused); // flag is always set
            Assert.False(system.CPU.Registers.P.Break); // flag is cleared
            Assert.False(system.CPU.Registers.P.Decimal);
            Assert.True(system.CPU.Registers.P.Interrupt);
            Assert.False(system.CPU.Registers.P.Zero);
            Assert.True(system.CPU.Registers.P.Carry);

            Assert.Equal(0x01FD, system.RAM.LastReadAddress);
            Assert.Equal(0xFD, system.CPU.Registers.SP);
        }

        [Fact]
        public void T4_IsCorrect() // pull PCL from stack
        {
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 5, pc: 0x0202);

            Assert.Equal(0x01FE, system.RAM.LastReadAddress);
            Assert.Equal(0xFE, system.CPU.Registers.SP);
        }

        [Fact]
        public void T5_IsCorrect() // pull PCH from stack
        {
            Tick(6);
            CheckSystem(readCount: 6, writeCount: 0, cycles: 0, pc: 0x0102);

            Assert.Equal(0x01FF, system.RAM.LastReadAddress);
            Assert.Equal(0xFF, system.CPU.Registers.SP);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(7);
            CheckSystem(readCount: 7, writeCount: 0, cycles: 1, pc: 0x0103);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }
}
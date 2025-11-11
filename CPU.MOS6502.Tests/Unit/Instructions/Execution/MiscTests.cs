namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

using Operations = Machinery.Instructions.Misc.Operations;
using ExecSteps = Machinery.Instructions.Misc.Execution;

public class MiscTests
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
}
using CPU.MOS6502.Internals.Instructions;

namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

public class StoreTests
{
    public class ZeroPage : Base
    {
        public ZeroPage() : base()
        {
            opCode = 0xAB;
            adl = 0x04;
            data = 0xCD;
            AddWriteInstruction(opCode, Internals.Instructions.Store.Execution.ZeroPage);
            LoadData([opCode, adl]);
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
        public void T1_IsCorrect() // fetch zero page effective address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Low);
            Assert.False(opCalled);
        }

        [Fact]
        public void T2_IsCorrect() // write data to memory
        {
            Tick(3);
            CheckSystem(readCount: 2, writeCount: 1, cycles: 0, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Full);
            Assert.Equal(adl, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(adl));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(4);
            CheckSystem(readCount: 3, writeCount: 1, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class Absolute : Base
    {
        public Absolute() : base()
        {
            opCode = 0xAB;
            adl = 0x23;
            adh = 0x01;
            data = 0xCD;
            AddWriteInstruction(opCode, Internals.Instructions.Store.Execution.Absolute);

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
        public void T1_IsCorrect() // fetch low order effective address byte
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch high order effective address byte
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 3);

            Assert.Equal(adh, system.CPU.Address.High);
            Assert.False(opCalled);
        }

        [Fact]
        public void T3_IsCorrect() // write data to memory
        {
            Tick(4);
            CheckSystem(readCount: 3, writeCount: 1, cycles: 0, pc: 3);

            Assert.Equal((adh << 8 ) | adl, system.CPU.Address.Full);
            Assert.Equal(system.CPU.Address.Full, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(system.CPU.Address.Full));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(5);
            CheckSystem(readCount: 4, writeCount: 1, cycles: 1, pc: 4);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class IndirectX : Base
    {
        public IndirectX() : base()
        {
            opCode = 0xAB;
            bal = 0xFF; // wrap around
            adl = 0x23;
            adh = 0x01;
            data = 0xCD;
            AddWriteInstruction(opCode, Internals.Instructions.Store.Execution.IndirectX);

            system.CPU.Registers.X = 4;

            byte[] program = new byte[0x200];
            program[0] = opCode;
            program[1] = bal;
            program[2] = 0x00;
            program[3] = adl;
            program[4] = adh;

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
        public void T1_IsCorrect() // fetch page zero base address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(bal, system.CPU.BaseAddress.Low);
        }

        [Fact]
        public void T2_IsCorrect() // dummy read
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 2);

            Assert.Equal(0, system.CPU.BaseAddress.High);
            Assert.Equal(bal, system.CPU.BaseAddress.Full);
            Assert.Equal(system.RAM.LastReadAddress, system.CPU.BaseAddress.Full);
        }

        [Fact]
        public void T3_IsCorrect() // fetch low order byte of effective address
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 2);

            Assert.Equal(system.RAM.LastReadAddress, (system.CPU.BaseAddress.Full + system.CPU.Registers.X) & 0x00FF);
            Assert.Equal(adl, system.CPU.Address.Low);
        }

        [Fact]
        public void T4_IsCorrect() // fetch high order byte of effective address
        {
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 5, pc: 2);

            Assert.Equal(system.RAM.LastReadAddress, (system.CPU.BaseAddress.Full + system.CPU.Registers.X + 1) & 0x00FF);
            Assert.Equal(adh, system.CPU.Address.High);
            Assert.False(opCalled);
        }

        [Fact]
        public void T5_IsCorrect() // write data to memory
        {
            Tick(6);
            CheckSystem(readCount: 5, writeCount: 1, cycles: 0, pc: 2);

            Assert.Equal((adh << 8) | adl, system.CPU.Address.Full);
            Assert.Equal(system.CPU.Address.Full, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(system.CPU.Address.Full));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(7);
            CheckSystem(readCount: 6, writeCount: 1, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    abstract public class AbsoluteIndexed : Base
    {
        abstract protected byte IndexRegister { get; set; }

        abstract protected Steps Steps { get; }

        public AbsoluteIndexed() : base()
        {
            opCode = 0xAB;
            bal = 0x23;
            bah = 0x01;
            data = 0xCD;
            AddWriteInstruction(opCode, Steps);

            program = new byte[0x300];
            program[0] = opCode;
            program[1] = bal;
            program[2] = bah;

            LoadData(program);
        }

        protected void ArrangePageCrossing(bool shouldCross)
        {
            IndexRegister = (byte)(shouldCross ? 0xFF : 0x42);
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
        public void T1_IsCorrect() // fetch low order byte of base address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(bal, system.CPU.BaseAddress.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch high order byte of base address
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 3);

            Assert.Equal(bah, system.CPU.BaseAddress.High);
            Assert.False(opCalled);
        }

        [Theory]
        [InlineData(true)] // page is crossed
        [InlineData(false)] // page is not crossed
        public void T3_IsCorrect(bool shouldCross) // dummy read
        {
            ArrangePageCrossing(shouldCross);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 3);

            Assert.Equal((bah << 8) | (byte)(bal + IndexRegister), system.RAM.LastReadAddress);
            Assert.False(opCalled);
        }

        [Theory]
        [InlineData(true)] // page is crossed
        [InlineData(false)] // page is not crossed
        public void T4_IsCorrect(bool shouldCross) // write data to memory
        {
            ArrangePageCrossing(shouldCross);
            Tick(5);
            CheckSystem(readCount: 4, writeCount: 1, cycles: 0, pc: 3);

            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.CPU.Address.Full);
            Assert.Equal(system.CPU.Address.Full, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(system.CPU.Address.Full));
            Assert.True(opCalled);
        }

        [Theory]
        [InlineData(true)] // page is crossed
        [InlineData(false)] // page is not crossed
        public void After_IsCorrect(bool shouldCross) // next instruction
        {
            ArrangePageCrossing(shouldCross);
            Tick(6);
            CheckSystem(readCount: 5, writeCount: 1, cycles: 1, pc: 4);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class AbsoluteX : AbsoluteIndexed
    {
        protected override byte IndexRegister
        {
            get => system.CPU.Registers.X;
            set => system.CPU.Registers.X = value;
        }

        protected override Steps Steps => Internals.Instructions.Store.Execution.AbsoluteX;
    }

    public class AbsoluteY : AbsoluteIndexed
    {
        protected override byte IndexRegister
        {
            get => system.CPU.Registers.Y;
            set => system.CPU.Registers.Y = value;
        }

        protected override Steps Steps => Internals.Instructions.Store.Execution.AbsoluteY;
    }

    abstract public class ZeroPageIndexed : Base
    {
        abstract protected byte IndexRegister { get; set; }

        abstract protected Steps Steps { get; }

        public ZeroPageIndexed() : base()
        {
            opCode = 0xAB;
            bal = 0x23;
            data = 0xCD;
            AddWriteInstruction(opCode, Steps);

            program = new byte[0x200];
            program[0] = opCode;
            program[1] = bal;

            LoadData(program);
        }

        protected void ArrangeWrapAround(bool shouldWrap)
        {
            IndexRegister = (byte)(shouldWrap ? 0xFF : 0x42);
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
        public void T1_IsCorrect() // fetch page zero base address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(bal, system.CPU.BaseAddress.Low);
        }

        [Fact]
        public void T2_IsCorrect() // dummy read
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 2);

            Assert.Equal(bal, system.RAM.LastReadAddress);
            Assert.False(opCalled);
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T3_IsCorrect(bool shouldWrap) // write data to memory
        {
            ArrangeWrapAround(shouldWrap);
            Tick(4);
            CheckSystem(readCount: 3, writeCount: 1, cycles: 0, pc: 2);

            Assert.Equal((byte)(bal + IndexRegister), system.CPU.Address.Full);
            Assert.Equal(system.CPU.Address.Full, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(system.CPU.Address.Full));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(5);
            CheckSystem(readCount: 4, writeCount: 1, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class ZeroPageX : ZeroPageIndexed
    {
        protected override byte IndexRegister
        {
            get => system.CPU.Registers.X;
            set => system.CPU.Registers.X = value;
        }

        protected override Steps Steps => Internals.Instructions.Store.Execution.ZeroPageX;
    }

    public class ZeroPageY : ZeroPageIndexed
    {
        protected override byte IndexRegister
        {
            get => system.CPU.Registers.Y;
            set => system.CPU.Registers.Y = value;
        }

        protected override Steps Steps => Internals.Instructions.Store.Execution.ZeroPageY;
    }

    public class IndirectY : Base
    {
        public IndirectY() : base()
        {
            opCode = 0xAB;
            ial = 0x42;
            bal = 0x23;
            bah = 0x01;
            data = 0xCD;
            AddWriteInstruction(opCode, Internals.Instructions.Store.Execution.IndirectY);

            program = new byte[0x300];
            program[0] = opCode;
            program[1] = ial;
            program[ial] = bal;
            program[ial + 1] = bah;

            LoadData(program);
        }

        protected void ArrangePageCrossing(bool shouldCross)
        {
            system.CPU.Registers.Y = (byte)(shouldCross ? 0xFF : 0x42);
            program[((bah << 8) | bal) + system.CPU.Registers.Y] = data;
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
        public void T1_IsCorrect() // fetch page zero indirect address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(ial, system.CPU.IndirectAddress.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch low order byte of base address
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 2);

            Assert.Equal(0x00, system.CPU.IndirectAddress.High);
            Assert.Equal(bal, system.CPU.BaseAddress.Low);
        }

        [Fact]
        public void T3_IsCorrect() // fetch high order byte of base address
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 2);

            Assert.Equal(bah, system.CPU.BaseAddress.High);
        }

        [Theory]
        [InlineData(true)] // page is crossed
        [InlineData(false)] // page is not crossed
        public void T4_IsCorrect(bool shouldCross) // dummy read
        {
            ArrangePageCrossing(shouldCross);
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 5, pc: 2);

            Assert.Equal((bah << 8) | (byte)(bal + system.CPU.Registers.Y), system.RAM.LastReadAddress);
            Assert.False(opCalled);
        }

        [Theory]
        [InlineData(true)] // page is crossed
        [InlineData(false)] // page is not crossed
        public void T5_IsCorrect(bool shouldCross) // write data to memory
        {
            ArrangePageCrossing(shouldCross);
            Tick(6);
            CheckSystem(readCount: 5, writeCount: 1, cycles: 0, pc: 2);

            Assert.Equal(((bah << 8) | bal) + system.CPU.Registers.Y, system.CPU.Address.Full);
            Assert.Equal(system.CPU.Address.Full, system.RAM.LastWriteAddress);
            Assert.Equal(data, system.RAM.PeekAt(system.CPU.Address.Full));
            Assert.True(opCalled);
        }

        [Theory]
        [InlineData(true)] // page is crossed
        [InlineData(false)] // page is not crossed
        public void After_IsCorrect(bool shouldCross) // next instruction
        {
            ArrangePageCrossing(shouldCross);
            Tick(7);
            CheckSystem(readCount: 6, writeCount: 1, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }
}
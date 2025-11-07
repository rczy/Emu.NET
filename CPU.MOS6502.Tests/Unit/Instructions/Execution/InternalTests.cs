using CPU.MOS6502.Internals.Instructions;

namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

public class InternalTests
{
    public class Immediate : Base
    {
        public Immediate() : base()
        {
            opCode = 0xAB;
            data = 0xCD;
            AddDummyInstruction(opCode, Internals.Instructions.Internal.Execution.Immediate);
            LoadData([opCode, data]);
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
        public void T1_IsCorrect() // fetch data and execute operation
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 0, pc: 2);

            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class ZeroPage : Base
    {
        public ZeroPage() : base()
        {
            opCode = 0xAB;
            adl = 0x04;
            data = 0xCD;
            AddDummyInstruction(opCode, Internals.Instructions.Internal.Execution.ZeroPage);
            LoadData([opCode, adl, 0x00, 0x00, data]);
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
        public void T1_IsCorrect() // fetch effective address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Low);
            Assert.False(opCalled);
        }

        [Fact]
        public void T2_IsCorrect() // fetch data and execute operation
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 0, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 1, pc: 3);

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
            AddDummyInstruction(opCode, Internals.Instructions.Internal.Execution.Absolute);

            program = new byte[0x200];
            program[0] = opCode;
            program[1] = adl;
            program[2] = adh;
            program[(adh << 8) | adl] = data;

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
        public void T3_IsCorrect() // fetch data and execute operation
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 0, pc: 3);

            Assert.Equal((adh << 8) | adl, system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 1, pc: 4);

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
            AddDummyInstruction(opCode, Internals.Instructions.Internal.Execution.IndirectX);

            system.CPU.Registers.X = 4;

            byte[] program = new byte[0x200];
            program[0] = opCode;
            program[1] = bal;
            program[2] = 0x00;
            program[3] = adl;
            program[4] = adh;
            program[(adh << 8) | adl] = data;

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
        public void T5_IsCorrect() // fetch data and execute operation
        {
            Tick(6);
            CheckSystem(readCount: 6, writeCount: 0, cycles: 0, pc: 2);

            Assert.Equal((adh << 8) | adl, system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(7);
            CheckSystem(readCount: 7, writeCount: 0, cycles: 1, pc: 3);

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
            AddDummyInstruction(opCode, Steps);

            program = new byte[0x300];
            program[0] = opCode;
            program[1] = bal;
            program[2] = bah;

            LoadData(program);
        }

        protected void ArrangePageCrossing(bool shouldCross)
        {
            IndexRegister = (byte)(shouldCross ? 0xFF : 0x42);
            program[((bah << 8) | bal) + IndexRegister] = data;
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

        [Fact]
        public void T3_WhenPageIsNotCrossed_IsCorrect() // fetch data (no page crossing) and execute operation
        {
            ArrangePageCrossing(shouldCross: false);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 0, pc: 3);

            Assert.Equal((bah << 8) | (byte)(bal + IndexRegister), system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_PageIsNotCrossed_IsCorrect() // next instruction (no page crossing)
        {
            ArrangePageCrossing(shouldCross: false);
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 1, pc: 4);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T3_WhenPageIsCrossed_IsCorrect() // fetch data (with page crossing)
        {
            ArrangePageCrossing(shouldCross: true);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 3);

            Assert.Equal((bah << 8) | (byte)(bal + IndexRegister), system.RAM.LastReadAddress); // ignored data fetch
            Assert.False(opCalled);
        }

        [Fact]
        public void T4_WhenPageIsCrossed_IsCorrect() // fetch data (with page crossing) and execute operation
        {
            ArrangePageCrossing(shouldCross: true);
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 0, pc: 3);

            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_PageIsCrossed_IsCorrect() // next instruction (with page crossing)
        {
            ArrangePageCrossing(shouldCross: true);
            Tick(6);
            CheckSystem(readCount: 6, writeCount: 0, cycles: 1, pc: 4);

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

        protected override Steps Steps => Internals.Instructions.Internal.Execution.AbsoluteX;
    }

    public class AbsoluteY : AbsoluteIndexed
    {
        protected override byte IndexRegister
        {
            get => system.CPU.Registers.Y;
            set => system.CPU.Registers.Y = value;
        }

        protected override Steps Steps => Internals.Instructions.Internal.Execution.AbsoluteY;
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
            AddDummyInstruction(opCode, Steps);

            program = new byte[0x200];
            program[0] = opCode;
            program[1] = bal;

            LoadData(program);
        }

        protected void ArrangeWrapAround(bool shouldWrap)
        {
            IndexRegister = (byte)(shouldWrap ? 0xFF : 0x42);
            program[(byte)(bal + IndexRegister)] = data;
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

            Assert.Equal(bal, system.RAM.LastReadAddress);
            Assert.False(opCalled);
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T3_IsCorrect(bool shouldWrap) // fetch data and execute operation
        {
            ArrangeWrapAround(shouldWrap);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 0, pc: 2);

            Assert.Equal((byte)(bal + IndexRegister), system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 1, pc: 3);

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

        protected override Steps Steps => Internals.Instructions.Internal.Execution.ZeroPageX;
    }

    public class ZeroPageY : ZeroPageIndexed
    {
        protected override byte IndexRegister
        {
            get => system.CPU.Registers.Y;
            set => system.CPU.Registers.Y = value;
        }

        protected override Steps Steps => Internals.Instructions.Internal.Execution.ZeroPageY;
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
            AddDummyInstruction(opCode, Internals.Instructions.Internal.Execution.IndirectY);

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
            Assert.False(opCalled);
        }

        [Fact]
        public void T4_WhenPageIsNotCrossed_IsCorrect() // fetch data (no page crossing) and execute operation
        {
            ArrangePageCrossing(shouldCross: false);
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 0, pc: 2);

            Assert.Equal((bah << 8) | (byte)(bal + system.CPU.Registers.Y), system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_PageIsNotCrossed_IsCorrect() // next instruction (no page crossing)
        {
            ArrangePageCrossing(shouldCross: false);
            Tick(6);
            CheckSystem(readCount: 6, writeCount: 0, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }

        [Fact]
        public void T4_WhenPageIsCrossed_IsCorrect() // fetch data (with page crossing), ignored
        {
            ArrangePageCrossing(shouldCross: true);
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 5, pc: 2);

            Assert.Equal((bah << 8) | (byte)(bal + system.CPU.Registers.Y), system.CPU.Address.Full);
            Assert.Equal(system.CPU.Address.Full, system.RAM.LastReadAddress);
            Assert.False(opCalled);
        }

        [Fact]
        public void T5_WhenPageIsCrossed_IsCorrect() // fetch data (with page crossing) and execute operation
        {
            ArrangePageCrossing(shouldCross: true);
            Tick(6);
            CheckSystem(readCount: 6, writeCount: 0, cycles: 0, pc: 2);

            Assert.Equal(((bah << 8) | bal) + system.CPU.Registers.Y, system.CPU.Address.Full);
            Assert.Equal(data, system.CPU.Data);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_PageIsCrossed_IsCorrect() // next instruction (with page crossing)
        {
            ArrangePageCrossing(shouldCross: true);
            Tick(7);
            CheckSystem(readCount: 7, writeCount: 0, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }
}
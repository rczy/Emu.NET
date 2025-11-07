using CPU.MOS6502.Internals.Instructions;

namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

public class ReadModifyWrite
{
    public class ZeroPage : Base
    {
        protected byte origData;

        public ZeroPage() : base()
        {
            opCode = 0xAB;
            adl = 0x04;
            origData = 0xCD;
            data = 0xEF;
            AddWriteInstruction(opCode, Internals.Instructions.ReadModifyWrite.Execution.ZeroPage);
            LoadData([opCode, adl, 0x00, 0x00, origData]);
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
        public void T1_IsCorrect() // fetch page zero effective address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch data
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Full);
            Assert.Equal(adl, system.RAM.LastReadAddress);
            Assert.Equal(origData, system.CPU.Data);
        }

        [Fact]
        public void T3_IsCorrect() // write original data back
        {
            Tick(4);
            CheckSystem(readCount: 3, writeCount: 1, cycles: 4, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Full);
            Assert.Equal(adl, system.RAM.LastWriteAddress);
            Assert.Equal(origData, system.RAM.PeekAt(adl));
            Assert.False(opCalled);
        }

        [Fact]
        public void T4_IsCorrect() // execute instruction and write modified data
        {
            Tick(5);
            CheckSystem(readCount: 3, writeCount: 2, cycles: 0, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Full);
            Assert.Equal(adl, system.RAM.LastWriteAddress);
            Assert.NotEqual(origData, system.RAM.PeekAt(adl));
            Assert.Equal(data, system.RAM.PeekAt(adl));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(6);
            CheckSystem(readCount: 4, writeCount: 2, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class Absolute : Base
    {
        protected byte origData;

        public Absolute() : base()
        {
            opCode = 0xAB;
            adl = 0x23;
            adh = 0x01;
            origData = 0xCD;
            data = 0xEF;
            AddWriteInstruction(opCode, Internals.Instructions.ReadModifyWrite.Execution.Absolute);

            program = new byte[0x200];
            program[0] = opCode;
            program[1] = adl;
            program[2] = adh;
            program[(adh << 8) | adl] = origData;

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
        public void T1_IsCorrect() // fetch low order byte of effective address
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 2, pc: 2);

            Assert.Equal(adl, system.CPU.Address.Low);
        }

        [Fact]
        public void T2_IsCorrect() // fetch high order byte of effective address
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 3, pc: 3);

            Assert.Equal(adh, system.CPU.Address.High);
        }

        [Fact]
        public void T3_IsCorrect() // fetch data
        {
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 3);

            Assert.Equal((adh << 8) | adl, system.CPU.Address.Full);
            Assert.Equal((adh << 8) | adl, system.RAM.LastReadAddress);
            Assert.Equal(origData, system.CPU.Data);
        }

        [Fact]
        public void T4_IsCorrect() // write original data back
        {
            Tick(5);
            CheckSystem(readCount: 4, writeCount: 1, cycles: 5, pc: 3);

            Assert.Equal((adh << 8) | adl, system.CPU.Address.Full);
            Assert.Equal((adh << 8) | adl, system.RAM.LastWriteAddress);
            Assert.Equal(origData, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.False(opCalled);
        }

        [Fact]
        public void T5_IsCorrect() // execute instruction and write modified data
        {
            Tick(6);
            CheckSystem(readCount: 4, writeCount: 2, cycles: 0, pc: 3);

            Assert.Equal((adh << 8) | adl, system.CPU.Address.Full);
            Assert.Equal((adh << 8) | adl, system.RAM.LastWriteAddress);
            Assert.NotEqual(origData, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.Equal(data, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(7);
            CheckSystem(readCount: 5, writeCount: 2, cycles: 1, pc: 4);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class ZeroPageX : Base
    {
        protected byte origData;

        protected byte IndexRegister
        {
            get => system.CPU.Registers.X;
            set => system.CPU.Registers.X = value;
        }

        public ZeroPageX() : base()
        {
            opCode = 0xAB;
            bal = 0x23;
            origData = 0xCD;
            data = 0xEF;
            AddWriteInstruction(opCode, Internals.Instructions.ReadModifyWrite.Execution.ZeroPageX);

            program = new byte[0x200];
            program[0] = opCode;
            program[1] = bal;

            LoadData(program);
        }

        protected void ArrangeWrapAround(bool shouldWrap)
        {
            IndexRegister = (byte)(shouldWrap ? 0xFF : 0x42);
            program[(byte)(bal + IndexRegister)] = origData;
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
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T3_IsCorrect(bool shouldWrap) // fetch data
        {
            ArrangeWrapAround(shouldWrap);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 2);

            Assert.Equal((byte)(bal + IndexRegister), system.CPU.Address.Full);
            Assert.Equal((byte)(bal + IndexRegister), system.RAM.LastReadAddress);
            Assert.Equal(origData, system.CPU.Data);
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T4_IsCorrect(bool shouldWrap) // write original data back
        {
            ArrangeWrapAround(shouldWrap);
            Tick(5);
            CheckSystem(readCount: 4, writeCount: 1, cycles: 5, pc: 2);

            Assert.Equal((byte)(bal + IndexRegister), system.CPU.Address.Full);
            Assert.Equal((byte)(bal + IndexRegister), system.RAM.LastWriteAddress);
            Assert.Equal(origData, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.False(opCalled);
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T5_IsCorrect(bool shouldWrap) // execute instruction and write modified data
        {
            ArrangeWrapAround(shouldWrap);
            Tick(6);
            CheckSystem(readCount: 4, writeCount: 2, cycles: 0, pc: 2);

            Assert.Equal((byte)(bal + IndexRegister), system.CPU.Address.Full);
            Assert.Equal((byte)(bal + IndexRegister), system.RAM.LastWriteAddress);
            Assert.NotEqual(origData, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.Equal(data, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(7);
            CheckSystem(readCount: 5, writeCount: 2, cycles: 1, pc: 3);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }

    public class AbsoluteX : Base
    {
        protected byte origData;

        protected byte IndexRegister
        {
            get => system.CPU.Registers.X;
            set => system.CPU.Registers.X = value;
        }

        public AbsoluteX() : base()
        {
            opCode = 0xAB;
            bal = 0x23;
            bah = 0x01;
            origData = 0xCD;
            data = 0xEF;
            AddWriteInstruction(opCode, Internals.Instructions.ReadModifyWrite.Execution.AbsoluteX);

            program = new byte[0x300];
            program[0] = opCode;
            program[1] = bal;
            program[2] = bah;

            LoadData(program);
        }

        protected void ArrangePageCrossing(bool shouldCross)
        {
            IndexRegister = (byte)(shouldCross ? 0xFF : 0x42);
            program[((bah << 8) | bal) + IndexRegister] = origData;
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
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T3_IsCorrect(bool shouldCross) // fetch data
        {
            ArrangePageCrossing(shouldCross);
            Tick(4);
            CheckSystem(readCount: 4, writeCount: 0, cycles: 4, pc: 3);

            Assert.Equal((bah << 8) | (byte)(bal + IndexRegister), system.RAM.LastReadAddress); // ignored data fetch
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T4_IsCorrect(bool shouldCross) // fetch data
        {
            ArrangePageCrossing(shouldCross);
            Tick(5);
            CheckSystem(readCount: 5, writeCount: 0, cycles: 5, pc: 3);

            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.CPU.Address.Full);
            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.RAM.LastReadAddress);
            Assert.Equal(origData, system.CPU.Data);
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T5_IsCorrect(bool shouldCross) // write original data back
        {
            ArrangePageCrossing(shouldCross);
            Tick(6);
            CheckSystem(readCount: 5, writeCount: 1, cycles: 6, pc: 3);

            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.CPU.Address.Full);
            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.RAM.LastWriteAddress);
            Assert.Equal(origData, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.False(opCalled);
        }

        [Theory]
        [InlineData(true)] // wrap around
        [InlineData(false)] // no wrap around
        public void T6_IsCorrect(bool shouldCross) // execute instruction and write modified data
        {
            ArrangePageCrossing(shouldCross);
            Tick(7);
            CheckSystem(readCount: 5, writeCount: 2, cycles: 0, pc: 3);

            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.CPU.Address.Full);
            Assert.Equal(((bah << 8) | bal) + IndexRegister, system.RAM.LastWriteAddress);
            Assert.NotEqual(origData, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.Equal(data, system.RAM.PeekAt(system.RAM.LastWriteAddress));
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction (with page crossing)
        {
            Tick(8);
            CheckSystem(readCount: 6, writeCount: 2, cycles: 1, pc: 4);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }
}
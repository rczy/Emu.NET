namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

using ExecSteps = Machinery.Instructions.SingleByte.Execution;

public abstract class SingleByteTests
{
    [Trait("Category", "Unit")]
    public class Implied : Base
    {
        public Implied()
        {
            opCode = 0xAB;
            AddDummyInstruction(opCode, ExecSteps.Implied);
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
        public void T1_IsCorrect() // dummy read and execution
        {
            Tick(2);
            CheckSystem(readCount: 2, writeCount: 0, cycles: 0, pc: 1);

            Assert.Equal(system.RAM.LastReadAddress, system.CPU.Registers.PC);
            Assert.True(opCalled);
        }

        [Fact]
        public void After_IsCorrect() // next instruction
        {
            Tick(3);
            CheckSystem(readCount: 3, writeCount: 0, cycles: 1, pc: 2);

            Assert.NotEqual(opCode, system.CPU.Decoder.OpCode);
        }
    }
}
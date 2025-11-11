using CPU.MOS6502.Machinery.Instructions;
using CPU.MOS6502.Tests.Utils;

namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

public class Base
{
    protected readonly SimpleSystem system;

    protected byte opCode; // dummy opcode
    protected byte data; // dummy data
    protected byte adl; // effective addres low byte
    protected byte adh; // effective addres high byte
    protected byte bal; // base addres low byte
    protected byte bah; // base addres high byte
    protected byte ial; // indirect addres low byte
    protected byte iah; // indirect addres high byte

    protected byte[] program = [];

    protected bool opCalled; // operation is executed
    protected int ticks;

    protected Base()
    {
        system = new SimpleSystem();
        opCalled = false;
        ticks = 0;
    }

    protected void AddDummyInstruction(byte opCode, Steps steps)
    {
        AddInstruction(opCode, cpu => opCalled = true, steps);
    }

    protected void AddWriteInstruction(byte opCode, Steps steps)
    {
        void write(Core cpu)
        {
            cpu.Bus.Write(cpu.Address, data);
            opCalled = true;
        }
        AddInstruction(opCode, write, steps);
    }

    protected void AddInstruction(byte opCode, Operation op, Steps steps)
    {
        system.CPU.Decoder.AddInstruction(opCode, "TEST", "test addressing", op, steps);
    }

    protected void LoadData(byte[] data)
    {
        system.RAM.LoadData(data);
    }

    protected void Tick(int cycles)
    {
        ticks = cycles;
        for (int i = 0; i < cycles; i++)
            system.CPU.Tick();
    }

    protected void CheckSystem(byte readCount, byte writeCount, int cycles, ushort pc)
    {
        Assert.Equal(readCount, system.RAM.ReadCount);
        Assert.Equal(writeCount, system.RAM.WriteCount);
        Assert.Equal(cycles, system.CPU.Cycles);
        Assert.Equal(pc, system.CPU.Registers.PC);
        Assert.Equal(ticks, readCount + writeCount);
    }
}
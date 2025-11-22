using CPU.MOS6502.Machinery.Instructions;
using CPU.MOS6502.Tests.Utils;

namespace CPU.MOS6502.Tests.Unit.Instructions.Execution;

public class Base
{
    protected readonly SimpleSystem system;

    protected byte opCode; // dummy opcode
    protected byte data; // dummy data
    protected byte adl; // effective address low byte
    protected byte adh; // effective address high byte
    protected byte bal; // base address low byte
    protected byte bah; // base address high byte
    protected byte ial; // indirect address low byte
    protected byte iah; // indirect address high byte

    protected byte[] program = [];

    protected bool opCalled; // operation is executed
    private int _ticks;

    protected Base()
    {
        system = new SimpleSystem();
        opCalled = false;
        _ticks = 0;
    }

    protected void AddDummyInstruction(byte opcode, Steps steps)
    {
        AddInstruction(opcode, _ => opCalled = true, steps);
    }

    protected void AddWriteInstruction(byte opcode, Steps steps)
    {
        AddInstruction(opcode, Write, steps);
        return;

        void Write(Core cpu)
        {
            cpu.Data = data;
            opCalled = true;
        }
    }

    protected void AddInstruction(byte opcode, Operation op, Steps steps)
    {
        system.CPU.Decoder.AddInstruction(opcode, op, steps);
    }

    protected void LoadData(byte[] bytes)
    {
        system.RAM.LoadData(bytes);
    }

    protected void Tick(int cycles)
    {
        for (int i = 0; i < cycles; i++)
        {
            system.CPU.Tick();
            _ticks++;
        }
    }

    protected void CheckSystem(byte readCount, byte writeCount, int cycles, ushort pc)
    {
        Assert.Equal(readCount, system.RAM.ReadCount);
        Assert.Equal(writeCount, system.RAM.WriteCount);
        Assert.Equal(cycles, system.CPU.Cycles);
        Assert.Equal(pc, system.CPU.Registers.PC);
        Assert.Equal(_ticks, readCount + writeCount);
    }
}
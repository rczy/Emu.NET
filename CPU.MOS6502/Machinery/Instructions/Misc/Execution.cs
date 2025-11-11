namespace CPU.MOS6502.Machinery.Instructions.Misc;

static class Execution
{
    public static bool Push(Core cpu, Operation op) // 3 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Data = cpu.Bus.Read(cpu.Registers.PC); // dummy read
                return false;
            case 2:
                cpu.Address.Low = cpu.Registers.SP--;
                cpu.Address.High = 0x01;
                break;
        }
        op(cpu);
        return true;
    }

    public static bool Pull(Core cpu, Operation op) // 4 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Data = cpu.Bus.Read(cpu.Registers.PC); // dummy read
                return false;
            case 2:
                cpu.Address.Low = cpu.Registers.SP;
                cpu.Address.High = 0x01;
                cpu.Data = cpu.Bus.Read(cpu.Address); // dummy read
                return false;
            case 3:
                cpu.Address.Low = ++cpu.Registers.SP;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                break;
        }
        op(cpu);
        return true;
    }

    public static bool JumpToSubroutine(Core cpu, Operation op) // 6 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Address.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.Data = cpu.Bus.Read((ushort)(0x0100 | cpu.Registers.SP)); // dummy read
                return false;
            case 3:
                cpu.Bus.Write((ushort)(0x0100 | cpu.Registers.SP--), (byte)(cpu.Registers.PC >>> 8));
                return false;
            case 4:
                cpu.Bus.Write((ushort)(0x0100 | cpu.Registers.SP--), (byte)(cpu.Registers.PC & 0x00FF));
                return false;
            case 5:
                cpu.Address.High = cpu.Bus.Read(cpu.Registers.PC);
                break;
        }
        op(cpu);
        return true;
    }
}
namespace CPU.MOS6502.Machinery.Instructions.ReadModifyWrite;

static class Execution
{
    public static bool ZeroPage(Core cpu, Operation op) // 5 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Address.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.Address.High = 0x00;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                return false;
            case 3:
                cpu.Bus.Write(cpu.Address, cpu.Data);
                return false;
            case 4:
                op(cpu);
                cpu.Bus.Write(cpu.Address, cpu.Data);
                break;
        }
        return true;
    }

    public static bool Absolute(Core cpu, Operation op) // 6 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Address.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.Address.High = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 3:
                cpu.Data = cpu.Bus.Read(cpu.Address);
                return false;
            case 4:
                cpu.Bus.Write(cpu.Address, cpu.Data);
                return false;
            case 5:
                op(cpu);
                cpu.Bus.Write(cpu.Address, cpu.Data);
                break;
        }
        return true;
    }

    public static bool ZeroPageX(Core cpu, Operation op) // 6 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.BaseAddress.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.BaseAddress.High = 0x00;
                cpu.Data = cpu.Bus.Read(cpu.BaseAddress); // dummy read
                return false;
            case 3:
                cpu.BaseAddress.Low += cpu.Registers.X;
                cpu.Address.Full = cpu.BaseAddress.Full;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                return false;
            case 4:
                cpu.Bus.Write(cpu.Address, cpu.Data);
                return false;
            case 5:
                op(cpu);
                cpu.Bus.Write(cpu.Address, cpu.Data);
                break;
        }
        return true;
    }

    public static bool AbsoluteX(Core cpu, Operation op) // 7 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.BaseAddress.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.BaseAddress.High = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 3:
                int adl = cpu.BaseAddress.Low + cpu.Registers.X;
                cpu.Address.Low = (byte)adl;
                cpu.Address.High = cpu.BaseAddress.High;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                if (adl > 0xFF) // page boundary crossed
                {
                    cpu.Address.High++;
                }
                return false;
            case 4:
                cpu.Data = cpu.Bus.Read(cpu.Address);
                return false;
            case 5:
                cpu.Bus.Write(cpu.Address, cpu.Data);
                return false;
            case 6:
                op(cpu);
                cpu.Bus.Write(cpu.Address, cpu.Data);
                break;
        }
        return true;
    }
}
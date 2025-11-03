namespace CPU.MOS6502.Internals.Instructions.Internal;

static class Execution
{
    public static bool Immediate(Core cpu, Operation op) // 2 cycles
    {
        cpu.Data = cpu.Bus.Read(cpu.Registers.PC++);
        op(cpu);
        return true;
    }

    public static bool ZeroPage(Core cpu, Operation op) // 3 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Address.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.Address.High = 0x00;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                break;
        }
        op(cpu);
        return true;
    }

    public static bool Absolute(Core cpu, Operation op) // 4 cycles
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
                break;
        }
        op(cpu);
        return true;
    }

    public static bool IndirectX(Core cpu, Operation op) // 6 cycles
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
                cpu.Address.Low = cpu.Bus.Read((byte)(cpu.BaseAddress + cpu.Registers.X));
                return false;
            case 4:
                cpu.Address.High = cpu.Bus.Read((byte)(cpu.BaseAddress + cpu.Registers.X + 1));
                return false;
            case 5:
                cpu.Data = cpu.Bus.Read(cpu.Address);
                break;
        }
        op(cpu);
        return true;
    }

    private static bool Absolute_(Core cpu, Operation op, byte register) // 4 or 5 cycles
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
                int adl = cpu.BaseAddress.Low + register;
                cpu.Address.Low = (byte)adl;
                cpu.Address.High = cpu.BaseAddress.High;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                if (adl > 0xFF) // page boundary crossed
                {
                    return false;
                }
                break;
            case 4:
                cpu.Address.High++;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                break;
        }
        op(cpu);
        return true;
    }

    public static bool AbsoluteX(Core cpu, Operation op)
    {
        return Absolute_(cpu, op, cpu.Registers.X);
    }

    public static bool AbsoluteY(Core cpu, Operation op)
    {
        return Absolute_(cpu, op, cpu.Registers.Y);
    }

    private static bool ZeroPage_(Core cpu, Operation op, byte register) // 4 cycles
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
                cpu.BaseAddress.Low += register;
                cpu.Address.Full = cpu.BaseAddress.Full;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                break;
        }
        op(cpu);
        return true;
    }

    public static bool ZeroPageX(Core cpu, Operation op)
    {
        return ZeroPage_(cpu, op, cpu.Registers.X);
    }

    public static bool ZeroPageY(Core cpu, Operation op)
    {
        return ZeroPage_(cpu, op, cpu.Registers.Y);
    }

    public static bool IndirectY(Core cpu, Operation op) // 5 or 6 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.IndirectAddress.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.IndirectAddress.High = 0x00;
                cpu.BaseAddress.Low = cpu.Bus.Read(cpu.IndirectAddress);
                return false;
            case 3:
                cpu.IndirectAddress.Low++;
                cpu.BaseAddress.High = cpu.Bus.Read(cpu.IndirectAddress);
                return false;
            case 4:
                int adl = cpu.BaseAddress.Low + cpu.Registers.Y;
                cpu.Address.Low = (byte)adl;
                cpu.Address.High = cpu.BaseAddress.High;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                if (adl > 0xFF) // page boundary crossed
                {
                    return false;
                }
                break;
            case 5:
                cpu.Address.High++;
                cpu.Data = cpu.Bus.Read(cpu.Address);
                break;
        }
        op(cpu);
        return true;
    }
}
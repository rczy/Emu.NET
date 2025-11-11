namespace CPU.MOS6502.Machinery.Instructions.FlowAndStack;

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

    public static bool ReturnFromSubroutine(Core cpu, Operation op) // 6 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Data = cpu.Bus.Read(cpu.Registers.PC); // dummy read
                return false;
            case 2:
                cpu.Data = cpu.Bus.Read((ushort)(0x0100 | cpu.Registers.SP)); // dummy read
                return false;
            case 3:
                cpu.Registers.PC &= 0xFF00;
                cpu.Registers.PC |= cpu.Bus.Read((ushort)(0x0100 | ++cpu.Registers.SP));
                return false;
            case 4:
                cpu.Registers.PC &= 0x00FF;
                byte PCH = cpu.Data = cpu.Bus.Read((ushort)(0x0100 | ++cpu.Registers.SP));
                cpu.Registers.PC |= (ushort)(PCH << 8);
                return false;
            case 5:
                cpu.Data = cpu.Bus.Read(cpu.Registers.PC); // dummy read
                break;
        }
        op(cpu);
        return true;
    }

    public static bool JumpAbsolute(Core cpu, Operation op) // 3 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Address.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.Address.High = cpu.Bus.Read(cpu.Registers.PC);
                break;
        }
        op(cpu);
        return true;
    }

    public static bool JumpImplied(Core cpu, Operation op) // 5 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.IndirectAddress.Low = cpu.Bus.Read(cpu.Registers.PC++);
                return false;
            case 2:
                cpu.IndirectAddress.High = cpu.Bus.Read(cpu.Registers.PC);
                return false;
            case 3:
                cpu.Address.Low = cpu.Bus.Read(cpu.IndirectAddress);
                return false;
            case 4:
                cpu.Address.High = cpu.Bus.Read((ushort)(cpu.IndirectAddress + 1));
                break;
        }
        op(cpu);
        return true;
    }

    public static bool Branch(Core cpu, Operation op) // 2, 3 or 4 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Address.Low = cpu.Bus.Read(cpu.Registers.PC++);
                cpu.Address.High = 0x00;
                op(cpu);
                if (cpu.Data == 0) // branch not taken
                {
                    return true;
                }
                return false;
            case 2:
                int pcl = (byte)cpu.Registers.PC + cpu.Address;
                cpu.Registers.PC = (ushort)(cpu.Registers.PC & 0xFF00 | pcl & 0x00FF);
                cpu.Bus.Read(cpu.Registers.PC);
                if (pcl > 0xFF) // page boundary crossed
                {
                    return false;
                }
                break;
            case 3:
                cpu.Registers.PC += 0x100;
                cpu.Bus.Read(cpu.Registers.PC);
                break;
        }
        return true;
    }
}
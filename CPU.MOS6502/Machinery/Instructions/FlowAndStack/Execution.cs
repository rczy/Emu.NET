namespace CPU.MOS6502.Machinery.Instructions.FlowAndStack;

using Seq = InterruptHandler.Interrupts;

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
                cpu.Bus.Write((ushort)(0x0100 | cpu.Registers.SP--), (byte)cpu.Registers.PC);
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
                byte pch = cpu.Data = cpu.Bus.Read((ushort)(0x0100 | ++cpu.Registers.SP));
                cpu.Registers.PC |= (ushort)(pch << 8);
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
                cpu.Address.Low = cpu.Bus.Read(cpu.Registers.PC++); // fetch offset
                op(cpu);
                if (cpu.Data == 0) // branch not taken
                {
                    return true;
                }
                return false;
            case 2:
                var offset = cpu.Address.Low;
                var negative = (offset & 0x80) != 0;
                var pcl = (byte)cpu.Registers.PC + offset;
                var carry = (pcl & 0x100) != 0;
                cpu.Registers.PC = (ushort)((cpu.Registers.PC & 0xFF00) | (pcl & 0x00FF));
                cpu.Bus.Read(cpu.Registers.PC);
                if (negative ^ carry) // page boundary crossed
                {
                    cpu.Address.High = (byte)(carry ? 0x01 : 0xFF);
                    cpu.Address.Low = 0x00;
                    return false;
                }
                break;
            case 3:
                cpu.Registers.PC += cpu.Address;
                cpu.Bus.Read(cpu.Registers.PC);
                break;
        }
        return true;
    }

    public static bool Break(Core cpu, Operation op) // 7 cycles
    {
        switch (cpu.Cycles)
        {
            case 1:
                cpu.Data = cpu.Bus.Read(cpu.Registers.PC); // dummy read
                return false;
            case 2:
                AccessStackWith((byte)(cpu.Registers.PC >>> 8));
                return false;
            case 3:
                AccessStackWith((byte)cpu.Registers.PC);
                return false;
            case 4:
                cpu.Registers.P.Break = cpu.InterruptHandler.Sequence == Seq.None;
                AccessStackWith(cpu.Registers.P);
                return false;
            case 5:
                cpu.Registers.P.Interrupt = true;
                ushort vectorLow = cpu.InterruptHandler.Sequence switch
                {
                    Seq.NMI => 0xFFFA,
                    Seq.RES => 0xFFFC,
                    Seq.IRQ => 0xFFFE,
                    Seq.None => 0xFFFE,
                };
                cpu.Address.Low = cpu.Bus.Read(vectorLow);
                return false;
            case 6:
                ushort vectorHigh = cpu.InterruptHandler.Sequence switch
                {
                    Seq.NMI => 0xFFFB,
                    Seq.RES => 0xFFFD,
                    Seq.IRQ => 0xFFFF,
                    Seq.None => 0xFFFF,
                };
                cpu.Address.High = cpu.Bus.Read(vectorHigh);

                if (cpu.InterruptHandler.Sequence == Seq.NMI)
                {
                    cpu.InterruptHandler.PendingNMI = false;
                }
                break;
        }
        op(cpu);
        return true;

        void AccessStackWith(byte data)
        {
            var sp = (ushort)(0x0100 | cpu.Registers.SP--);
            if (cpu.InterruptHandler.Sequence != Seq.RES)
            {
                cpu.Bus.Write(sp, data);
            }
            else
            {
                cpu.Bus.Read(sp);
            }
        }
    }

    public static bool ReturnFromInterrupt(Core cpu, Operation op) // 6 cycles
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
                cpu.Registers.P = cpu.Bus.Read((ushort)(0x0100 | ++cpu.Registers.SP));
                cpu.Registers.P.Break = false;
                return false;
            case 4:
                cpu.Registers.PC &= 0xFF00;
                cpu.Registers.PC |= cpu.Bus.Read((ushort)(0x0100 | ++cpu.Registers.SP));
                return false;
            case 5:
                cpu.Registers.PC &= 0x00FF;
                byte pch = cpu.Data = cpu.Bus.Read((ushort)(0x0100 | ++cpu.Registers.SP));
                cpu.Registers.PC |= (ushort)(pch << 8);
                break;
        }
        op(cpu);
        return true;
    }
}
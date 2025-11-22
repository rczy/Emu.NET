namespace CPU.MOS6502.Machinery.Instructions.ReadModifyWrite;

static class Operations
{
    public static void ASL(Core cpu)
    {
        cpu.Registers.P.Carry = (cpu.Data & 0x80) != 0;
        cpu.Data <<= 1;
        cpu.Registers.P.Negative = (cpu.Data & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Data == 0;
        cpu.Bus.Write(cpu.Address, cpu.Data);
    }
    
    public static void DEC(Core cpu)
    {
        cpu.Data -= 1;
        cpu.Registers.P.Negative = (cpu.Data & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Data == 0;
        cpu.Bus.Write(cpu.Address, cpu.Data);
    }
    
    public static void INC(Core cpu)
    {
        cpu.Data += 1;
        cpu.Registers.P.Negative = (cpu.Data & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Data == 0;
        cpu.Bus.Write(cpu.Address, cpu.Data);
    }
    
    public static void LSR(Core cpu)
    {
        cpu.Registers.P.Carry = (cpu.Data & 0x01) != 0;
        cpu.Data >>>= 1;
        cpu.Registers.P.Negative = false;
        cpu.Registers.P.Zero = cpu.Data == 0;
        cpu.Bus.Write(cpu.Address, cpu.Data);
    }
    
    public static void ROL(Core cpu)
    {
        var carry = (byte)(cpu.Registers.P.Carry ? 1 : 0);
        cpu.Registers.P.Carry = (cpu.Data & 0x80) != 0;
        cpu.Data <<= 1;
        cpu.Data |= carry;
        cpu.Registers.P.Negative = (cpu.Data & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Data == 0;
        cpu.Bus.Write(cpu.Address, cpu.Data);
    }
    
    public static void ROR(Core cpu)
    {
        var carry = (byte)(cpu.Registers.P.Carry ? 0x80 : 0);
        cpu.Registers.P.Carry = (cpu.Data & 0x01) != 0;
        cpu.Data >>>= 1;
        cpu.Data |= carry;
        cpu.Registers.P.Negative = (cpu.Data & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Data == 0;
        cpu.Bus.Write(cpu.Address, cpu.Data);
    }
}
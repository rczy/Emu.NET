namespace CPU.MOS6502.Machinery.Instructions.SingleByte;

static class Operations
{
    public static void UNKNOWN(Core cpu)
    {
    }
    
    public static void ASL(Core cpu)
    {
        cpu.Registers.P.Carry = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.A <<= 1;
        cpu.Registers.P.Negative = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.A == 0;
    }
    
    public static void CLC(Core cpu)
    {
        cpu.Registers.P.Carry = false;
    }
    
    public static void CLD(Core cpu)
    {
        cpu.Registers.P.Decimal = false;
    }
    
    public static void CLI(Core cpu)
    {
        cpu.Registers.P.Interrupt = false;
    }
    
    public static void CLV(Core cpu)
    {
        cpu.Registers.P.Overflow = false;
    }
    
    public static void DEX(Core cpu)
    {
        cpu.Registers.X -= 1;
        cpu.Registers.P.Negative = (cpu.Registers.X & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.X == 0;
    }
    
    public static void DEY(Core cpu)
    {
        cpu.Registers.Y -= 1;
        cpu.Registers.P.Negative = (cpu.Registers.Y & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.Y == 0;
    }
    
    public static void INX(Core cpu)
    {
        cpu.Registers.X += 1;
        cpu.Registers.P.Negative = (cpu.Registers.X & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.X == 0;
    }
    
    public static void INY(Core cpu)
    {
        cpu.Registers.Y += 1;
        cpu.Registers.P.Negative = (cpu.Registers.Y & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.Y == 0;
    }
    
    public static void LSR(Core cpu)
    {
        cpu.Registers.P.Carry = (cpu.Registers.A & 0x01) != 0;
        cpu.Registers.A >>>= 1;
        cpu.Registers.P.Negative = false;
        cpu.Registers.P.Zero = cpu.Registers.A == 0;
    }
    
    public static void NOP(Core cpu)
    {
    }
    
    public static void ROL(Core cpu)
    {
        var carry = (byte)(cpu.Registers.P.Carry ? 1 : 0);
        cpu.Registers.P.Carry = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.A <<= 1;
        cpu.Registers.A |= carry;
        cpu.Registers.P.Negative = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.A == 0;
    }
    
    public static void ROR(Core cpu)
    {
        var carry = (byte)(cpu.Registers.P.Carry ? 0x80 : 0);
        cpu.Registers.P.Carry = (cpu.Registers.A & 0x01) != 0;
        cpu.Registers.A >>>= 1;
        cpu.Registers.A |= carry;
        cpu.Registers.P.Negative = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.A == 0;
    }
    
    public static void SEC(Core cpu)
    {
        cpu.Registers.P.Carry = true;
    }
    
    public static void SED(Core cpu)
    {
        cpu.Registers.P.Decimal = true;
    }
    
    public static void SEI(Core cpu)
    {
        cpu.Registers.P.Interrupt = true;
    }
    
    public static void TAX(Core cpu)
    {
        cpu.Registers.X = cpu.Registers.A;
        cpu.Registers.P.Negative = (cpu.Registers.X & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.X == 0;
    }
    
    public static void TAY(Core cpu)
    {
        cpu.Registers.Y = cpu.Registers.A;
        cpu.Registers.P.Negative = (cpu.Registers.Y & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.Y == 0;
    }
    
    public static void TSX(Core cpu)
    {
        cpu.Registers.X = cpu.Registers.SP;
        cpu.Registers.P.Negative = (cpu.Registers.X & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.X == 0;
    }
    
    public static void TXA(Core cpu)
    {
        cpu.Registers.A = cpu.Registers.X;
        cpu.Registers.P.Negative = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.A == 0;
    }
    
    public static void TXS(Core cpu)
    {
        cpu.Registers.SP = cpu.Registers.X;
    }
    
    public static void TYA(Core cpu)
    {
        cpu.Registers.A = cpu.Registers.Y;
        cpu.Registers.P.Negative = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.A == 0;
    }
}
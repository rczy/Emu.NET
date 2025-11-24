namespace CPU.MOS6502.Machinery.Instructions.FlowAndStack;

static class Operations
{
    public static void BCC(Core cpu)
    {
        cpu.Data = (byte)(!cpu.Registers.P.Carry ? 1 : 0);
    }
    
    public static void BCS(Core cpu)
    {
        cpu.Data = (byte)(cpu.Registers.P.Carry ? 1 : 0);
    }
    
    public static void BEQ(Core cpu)
    {
        cpu.Data = (byte)(cpu.Registers.P.Zero ? 1 : 0);
    }
    
    public static void BMI(Core cpu)
    {
        cpu.Data = (byte)(cpu.Registers.P.Negative ? 1 : 0);
    }
    
    public static void BNE(Core cpu)
    {
        cpu.Data = (byte)(!cpu.Registers.P.Zero ? 1 : 0);
    }
    
    public static void BPL(Core cpu)
    {
        cpu.Data = (byte)(!cpu.Registers.P.Negative ? 1 : 0);
    }
    
    public static void BRK(Core cpu)
    {
        cpu.Registers.PC = cpu.Address;
    }
    
    public static void BVC(Core cpu)
    {
        cpu.Data = (byte)(!cpu.Registers.P.Overflow ? 1 : 0);
    }
    
    public static void BVS(Core cpu)
    {
        cpu.Data = (byte)(cpu.Registers.P.Overflow ? 1 : 0);
    }
    
    public static void JMP(Core cpu)
    {
        cpu.Registers.PC = cpu.Address;
    }

    public static void JSR(Core cpu)
    {
        cpu.Registers.PC = cpu.Address;
    }
    
    public static void PHA(Core cpu)
    {
        cpu.Data = cpu.Registers.A;
    }
    
    public static void PHP(Core cpu)
    {
        cpu.Registers.P.Break = true;
        cpu.Data = (byte)cpu.Registers.P;
    }
    
    public static void PLA(Core cpu)
    {
        cpu.Registers.A = cpu.Data;
        cpu.Registers.P.Negative = (cpu.Registers.A & 0x80) != 0;
        cpu.Registers.P.Zero = cpu.Registers.A == 0;
    }
    
    public static void PLP(Core cpu)
    {
        var breakFlag = cpu.Registers.P.Break; 
        cpu.Registers.P = cpu.Data;
        cpu.Registers.P.Break = breakFlag;
    }
    
    public static void RTI(Core cpu)
    {
    }

    public static void RTS(Core cpu)
    {
        cpu.Registers.PC++;
    }
}
namespace CPU.MOS6502.Machinery.Instructions.Internal;

static class Operations
{
    public static void ADC(Core cpu)
    {
        var accumulator = cpu.Registers.A;
        var memory = cpu.Data;
        
        // low nibble adder
        var lo = (accumulator & 0x0F) + (memory & 0x0F) + (cpu.Registers.P.Carry ? 1 : 0);
        var halfCarry = (cpu.Registers.P.Decimal) ? (lo > 9) : (lo > 15);

        // high nibble adder
        var hi = (accumulator >>> 4) + (memory >>> 4) + (halfCarry ? 1 : 0);
        var carryOut = (cpu.Registers.P.Decimal) ? (hi > 9) : (hi > 15);
        
        var result = (hi << 4) | (lo & 0x0F);

        // compute flags
        cpu.Registers.P.Carry = carryOut;
        cpu.Registers.P.Negative = (result & 0x80) != 0;
        cpu.Registers.P.Zero = (result & 0xFF) == 0;
        cpu.Registers.P.Overflow = ((accumulator ^ result) & (memory ^ result) & 0x80) != 0;

        // decimal adjust
        if (cpu.Registers.P.Decimal)
        {
            if (halfCarry) lo += 6;
            if (carryOut) hi += 6;
            result = (hi << 4) | (lo & 0x0F);
        }

        // store result
        cpu.Registers.A = (byte)result;
    }
    
    public static void AND(Core cpu)
    {
    }
    
    public static void BIT(Core cpu)
    {
    }
    
    public static void CMP(Core cpu)
    {
    }
    
    public static void CPX(Core cpu)
    {
    }
    
    public static void CPY(Core cpu)
    {
    }
    
    public static void EOR(Core cpu)
    {
    }
    
    public static void LDA(Core cpu)
    {
    }
    
    public static void LDX(Core cpu)
    {
    }
    
    public static void LDY(Core cpu)
    {
    }
    
    public static void ORA(Core cpu)
    {
    }
    
    public static void SBC(Core cpu)
    {
        var accumulator = cpu.Registers.A;
        var memory = cpu.Data;
        
        // low nibble adder
        var lo = (accumulator & 0x0F) - (memory & 0x0F) - (cpu.Registers.P.Carry ? 0 : 1);
        var halfBorrow = lo < 0;

        // high nibble adder
        var hi = (accumulator >>> 4) - (memory >>> 4) - (halfBorrow ? 1 : 0);
        var borrowOut = hi < 0;
        
        var result = (hi << 4) | (lo & 0x0F);

        // compute flags
        cpu.Registers.P.Carry = !borrowOut;
        cpu.Registers.P.Negative = (result & 0x80) != 0;
        cpu.Registers.P.Zero = (result & 0xFF) == 0;
        cpu.Registers.P.Overflow = ((accumulator ^ result) & (~memory ^ result) & 0x80) != 0;

        // decimal adjust
        if (cpu.Registers.P.Decimal)
        {
            if (halfBorrow) lo -= 6;
            if (borrowOut) hi -= 6;
            result = (hi << 4) | (lo & 0x0F);
        }

        // store result
        cpu.Registers.A = (byte)result;
    }
}
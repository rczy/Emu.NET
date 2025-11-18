namespace CPU.MOS6502.Tests.Unit.Instructions.Operations;

public class SingleByteTests : Base
{
    [Theory]
    [InlineData(0b0000_0000, 0b0000_0000, false, true, false)]
    [InlineData(0b1000_0000, 0b0000_0000, false, true, true)]
    [InlineData(0b0100_0000, 0b1000_0000, true, false, false)]
    [InlineData(0b1100_0000, 0b1000_0000, true, false, true)]
    [InlineData(0b0010_0000, 0b0100_0000, false, false, false)]
    [InlineData(0b0000_0001, 0b0000_0010, false, false, false)]
    [InlineData(0b1111_1111, 0b1111_1110, true, false, true)]
    public void ASL_Executes_Correctly(byte input, byte result, bool negative, bool zero, bool carry)
    {
        system.CPU.Registers.A = input;
        Machinery.Instructions.SingleByte.Operations.ASL(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }

    [Fact]
    public void CLC_Executes_Correctly()
    {
        system.CPU.Registers.P.Carry = true;
        Machinery.Instructions.SingleByte.Operations.CLC(system.CPU);
        
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void CLD_Executes_Correctly()
    {
        system.CPU.Registers.P.Decimal = true;
        Machinery.Instructions.SingleByte.Operations.CLD(system.CPU);
        
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void CLI_Executes_Correctly()
    {
        system.CPU.Registers.P.Interrupt = true;
        Machinery.Instructions.SingleByte.Operations.CLI(system.CPU);
        
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void CLV_Executes_Correctly()
    {
        system.CPU.Registers.P.Overflow = true;
        Machinery.Instructions.SingleByte.Operations.CLV(system.CPU);
        
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }

    [Theory]
    [InlineData(0x02, 0x01, false, false)]
    [InlineData(0x01, 0x00, false, true)]
    [InlineData(0x00, 0xFF, true, false)]
    [InlineData(0xFF, 0xFE, true, false)]
    [InlineData(0x81, 0x80, true, false)]
    [InlineData(0x80, 0x7F, false, false)]
    public void DEX_Executes_Correctly(byte input, byte result, bool negative, bool zero)
    {
        system.CPU.Registers.X = input;
        Machinery.Instructions.SingleByte.Operations.DEX(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.X);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x02, 0x01, false, false)]
    [InlineData(0x01, 0x00, false, true)]
    [InlineData(0x00, 0xFF, true, false)]
    [InlineData(0xFF, 0xFE, true, false)]
    [InlineData(0x81, 0x80, true, false)]
    [InlineData(0x80, 0x7F, false, false)]
    public void DEY_Executes_Correctly(byte input, byte result, bool negative, bool zero)
    {
        system.CPU.Registers.Y = input;
        Machinery.Instructions.SingleByte.Operations.DEY(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.Y);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x01, 0x02, false, false)]
    [InlineData(0x00, 0x01, false, false)]
    [InlineData(0xFF, 0x00, false, true)]
    [InlineData(0xFE, 0xFF, true, false)]
    [InlineData(0x80, 0x81, true, false)]
    [InlineData(0x7F, 0x80, true, false)]
    public void INX_Executes_Correctly(byte input, byte result, bool negative, bool zero)
    {
        system.CPU.Registers.X = input;
        Machinery.Instructions.SingleByte.Operations.INX(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.X);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x01, 0x02, false, false)]
    [InlineData(0x00, 0x01, false, false)]
    [InlineData(0xFF, 0x00, false, true)]
    [InlineData(0xFE, 0xFF, true, false)]
    [InlineData(0x80, 0x81, true, false)]
    [InlineData(0x7F, 0x80, true, false)]
    public void INY_Executes_Correctly(byte input, byte result, bool negative, bool zero)
    {
        system.CPU.Registers.Y = input;
        Machinery.Instructions.SingleByte.Operations.INY(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.Y);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0b0000_0000, 0b0000_0000, true, false)]
    [InlineData(0b1000_0000, 0b0100_0000, false, false)]
    [InlineData(0b0000_0010, 0b0000_0001, false, false)]
    [InlineData(0b0000_0011, 0b0000_0001, false, true)]
    [InlineData(0b0000_0100, 0b0000_0010, false, false)]
    [InlineData(0b0000_0001, 0b0000_0000, true, true)]
    [InlineData(0b1111_1111, 0b0111_1111, false, true)]
    public void LSR_Executes_Correctly(byte input, byte result, bool zero, bool carry)
    {
        system.CPU.Registers.A = input;
        Machinery.Instructions.SingleByte.Operations.LSR(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative: false, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }

    [Theory]
    [InlineData(0b0010_0000, 0x00, 0x00, 0x00, 0x00)]
    [InlineData(0b1111_1111, 0xFF, 0xFF, 0xFF, 0xFF)]
    public void NOP_Executes_Correctly(byte flags, byte stackPtr, byte accumulator, byte x, byte y)
    {
        system.CPU.Registers.P = flags;
        system.CPU.Registers.SP = stackPtr;
        system.CPU.Registers.A = accumulator;
        system.CPU.Registers.X = x;
        system.CPU.Registers.Y = y;
        Machinery.Instructions.SingleByte.Operations.NOP(system.CPU);
        
        Assert.Equal(flags, (byte)system.CPU.Registers.P);
        Assert.Equal(stackPtr, system.CPU.Registers.SP);
        Assert.Equal(accumulator, system.CPU.Registers.A);
        Assert.Equal(x, system.CPU.Registers.X);
        Assert.Equal(y, system.CPU.Registers.Y);
    }
    
    [Theory]
    [InlineData(0b0000_0000, 0b0000_0000, false, true, false, false)]
    [InlineData(0b0000_0000, 0b0000_0001, false, false, true, false)]
    [InlineData(0b1000_0000, 0b0000_0000, false, true, false, true)]
    [InlineData(0b1000_0000, 0b0000_0001, false, false, true, true)]
    [InlineData(0b1111_1111, 0b1111_1111, true, false, true, true)]
    [InlineData(0b1111_1111, 0b1111_1110, true, false, false, true)]
    public void ROL_Executes_Correctly(byte input, byte result, bool negative, bool zero, bool carryOld, bool carry)
    {
        system.CPU.Registers.A = input;
        system.CPU.Registers.P.Carry = carryOld;
        Machinery.Instructions.SingleByte.Operations.ROL(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0b0000_0000, 0b0000_0000, false, true, false, false)]
    [InlineData(0b0000_0001, 0b0000_0000, false, true, false, true)]
    [InlineData(0b0000_0000, 0b1000_0000, true, false, true, false)]
    [InlineData(0b0000_0001, 0b1000_0000,true, false, true, true)]
    [InlineData(0b1111_1111, 0b1111_1111, true, false, true, true)]
    [InlineData(0b1111_1111, 0b0111_1111, false, false, false, true)]
    public void ROR_Executes_Correctly(byte input, byte result, bool negative, bool zero, bool carryOld, bool carry)
    {
        system.CPU.Registers.A = input;
        system.CPU.Registers.P.Carry = carryOld;
        Machinery.Instructions.SingleByte.Operations.ROR(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void SEC_Executes_Correctly()
    {
        system.CPU.Registers.P.Carry = false;
        Machinery.Instructions.SingleByte.Operations.SEC(system.CPU);
        
        CheckFlags(negative: false, zero: false, carry: true, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void SED_Executes_Correctly()
    {
        system.CPU.Registers.P.Decimal = false;
        Machinery.Instructions.SingleByte.Operations.SED(system.CPU);
        
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: true, overflow: false);
    }
    
    [Fact]
    public void SEI_Executes_Correctly()
    {
        system.CPU.Registers.P.Interrupt = false;
        Machinery.Instructions.SingleByte.Operations.SEI(system.CPU);
        
        CheckFlags(negative: false, zero: false, carry: false, interrupt: true, @decimal: false, overflow: false);
    }

    [Theory]
    [InlineData(0x00, 0xFF, false, true)]
    [InlineData(0xFF, 0x00, true, false)]
    [InlineData(0x7F, 0x00, false, false)]
    public void TAX_Executes_Correctly(byte src, byte dst, bool negative, bool zero)
    {
        system.CPU.Registers.A = src;
        system.CPU.Registers.X = dst;
        Machinery.Instructions.SingleByte.Operations.TAX(system.CPU);
        
        Assert.Equal(src, system.CPU.Registers.X);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0xFF, false, true)]
    [InlineData(0xFF, 0x00, true, false)]
    [InlineData(0x7F, 0x00, false, false)]
    public void TAY_Executes_Correctly(byte src, byte dst, bool negative, bool zero)
    {
        system.CPU.Registers.A = src;
        system.CPU.Registers.Y = dst;
        Machinery.Instructions.SingleByte.Operations.TAY(system.CPU);
        
        Assert.Equal(src, system.CPU.Registers.Y);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0xFF, false, true)]
    [InlineData(0xFF, 0x00, true, false)]
    [InlineData(0x7F, 0x00, false, false)]
    public void TSX_Executes_Correctly(byte src, byte dst, bool negative, bool zero)
    {
        system.CPU.Registers.SP = src;
        system.CPU.Registers.X = dst;
        Machinery.Instructions.SingleByte.Operations.TSX(system.CPU);
        
        Assert.Equal(src, system.CPU.Registers.X);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0xFF, false, true)]
    [InlineData(0xFF, 0x00, true, false)]
    [InlineData(0x7F, 0x00, false, false)]
    public void TXA_Executes_Correctly(byte src, byte dst, bool negative, bool zero)
    {
        system.CPU.Registers.X = src;
        system.CPU.Registers.A = dst;
        Machinery.Instructions.SingleByte.Operations.TXA(system.CPU);
        
        Assert.Equal(src, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0xFF)]
    [InlineData(0xFF, 0x00)]
    [InlineData(0x7F, 0x00)]
    public void TXS_Executes_Correctly(byte src, byte dst)
    {
        system.CPU.Registers.X = src;
        system.CPU.Registers.SP = dst;
        Machinery.Instructions.SingleByte.Operations.TXS(system.CPU);
        
        Assert.Equal(src, system.CPU.Registers.SP);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0xFF, false, true)]
    [InlineData(0xFF, 0x00, true, false)]
    [InlineData(0x7F, 0x00, false, false)]
    public void TYA_Executes_Correctly(byte src, byte dst, bool negative, bool zero)
    {
        system.CPU.Registers.Y = src;
        system.CPU.Registers.A = dst;
        Machinery.Instructions.SingleByte.Operations.TYA(system.CPU);
        
        Assert.Equal(src, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
}
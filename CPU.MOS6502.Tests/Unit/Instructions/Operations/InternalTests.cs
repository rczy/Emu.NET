namespace CPU.MOS6502.Tests.Unit.Instructions.Operations;

public class InternalTests : Base
{
    [Theory]
    [InlineData(0x00, 0x00, false, 0x00, false, true, false, false)]
    [InlineData(0x00, 0x00, true, 0x01, false, false, false, false)]
    [InlineData(0x00, 0xFF, false, 0xFF, true, false, false, false)]
    [InlineData(0x00, 0xFF, true, 0x00, false, true, true, false)]
    [InlineData(0xFF, 0xFF, false, 0xFE, true, false, true, false)]
    [InlineData(0xFF, 0xFF, true, 0xFF, true, false, true, false)]
    [InlineData(0x7F, 0x01, false, 0x80, true, false, false, true)]
    [InlineData(0x80, 0xFF, false, 0x7F, false, false, true, true)]
    public void ADC_BinaryMode_Executes_Correctly(byte accumulator, byte data, bool carryIn, byte result, bool negative, bool zero, bool carry, bool overflow)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        system.CPU.Registers.P.Carry = carryIn;
        system.CPU.Registers.P.Decimal = false;
        Machinery.Instructions.Internal.Operations.ADC(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow);
    }
    
    [Theory]
    [InlineData(0x00, 0x00, false, 0x00, false, true, false, false)]
    [InlineData(0x79, 0x00, true, 0x80, true, false, false, true)]
    [InlineData(0x24, 0x56, false, 0x80, true, false, false, true)]
    [InlineData(0x93, 0x82, false, 0x75, false, false, true, true)]
    [InlineData(0x89, 0x76, false, 0x65, false, false, true, false)]
    [InlineData(0x89, 0x76, true, 0x66, false, true, true, false)]
    [InlineData(0x80, 0xF0, false, 0xD0, false, false, true, true)]
    [InlineData(0x80, 0xFA, false, 0xE0, true, false, true, false)]
    [InlineData(0x2F, 0x4F, false, 0x74, false, false, false, false)]
    [InlineData(0x6F, 0x00, true, 0x76, false, false, false, false)]
    public void ADC_DecimalMode_Executes_Correctly(byte accumulator, byte data, bool carryIn, byte result, bool negative, bool zero, bool carry, bool overflow)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        system.CPU.Registers.P.Carry = carryIn;
        system.CPU.Registers.P.Decimal = true;
        Machinery.Instructions.Internal.Operations.ADC(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: true, overflow);
    }
    
    [Theory]
    [InlineData(0x00, 0x00, true, 0x00, false, true, true, false)]
    [InlineData(0x00, 0x00, false, 0xFF, true, false, false, false)]
    [InlineData(0x00, 0x01, false, 0xFE, true, false, false, false)]
    [InlineData(0x7f, 0x01, false, 0x7D, false, false, true, false)]
    [InlineData(0x80, 0x01, false, 0x7E, false, false, true, true)]
    [InlineData(0xFF, 0x01, false, 0xFD, true, false, true, false)]
    [InlineData(0x02, 0x01, false, 0x00, false, true, true, false)]
    [InlineData(0xAB, 0xCD, true, 0xDE, true, false, false, false)]
    public void SBC_BinaryMode_Executes_Correctly(byte accumulator, byte data, bool carryIn, byte result, bool negative, bool zero, bool carry, bool overflow)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        system.CPU.Registers.P.Carry = carryIn;
        system.CPU.Registers.P.Decimal = false;
        Machinery.Instructions.Internal.Operations.SBC(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow);
    }
    
    [Theory]
    [InlineData(0x00, 0x00, false, 0x99, true, false, false, false)]
    [InlineData(0x00, 0x00, true, 0x00, false, true, true, false)]
    [InlineData(0x00, 0x01, true, 0x99, true, false, false, false)]
    [InlineData(0x0A, 0x00, true, 0x0A, false, false, true, false)]
    [InlineData(0x0B, 0x00, false, 0x0A, false, false, true, false)]
    [InlineData(0x9A, 0x00, true, 0x9A, true, false, true, false)]
    [InlineData(0x9B, 0x00, false, 0x9A, true, false, true, false)]
    [InlineData(0x69, 0x42, true, 0x27, false, false, true, false)]
    public void SBC_DecimalMode_Executes_Correctly(byte accumulator, byte data, bool carryIn, byte result, bool negative, bool zero, bool carry, bool overflow)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        system.CPU.Registers.P.Carry = carryIn;
        system.CPU.Registers.P.Decimal = true;
        Machinery.Instructions.Internal.Operations.SBC(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: true, overflow);
    }

    [Theory]
    [InlineData(0b1100_1100, 0b0000_0000, 0b0000_0000, false, true)]
    [InlineData(0b1100_1100, 0b1111_1111, 0b1100_1100, true, false)]
    [InlineData(0b1100_1100, 0b0011_0011, 0b0000_0000, false, true)]
    [InlineData(0b1100_1100, 0b1100_1100, 0b1100_1100, true, false)]
    public void AND_Executes_Correctly(byte accumulator, byte data, byte result, bool negative, bool zero)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.AND(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0b1100_1100, 0b0000_0000, false, false, true)]
    [InlineData(0b1100_1100, 0b1111_1111, true, true, false)]
    [InlineData(0b1100_1100, 0b0101_0101, false, true, false)]
    [InlineData(0b0011_0011, 0b1010_1010, true, false, false)]
    public void BIT_Executes_Correctly(byte accumulator, byte data, bool negative, bool overflow, bool zero)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.BIT(system.CPU);
        
        Assert.Equal(accumulator, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: overflow);
    }
    
    [Theory]
    [InlineData(0x00, 0x00, true, false, true)]
    [InlineData(0xAB, 0xCD, false, true, false)]
    [InlineData(0xCD, 0xAB, true, false, false)]
    [InlineData(0xAB, 0xAB, true, false, true)]
    public void CMP_Executes_Correctly(byte accumulator, byte data, bool carry, bool negative, bool zero)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.CMP(system.CPU);
        
        Assert.Equal(accumulator, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0x00, true, false, true)]
    [InlineData(0xAB, 0xCD, false, true, false)]
    [InlineData(0xCD, 0xAB, true, false, false)]
    [InlineData(0xAB, 0xAB, true, false, true)]
    public void CPX_Executes_Correctly(byte accumulator, byte data, bool carry, bool negative, bool zero)
    {
        system.CPU.Registers.X = accumulator;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.CPX(system.CPU);
        
        Assert.Equal(accumulator, system.CPU.Registers.X);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0x00, true, false, true)]
    [InlineData(0xAB, 0xCD, false, true, false)]
    [InlineData(0xCD, 0xAB, true, false, false)]
    [InlineData(0xAB, 0xAB, true, false, true)]
    public void CPY_Executes_Correctly(byte accumulator, byte data, bool carry, bool negative, bool zero)
    {
        system.CPU.Registers.Y = accumulator;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.CPY(system.CPU);
        
        Assert.Equal(accumulator, system.CPU.Registers.Y);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0b1100_1100, 0b0000_0000, 0b1100_1100, true, false)]
    [InlineData(0b1100_1100, 0b1111_1111, 0b0011_0011, false, false)]
    [InlineData(0b1100_1100, 0b0011_0011, 0b1111_1111, true, false)]
    [InlineData(0b1100_1100, 0b1100_1100, 0b0000_0000, false, true)]
    public void EOR_Executes_Correctly(byte accumulator, byte data, byte result, bool negative, bool zero)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.EOR(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0x80, true, false)]
    [InlineData(0x80, 0x00, false, true)]
    [InlineData(0x80, 0x40, false, false)]
    public void LDA_Executes_Correctly(byte register, byte data, bool negative, bool zero)
    {
        system.CPU.Registers.A = register;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.LDA(system.CPU);
        
        Assert.Equal(data, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0x80, true, false)]
    [InlineData(0x80, 0x00, false, true)]
    [InlineData(0x80, 0x40, false, false)]
    public void LDX_Executes_Correctly(byte register, byte data, bool negative, bool zero)
    {
        system.CPU.Registers.X = register;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.LDX(system.CPU);
        
        Assert.Equal(data, system.CPU.Registers.X);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x00, 0x80, true, false)]
    [InlineData(0x80, 0x00, false, true)]
    [InlineData(0x80, 0x40, false, false)]
    public void LDY_Executes_Correctly(byte register, byte data, bool negative, bool zero)
    {
        system.CPU.Registers.Y = register;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.LDY(system.CPU);
        
        Assert.Equal(data, system.CPU.Registers.Y);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0b1100_1100, 0b0000_0000, 0b1100_1100, true, false)]
    [InlineData(0b1100_1100, 0b1111_1111, 0b1111_1111, true, false)]
    [InlineData(0b1100_1100, 0b0011_0011, 0b1111_1111, true, false)]
    [InlineData(0b1100_1100, 0b1100_1100, 0b1100_1100, true, false)]
    public void ORA_Executes_Correctly(byte accumulator, byte data, byte result, bool negative, bool zero)
    {
        system.CPU.Registers.A = accumulator;
        system.CPU.Data = data;
        Machinery.Instructions.Internal.Operations.ORA(system.CPU);
        
        Assert.Equal(result, system.CPU.Registers.A);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
}
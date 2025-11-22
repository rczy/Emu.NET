namespace CPU.MOS6502.Tests.Unit.Instructions.Operations;

public class ReadModifyWriteTests : Base
{
    private const ushort Address = 0x02CD;

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
        system.CPU.Data = input;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.ReadModifyWrite.Operations.ASL(system.CPU);
        
        Assert.Equal(result, system.CPU.Data);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }

    [Theory]
    [InlineData(0x02, 0x01, false, false)]
    [InlineData(0x01, 0x00, false, true)]
    [InlineData(0x00, 0xFF, true, false)]
    [InlineData(0xFF, 0xFE, true, false)]
    [InlineData(0x81, 0x80, true, false)]
    [InlineData(0x80, 0x7F, false, false)]
    public void DEC_Executes_Correctly(byte input, byte result, bool negative, bool zero)
    {
        system.CPU.Data = input;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.ReadModifyWrite.Operations.DEC(system.CPU);
        
        Assert.Equal(result, system.CPU.Data);
        CheckFlags(negative, zero, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(0x01, 0x02, false, false)]
    [InlineData(0x00, 0x01, false, false)]
    [InlineData(0xFF, 0x00, false, true)]
    [InlineData(0xFE, 0xFF, true, false)]
    [InlineData(0x80, 0x81, true, false)]
    [InlineData(0x7F, 0x80, true, false)]
    public void INC_Executes_Correctly(byte input, byte result, bool negative, bool zero)
    {
        system.CPU.Data = input;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.ReadModifyWrite.Operations.INC(system.CPU);
        
        Assert.Equal(result, system.CPU.Data);
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
        system.CPU.Data = input;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.ReadModifyWrite.Operations.LSR(system.CPU);
        
        Assert.Equal(result, system.CPU.Data);
        CheckFlags(negative: false, zero, carry, interrupt: false, @decimal: false, overflow: false);
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
        system.CPU.Data = input;
        system.CPU.Registers.P.Carry = carryOld;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.ReadModifyWrite.Operations.ROL(system.CPU);
        
        Assert.Equal(result, system.CPU.Data);
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
        system.CPU.Data = input;
        system.CPU.Registers.P.Carry = carryOld;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.ReadModifyWrite.Operations.ROR(system.CPU);
        
        Assert.Equal(result, system.CPU.Data);
        CheckFlags(negative, zero, carry, interrupt: false, @decimal: false, overflow: false);
    }
}
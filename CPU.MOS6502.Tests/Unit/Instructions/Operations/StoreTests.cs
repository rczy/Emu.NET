namespace CPU.MOS6502.Tests.Unit.Instructions.Operations;

public class StoreTests : Base
{
    private const byte Data = 0xAB;
    private const ushort Address = 0x02CD;
    
    [Fact]
    public void STA_Executes_Correctly()
    {
        system.CPU.Registers.A = Data;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.Store.Operations.STA(system.CPU);
        
        Assert.Equal(Data, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void STX_Executes_Correctly()
    {
        system.CPU.Registers.X = Data;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.Store.Operations.STX(system.CPU);
        
        Assert.Equal(Data, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void STY_Executes_Correctly()
    {
        system.CPU.Registers.Y = Data;
        system.CPU.Address.Full = Address;
        Machinery.Instructions.Store.Operations.STY(system.CPU);
        
        Assert.Equal(Data, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
}
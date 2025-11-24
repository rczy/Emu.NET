namespace CPU.MOS6502.Tests.Unit.Instructions.Operations;

public class FlowAndStackTests : Base
{
    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 0)]
    public void BCC_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Carry = flag;
        Machinery.Instructions.FlowAndStack.Operations.BCC(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: flag, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void BCS_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Carry = flag;
        Machinery.Instructions.FlowAndStack.Operations.BCS(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: flag, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void BEQ_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Zero = flag;
        Machinery.Instructions.FlowAndStack.Operations.BEQ(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: false, zero: flag, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void BMI_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Negative = flag;
        Machinery.Instructions.FlowAndStack.Operations.BMI(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: flag, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 0)]
    public void BNE_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Zero = flag;
        Machinery.Instructions.FlowAndStack.Operations.BNE(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: false, zero: flag, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 0)]
    public void BPL_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Negative = flag;
        Machinery.Instructions.FlowAndStack.Operations.BPL(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: flag, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void BRK_Executes_Correctly()
    {
        const ushort address = 0xABCD;
        system.CPU.Address.Full = address;
        Machinery.Instructions.FlowAndStack.Operations.BRK(system.CPU);
        
        Assert.Equal(address, system.CPU.Registers.PC);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 0)]
    public void BVC_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Overflow = flag;
        Machinery.Instructions.FlowAndStack.Operations.BVC(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: flag);
    }
    
    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void BVS_Executes_Correctly(bool flag, byte taken)
    {
        system.CPU.Registers.P.Overflow = flag;
        Machinery.Instructions.FlowAndStack.Operations.BVS(system.CPU);
        
        Assert.Equal(taken, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: flag);
    }
    
    [Fact]
    public void JMP_Executes_Correctly()
    {
        const ushort address = 0xABCD;
        system.CPU.Address.Full = address;
        Machinery.Instructions.FlowAndStack.Operations.JMP(system.CPU);
        
        Assert.Equal(address, system.CPU.Registers.PC);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void JSR_Executes_Correctly()
    {
        const ushort address = 0xABCD;
        system.CPU.Address.Full = address;
        Machinery.Instructions.FlowAndStack.Operations.JSR(system.CPU);
        
        Assert.Equal(address, system.CPU.Registers.PC);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void PHA_Executes_Correctly()
    {
        const byte data = 0xFF;
        system.CPU.Registers.A = data;
        Machinery.Instructions.FlowAndStack.Operations.PHA(system.CPU);
        
        Assert.Equal(data, system.CPU.Data);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void PHP_Executes_Correctly()
    {
        system.CPU.Registers.P = 0x00;
        Machinery.Instructions.FlowAndStack.Operations.PHP(system.CPU);
        
        Assert.Equal(0x30, system.CPU.Data);
        Assert.True(system.CPU.Registers.P.Break);
        Assert.True(system.CPU.Registers.P.Unused);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void PLA_Executes_Correctly()
    {
        const byte data = 0xFF;
        system.CPU.Data = data;
        Machinery.Instructions.FlowAndStack.Operations.PLA(system.CPU);
        
        Assert.Equal(data, system.CPU.Registers.A);
        CheckFlags(negative: true, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void PLP_Executes_Correctly()
    {
        const byte data = 0xFF;
        system.CPU.Data = data;
        Machinery.Instructions.FlowAndStack.Operations.PLP(system.CPU);
        
        Assert.Equal(0xEF, (byte)system.CPU.Registers.P);
        CheckFlags(negative: true, zero: true, carry: true, interrupt: true, @decimal: true, overflow: true);
    }
    
    [Fact]
    public void RTI_Executes_Correctly()
    {
        const ushort pc = 0x0200;
        system.CPU.Registers.PC = pc;
        Machinery.Instructions.FlowAndStack.Operations.RTI(system.CPU);
        
        Assert.Equal(pc, system.CPU.Registers.PC);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
    
    [Fact]
    public void RTS_Executes_Correctly()
    {
        const ushort pc = 0x0200;
        system.CPU.Registers.PC = pc;
        Machinery.Instructions.FlowAndStack.Operations.RTS(system.CPU);
        
        Assert.Equal(pc + 1, system.CPU.Registers.PC);
        CheckFlags(negative: false, zero: false, carry: false, interrupt: false, @decimal: false, overflow: false);
    }
}
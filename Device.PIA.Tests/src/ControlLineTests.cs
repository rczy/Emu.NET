using Device.PIA.Tests.Utils;

namespace Device.PIA.Tests;

public class ControlLineTests : TestBase
{
    [Fact]
    public void PIA_Resets_Correctly()
    {
        var peripherals = new[] {pA, pB};
        
        foreach (var peripheral in peripherals)
        {
            peripheral.SetC1(true);
            peripheral.SetC2(true);
            peripheral.Port!.Registers.Control.Value = 0xFF;
            peripheral.Port!.Registers.DataDirection = 0xFF;
            peripheral.Port!.Registers.Output = 0xFF;
            
            pia.Reset();
            
            Assert.False(peripheral.Port.InterruptControl.C1);
            Assert.False(peripheral.Port.InterruptControl.C2);
            Assert.False(peripheral.Port.InterruptControl.IRQ);
            Assert.Equal(0, peripheral.Port!.Registers.Control.Value);
            Assert.Equal(0, peripheral.Port!.Registers.DataDirection);
            Assert.Equal(0, peripheral.Port!.Registers.Output);
        }
    }

    [Fact]
    public void C1_TriggersInterrupt_On_LowHighTransition()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b111); // set active transition to low-high
            
            Assert.False(interruptLine());
            Assert.Equal(0, pia.Read(cr) & 0x80);
            peripheral.SetC1(false);
            peripheral.SetC1(true);
            Assert.True(interruptLine());
            Assert.NotEqual(0, pia.Read(cr) & 0x80);
        }
    }
    
    [Fact]
    public void C1_TriggersInterrupt_On_HighLowTransition()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b101); // set active transition to high-low
            
            Assert.False(interruptLine());
            Assert.Equal(0, pia.Read(cr) & 0x80);
            peripheral.SetC1(true);
            peripheral.SetC1(false);
            Assert.True(interruptLine());
            Assert.NotEqual(0, pia.Read(cr) & 0x80);
        }
    }
    
    [Fact]
    public void C2_In_InputMode_TriggersInterrupt_On_LowHighTransition()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b011100); // set active transition to low-high
            
            Assert.False(interruptLine());
            Assert.Equal(0, pia.Read(cr) & 0x40);
            peripheral.SetC2(false);
            peripheral.SetC2(true);
            Assert.True(interruptLine());
            Assert.NotEqual(0, pia.Read(cr) & 0x40);
        }
    }
    
    [Fact]
    public void C2_In_InputMode_TriggersInterrupt_On_HighLowTransition()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b001100); // set active transition to high-low
            
            Assert.False(interruptLine());
            Assert.Equal(0, pia.Read(cr) & 0x40);
            peripheral.SetC2(true);
            peripheral.SetC2(false);
            Assert.True(interruptLine());
            Assert.NotEqual(0, pia.Read(cr) & 0x40);
        }
    }
    
    [Fact]
    public void C1_Interrupt_CanBe_Disabled()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b110);
            
            Assert.False(interruptLine());
            peripheral.SetC1(false);
            peripheral.SetC1(true);
            Assert.False(interruptLine()); // interrupt is disabled
            
            pia.Write(cr, 0b111);
            Assert.True(interruptLine()); // interrupt occurs after enabled again
        }
    }
    
    [Fact]
    public void C2_Interrupt_CanBe_Disabled_In_InputMode()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b010100);
            
            Assert.False(interruptLine());
            peripheral.SetC2(false);
            peripheral.SetC2(true);
            Assert.False(interruptLine()); // interrupt is disabled
            
            pia.Write(cr, 0b011100);
            Assert.True(interruptLine()); // interrupt occurs after enabled again
        }
    }
    
    [Fact]
    public void C1_Interrupt_Cleared_When_Reading_OutputRegister()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b111);
            
            Assert.False(interruptLine());
            peripheral.SetC1(false);
            peripheral.SetC1(true);
            Assert.True(interruptLine());

            var dr = (byte)(0 + offset);
            pia.Read(dr);
            Assert.False(interruptLine()); // interrupt cleared
        }
    }
    
    [Fact]
    public void C2_Interrupt_Cleared_When_Reading_OutputRegister_In_InputMode()
    {
        (TestPeripheral, Func<bool>, int)[] testData = [(pA, () => pia.IRQA, 0), (pB, () => pia.IRQB, 2)];

        foreach (var (peripheral, interruptLine, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b011100);
            
            Assert.False(interruptLine());
            peripheral.SetC2(false);
            peripheral.SetC2(true);
            Assert.True(interruptLine());

            var dr = (byte)(0 + offset);
            pia.Read(dr);
            Assert.False(interruptLine()); // interrupt cleared
        }
    }

    [Fact]
    public void C2_CannotChange_In_OutputMode()
    {
        var testData = new [] { (pA, 0), (pB, 2) };
        
        foreach (var (peripheral, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            pia.Write(cr, 0b100100);
            
            peripheral.SetC2(true);
            peripheral.SetC2(false);
            peripheral.SetC2(true);
            Assert.Equal(0, pia.Read(cr) & 0x40);
            Assert.False(peripheral.Port!.InterruptControl.C2);
        }
    }

    [Fact]
    public void C2_In_ManualMode_Outputs_CorrectSignal()
    {
        var testData = new [] { (pA, 0), (pB, 2) };
        
        foreach (var (peripheral, offset) in testData)
        {
            var cr = (byte)(1 + offset);
            
            pia.Write(cr, 0b111100);
            Assert.Equal(0, pia.Read(cr) & 0x40);
            Assert.True(peripheral.Port!.InterruptControl.C2);
            
            pia.Write(cr, 0b110100);
            Assert.Equal(0, pia.Read(cr) & 0x40);
            Assert.False(peripheral.Port!.InterruptControl.C2);
        }
    }

    [Theory]
    [InlineData(0b100110, false, true)] // low-high transition
    [InlineData(0b100100, true, false)] // high-low transition
    public void CA2_In_HandshakeMode_With_C1ActiveTransition_Restores(byte cr, bool from, bool to)
    {
        pA.Port!.InterruptControl.C2 = true;
        pia.Write(1, cr);
        
        pia.Write(0, 0x00);
        Assert.True(pA.Port!.InterruptControl.C2);

        pia.Write(1, (byte)(cr & ~0b100)); // switch to DDRA
        pia.Read(0);
        Assert.True(pA.Port!.InterruptControl.C2);
        pia.Write(1, cr); // switch back to ORA

        pia.Read(0);
        Assert.False(pA.Port!.InterruptControl.C2);
        
        pia.Enable();
        Assert.False(pA.Port!.InterruptControl.C2);
        
        pA.SetC1(from);
        pA.SetC1(to);
        Assert.True(pA.Port!.InterruptControl.C2);
    }
    
    [Theory]
    [InlineData(0b100110, false, true)] // low-high transition
    [InlineData(0b100100, true, false)] // high-low transition
    public void CB2_In_HandshakeMode_With_C1ActiveTransition_Restores(byte cr, bool from, bool to)
    {
        pB.Port!.InterruptControl.C2 = true;
        pia.Write(3, cr);
        
        pia.Read(2);
        Assert.True(pB.Port!.InterruptControl.C2);

        pia.Write(3, (byte)(cr & ~0b100)); // switch to DDRA
        pia.Write(2, 0x00);
        Assert.True(pB.Port!.InterruptControl.C2);
        pia.Write(3, cr); // switch back to ORA

        pia.Write(2, 0x00);
        Assert.False(pB.Port!.InterruptControl.C2);
        
        pia.Enable();
        Assert.False(pB.Port!.InterruptControl.C2);
        
        pB.SetC1(from);
        pB.SetC1(to);
        Assert.True(pB.Port!.InterruptControl.C2);
    }
    
    [Fact]
    public void CA2_In_HandshakeMode_With_EnablePulse_Restores()
    {
        const byte cr = 0b101100;
        
        pA.Port!.InterruptControl.C2 = true;
        pia.Write(1, cr);
        
        pia.Write(0, 0x00);
        Assert.True(pA.Port!.InterruptControl.C2);

        pia.Write(1, cr & ~0b100); // switch to DDRA
        pia.Read(0);
        Assert.True(pA.Port!.InterruptControl.C2);
        pia.Write(1, cr); // switch back to ORA

        pia.Read(0);
        Assert.False(pA.Port!.InterruptControl.C2);
        
        pA.SetC1(true);
        pA.SetC1(false);
        pA.SetC1(true);
        Assert.False(pA.Port!.InterruptControl.C2);
        
        pia.Enable();
        Assert.True(pA.Port!.InterruptControl.C2);
    }
    
    [Fact]
    public void CB2_In_HandshakeMode_With_EnablePulse_Restores()
    {
        const byte cr = 0b101100;
        
        pB.Port!.InterruptControl.C2 = true;
        pia.Write(3, cr);
        
        pia.Read(2);
        Assert.True(pB.Port!.InterruptControl.C2);

        pia.Write(3, cr & ~0b100); // switch to DDRA
        pia.Write(2, 0x00);
        Assert.True(pB.Port!.InterruptControl.C2);
        pia.Write(3, cr); // switch back to ORA

        pia.Write(2, 0x00);
        Assert.False(pB.Port!.InterruptControl.C2);
        
        pB.SetC1(true);
        pB.SetC1(false);
        pB.SetC1(true);
        Assert.False(pB.Port!.InterruptControl.C2);
        
        pia.Enable();
        Assert.True(pB.Port!.InterruptControl.C2);
    }
}
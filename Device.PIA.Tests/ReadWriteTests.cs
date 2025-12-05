using Device.PIA.Tests.Utils;

namespace Device.PIA.Tests;

public class ReadWriteTests : TestBase
{
    [Fact]
    public void PIA_WriteTo_Register_IsCorrect()
    {
        pia.Write(0, 0xFF);
        Assert.Equal(0xFF, pA.Port!.Registers.DataDirection);
        
        pia.Write(1, 0xFF);
        Assert.Equal(0x3F, pA.Port!.Registers.Control.Value);
        
        pia.Write(0, 0xFF);
        Assert.Equal(0xFF, pA.Port!.Registers.Output);
        
        pia.Write(2, 0xFF);
        Assert.Equal(0xFF, pB.Port!.Registers.DataDirection);
        
        pia.Write(3, 0xFF);
        Assert.Equal(0x3F, pB.Port!.Registers.Control.Value);
        
        pia.Write(2, 0xFF);
        Assert.Equal(0xFF, pB.Port!.Registers.Output);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => pia.Write(4, 0xFF));
    }
    
    [Fact]
    public void PIA_ReadFrom_Register_IsCorrect()
    {
        pA.Port!.Registers.DataDirection = 0xFF;
        Assert.Equal(0xFF, pia.Read(0));
        
        pA.Port!.Registers.Control.Value = 0xCD;
        Assert.Equal(0xCD, pia.Read(1));
        
        pA.Port!.Registers.Output = 0xEF;
        Assert.Equal(0xEF, pia.Read(0));
        
        pB.Port!.Registers.DataDirection = 0xFF;
        Assert.Equal(0xFF, pia.Read(2));
        
        pB.Port!.Registers.Control.Value = 0xCD;
        Assert.Equal(0xCD, pia.Read(3));
        
        pB.Port!.Registers.Output = 0xEF;
        Assert.Equal(0xEF, pia.Read(2));
        
        Assert.Throws<ArgumentOutOfRangeException>(() => pia.Read(4));
    }

    [Fact]
    public void PIA_WriteTo_Peripheral_IsCorrect()
    {
        var testData = new[] { (0, pA), (2, pB) };

        foreach (var (offset, peripheral) in testData)
        {
            var dr = (ushort)(0 + offset);
            var cr = (ushort)(1 + offset);
            
            pia.Write(cr, 0); // select DDR (data direction register)
            pia.Write(dr, 0x00); // write to DDR
            pia.Write(cr, 4); // select OR (output register)
            pia.Write(dr, 0xFF); // write to OR
            Assert.Equal(0x00, peripheral.Data);
        
            pia.Write(cr, 0);
            pia.Write(dr, 0xFF);
            pia.Write(cr, 4);
            pia.Write(dr, 0xFF);
            Assert.Equal(0xFF, peripheral.Data);
        
            pia.Write(cr, 0);
            pia.Write(dr, 0xF0);
            pia.Write(cr, 4);
            pia.Write(dr, 0xFF);
            Assert.Equal(0xF0, peripheral.Data);
        
            pia.Write(cr, 0);
            pia.Write(dr, 0x0F);
            pia.Write(cr, 4);
            pia.Write(dr, 0xFF);
            Assert.Equal(0x0F, peripheral.Data);
            
            pia.Write(dr, 0xF0);
            Assert.Equal(0x00, peripheral.Data);
        }
    }
    
    [Fact]
    public void PIA_ReadFrom_Peripheral_IsCorrect()
    {
        var testData = new[] { (0, pA), (2, pB) };

        foreach (var (offset, peripheral) in testData)
        {
            var dr = (ushort)(0 + offset);
            var cr = (ushort)(1 + offset);
            
            pia.Write(cr, 0); // select DDR (data direction register)
            pia.Write(dr, 0x00); // write to DDR
            pia.Write(cr, 4); // select OR (output register)
            pia.Write(dr, 0x00); // write to OR
            peripheral.Data = 0xFF; // data on peripheral
            Assert.Equal(0xFF, pia.Read(dr));

            pia.Write(cr, 0);
            pia.Write(dr, 0xFF);
            pia.Write(cr, 4);
            pia.Write(dr, 0xFF);
            peripheral.Data = 0x00;
            Assert.Equal(0xFF, pia.Read(dr));

            pia.Write(cr, 0);
            pia.Write(dr, 0b1010_1010);
            pia.Write(cr, 4);
            pia.Write(dr, 0x00);
            peripheral.Data = 0b1010_1010;
            Assert.Equal(0x00, pia.Read(dr));
            pia.Write(dr, 0b1010_1010);
            peripheral.Data = 0x00;
            Assert.Equal(0b1010_1010, pia.Read(dr));
            peripheral.Data = 0b0101_0101;
            Assert.Equal(0xFF, pia.Read(dr));
        }
    }
}
using CPU.MOS6502;
using Device.PIA.Internals;

namespace Device.PIA;

public class Adapter : IDevice
{
    public bool IRQA => _portA.InterruptControl.IRQ;
    public bool IRQB => _portB.InterruptControl.IRQ;
    
    private readonly Port _portA = new(PortSection.A);
    private readonly Port _portB = new(PortSection.B);
    
    public void Connect(PortSection portSection, IPeripheral peripheral)
    {
        if (portSection == PortSection.A)
        {
            _portA.Connect(peripheral);
            peripheral.Connect(_portA);
        }
        else if (portSection == PortSection.B)
        {
            _portB.Connect(peripheral);
            peripheral.Connect(_portB);
        }
    }
    
    public void Enable()
    {
        _portA.InterruptControl.HandleC2RestoreWithEnablePulse();
        _portB.InterruptControl.HandleC2RestoreWithEnablePulse();
    }
    
    public void Reset()
    {
        _portA.Reset();
        _portB.Reset();
    }
    
    public byte Read(ushort address)
    {
        return address switch
        {
            0 => _portA.ReadData(),
            1 => _portA.ReadControlRegister(),
            2 => _portB.ReadData(),
            3 => _portB.ReadControlRegister(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public void Write(ushort address, byte data)
    {
        switch (address)
        {
            case 0: _portA.WriteData(data); break;
            case 1: _portA.WriteControlRegister(data); break;
            case 2: _portB.WriteData(data); break;
            case 3: _portB.WriteControlRegister(data); break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
namespace Device.PIA.Internals;

public enum PortSection { A, B }

public class Port
{
    public InterruptControl InterruptControl { get; }
    internal PortSection PortSection { get; }
    internal Registers Registers { get; }
    
    private IPeripheral? _peripheral;
    
    public Port(PortSection portSection)
    {
        PortSection = portSection;
        Registers = new Registers();
        InterruptControl = new InterruptControl(this);
    }
    
    internal void Reset()
    {
        Registers.Output = Registers.DataDirection = Registers.Control.Value = 0;
        _peripheral?.Reset();
        InterruptControl.Reset();
    }
    
    internal void Connect(IPeripheral peripheral)
    {
        _peripheral = peripheral;
    }
    
    internal byte ReadData()
    {
        if (Registers.Control.OutputRegisterSelected)
        {
            Registers.Control.IRQ1 = false;
            Registers.Control.IRQ2 = false;
            InterruptControl.HandleC2Handshake(readNotWrite: true);
            var input = (_peripheral?.OnRead() ?? 0x00) & ~Registers.DataDirection;
            var output = Registers.Output & Registers.DataDirection;
            return (byte)(input | output);
        }
        return Registers.DataDirection;
    }
    
    internal void WriteData(byte data)
    {
        if (Registers.Control.OutputRegisterSelected)
        {
            InterruptControl.HandleC2Handshake(readNotWrite: false);
            Registers.Output = data;
            _peripheral?.OnWrite((byte)(Registers.Output & Registers.DataDirection));
            return;
        }
        Registers.DataDirection = data;
    }
    
    internal byte ReadControlRegister()
    {
        return Registers.Control.Value;
    }
    
    internal void WriteControlRegister(byte data)
    {
        Registers.Control.Value = (byte)(Registers.Control.Value & 0xC0 | data & 0x3F);
        InterruptControl.HandleC2ManualSet();
    }
}
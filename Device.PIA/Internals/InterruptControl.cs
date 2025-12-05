namespace Device.PIA.Internals;

public class InterruptControl(Port port)
{
    public bool C1 { get => _c1; set => ChangeC1(value); }
    public bool C2 { get => _c2; set => ChangeC2(value); }
    public bool IRQ => port.Registers.Control is { C1InterruptEnabled: true, IRQ1: true } or { C2InterruptEnabled: true, IRQ2: true };
    
    private bool _lastC1;
    private bool _lastC2;
    private bool _c1;
    private bool _c2;
    
    internal void Reset()
    {
        _lastC1 = _lastC2 = _c1 = _c2 = false;
    }
    
    internal void HandleC2Handshake(bool readNotWrite)
    {
        if (!port.Registers.Control.C2InHandshakeMode) return;
        
        if ((port.PortSection == PortSection.A && readNotWrite) || (port.PortSection == PortSection.B && !readNotWrite))
            SetC2(false);
    }

    internal void HandleC2ManualSet()
    {
        if (!port.Registers.Control.C2InManualMode) return;
        
        SetC2(port.Registers.Control.Bit3);
    }

    internal void HandleC2RestoreWithEnablePulse()
    {
        if (port.Registers.Control is { C2InHandshakeMode: true, C2StrobeWithC1Restore: false })
            SetC2(true);
    }
    
    private void ChangeC1(bool value)
    {
        SetC1(value);
        
        if (!DetectC1Transition()) return;
        
        port.Registers.Control.IRQ1 = true;
        
        if (port.Registers.Control is { C2InHandshakeMode: true, C2StrobeWithC1Restore: true })
            SetC2(true);
    }
    
    private void ChangeC2(bool value)
    {
        if (port.Registers.Control.C2IsOutput) return; // change only in input mode
        
        SetC2(value);

        if (!DetectC2Transition()) return;
        
        port.Registers.Control.IRQ2 = true;
    }
    
    private bool DetectC1Transition()
    {
        return port.Registers.Control.C1DetectLoHiTransition ? !_lastC1 && _c1 : _lastC1 && !_c1;
    }
    
    private bool DetectC2Transition()
    {
        return port.Registers.Control.C2DetectLoHiTransition ? !_lastC2 && _c2 : _lastC2 && !_c2;
    }
    
    private void SetC1(bool value)
    {
        _lastC1 = _c1;
        _c1 = value;
    }
    
    private void SetC2(bool value)
    {
        _lastC2 = _c2;
        _c2 = value;
    }
}
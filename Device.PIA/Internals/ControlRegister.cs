namespace Device.PIA.Internals;

public class ControlRegister
{
    internal byte Value { get; set; }

    private bool Bit0 => (Value & 0x01) != 0; // 0: disables interrupt by C1; 1: enables interrupt by C1
    private bool Bit1 => (Value & 0x02) != 0; // 0: detect C1 hi-lo transition; 1: detect C1 lo-hi transition
    private bool Bit2 => (Value & 0x04) != 0; // 0: data direction register selected; 1: output register selected
    internal bool Bit3 => (Value & 0x08) != 0; // used by C2
    private bool Bit4 => (Value & 0x10) != 0; // used by C2
    private bool Bit5 => (Value & 0x20) != 0; // 0: C2 is input; 1: C2 is output
    private bool Bit6 => (Value & 0x40) != 0; // goes high when C2 active transition occured in input mode (auto cleared)
    private bool Bit7 => (Value & 0x80) != 0; // goes high when C1 active transition occured (auto cleared)
    
    internal bool OutputRegisterSelected => Bit2;
    internal bool C1InterruptEnabled => Bit0;
    internal bool C1DetectLoHiTransition => Bit1;
    internal bool C2InterruptEnabled => Bit3;
    internal bool C2DetectLoHiTransition => Bit4;
    internal bool C2IsOutput => Bit5;
    internal bool C2InManualMode => Bit5 && Bit4;
    internal bool C2InHandshakeMode => Bit5 && !Bit4;
    internal bool C2StrobeWithC1Restore => !Bit3;
    
    internal bool IRQ1
    {
        get => Bit7;
        set => Value = (byte)(value ? Value | 0x80 : Value & 0x7F);
    }
    
    internal bool IRQ2
    {
        get => Bit6;
        set => Value = (byte)(value ? Value | 0x40 : Value & 0xBF);
    }
}
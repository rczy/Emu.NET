namespace CPU.MOS6502.Machinery;

public class Flags
{
    public bool Negative { get; internal set; }
    public bool Overflow { get; internal set; }
    public bool Unused => true;
    public bool Break { get; internal set; }
    public bool Decimal { get; internal set; }
    public bool Interrupt { get; internal set; }
    public bool Zero { get; internal set; }
    public bool Carry { get; internal set; }

    public static implicit operator byte(Flags f)
    {
        byte b = 0;
        b |= (byte)(f.Negative ? 0b1000_0000 : 0);
        b |= (byte)(f.Overflow ? 0b0100_0000 : 0);
        b |= 0b0010_0000;
        b |= (byte)(f.Break ? 0b0001_0000 : 0);
        b |= (byte)(f.Decimal ? 0b0000_1000 : 0);
        b |= (byte)(f.Interrupt ? 0b0000_0100 : 0);
        b |= (byte)(f.Zero ? 0b0000_0010 : 0);
        b |= (byte)(f.Carry ? 0b0000_0001 : 0);
        return b;
    }

    public static implicit operator Flags(byte b) => new()
    {
        Negative = (b & 0b1000_0000) != 0,
        Overflow = (b & 0b0100_0000) != 0,
        Break = (b & 0b0001_0000) != 0,
        Decimal = (b & 0b0000_1000) != 0,
        Interrupt = (b & 0b0000_0100) != 0,
        Zero = (b & 0b0000_0010) != 0,
        Carry = (b & 0b0000_0001) != 0,
    };
}
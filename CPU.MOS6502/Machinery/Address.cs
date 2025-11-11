namespace CPU.MOS6502.Machinery;

public class Address
{
    public ushort Full { get; internal set; }

    public byte Low
    {
        get => (byte)Full;
        internal set => Full = (ushort)((Full & 0xFF00) | value);
    }

    public byte High
    {
        get => (byte)(Full >>> 8);
        internal set => Full = (ushort)((Full & 0x00FF) | (value << 8));
    }

    public static implicit operator ushort(Address a) => a.Full;
    public static implicit operator Address(ushort u) => new() { Full = u };
}
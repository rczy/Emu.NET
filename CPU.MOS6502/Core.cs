namespace CPU.MOS6502;

using CPU.MOS6502.Machinery;

public class Core
{
    public Registers Registers { get; }
    public Signals Signals { get; }
    public Decoder Decoder { get; }
    public InterruptHandler InterruptHandler { get; }

    internal Bus Bus { get; }
    public int Cycles { get; private set; }

    public byte Data { get; internal set; }
    public Address Address { get; } = new();
    public Address BaseAddress { get; } = new();
    public Address IndirectAddress { get; } = new();

    public Core(Bus bus)
    {
        Bus = bus;
        Registers = new Registers();
        Signals = new Signals();
        Decoder = new Decoder(this);
        InterruptHandler = new InterruptHandler(this);
    }

    public void Tick()
    {
        Decoder.ExecuteSequence();
        Cycles = Signals.SYNC ? 0 : Cycles + 1;
    }
}
using CPU.MOS6502.Internals;
using CPU.MOS6502.Internals.Instructions;

namespace CPU.MOS6502;

public class Core
{
    public Registers Registers { get; }
    public Signals Signals { get; }
    public Decoder Decoder { get; }

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
    }

    public void Tick()
    {
        Decoder.ExecuteInstruction();
        Cycles = Signals.SYNC ? 0 : Cycles + 1;
    }
}
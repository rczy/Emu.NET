namespace Device.PIA.Internals;

public class Registers
{
    public byte Output { get; internal set; }
    public byte DataDirection { get; internal set; } // 0: input; 1: output
    public ControlRegister Control { get; } = new();
}
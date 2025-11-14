namespace CPU.MOS6502.Machinery;

using Instructions;

public class Decoder
{
    public record Instruction(string Mnemonic, string Addressing, Steps Steps, Operation Op);
    private Instruction[] InstructionTable { get; } = new Instruction[256];

    private Core Cpu { get; }
    public byte OpCode { get; private set; }
    public Instruction CurrentInstruction => InstructionTable[OpCode];

    public Decoder(Core cpu)
    {
        Cpu = cpu;
        LoadInstructionTable();
    }

    internal void ExecuteSequence()
    {
        Cpu.InterruptHandler.DetectNMI();

        if (Cpu.Signals.SYNC) // opcode fetch (T0)
        {
            if (Cpu.InterruptHandler.Poll())
            {
                Cpu.Bus.Read(Cpu.Registers.PC);
                OpCode = 0x00; // force BRK
            }
            else
            {
                OpCode = Cpu.Bus.Read(Cpu.Registers.PC++);
            }
            Cpu.Signals.SYNC = false;
            return;
        }

        Cpu.Signals.SYNC = CurrentInstruction.Steps(Cpu, CurrentInstruction.Op);
    }

    internal void AddInstruction(byte opcode, string mnemonic, string addressing, Operation op, Steps steps)
    {
        InstructionTable[opcode] = new Instruction(mnemonic, addressing, steps, op);
    }

    private void LoadInstructionTable()
    {
        for (int i = 0; i < InstructionTable.Length; i++)
            AddInstruction((byte)i, "???", "Implied", Instructions.SingleByte.Operations.UNKNOWN, Instructions.SingleByte.Execution.Implied);
    }
}
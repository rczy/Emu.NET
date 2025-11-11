namespace CPU.MOS6502.Machinery.Instructions;

public class Decoder
{
    public record Instruction(string Mnemonic, string Addressing, Steps Steps, Operation Op);
    private Instruction[] InstructionTable { get; } = new Instruction[256];

    private Core CPU { get; }
    public byte OpCode { get; private set; }
    public Instruction CurrentInstruction
    {
        get => InstructionTable[OpCode];
    }

    public Decoder(Core cpu)
    {
        CPU = cpu;
        LoadInstructionTable();
    }

    internal void ExecuteInstruction()
    {
        if (CPU.Signals.SYNC) // fetch OP CODE
        {
            OpCode = CPU.Bus.Read(CPU.Registers.PC++);
            CPU.Signals.SYNC = false;
            return;
        }

        CPU.Signals.SYNC = CurrentInstruction.Steps(CPU, CurrentInstruction.Op);
    }

    internal void AddInstruction(byte opcode, string mnemonic, string addressing, Operation op, Steps steps)
    {
        InstructionTable[opcode] = new Instruction(mnemonic, addressing, steps, op);
    }

    private void LoadInstructionTable()
    {
        for (int i = 0; i < InstructionTable.Length; i++)
            AddInstruction((byte)i, "???", "Implied", SingleByte.Operations.UNKNOWN, SingleByte.Execution.Implied);
    }
}
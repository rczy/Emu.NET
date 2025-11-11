namespace CPU.MOS6502.Machinery.Instructions.FlowAndStack;

static class Operations
{
    public static void JSR(Core cpu)
    {
        cpu.Registers.PC = cpu.Address;
    }

    public static void RTS(Core cpu)
    {
        cpu.Registers.PC++;
    }

    public static void JMP(Core cpu)
    {
        cpu.Registers.PC = cpu.Address;
    }
}
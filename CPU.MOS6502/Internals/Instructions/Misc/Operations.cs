namespace CPU.MOS6502.Internals.Instructions.Misc;

static class Operations
{
    public static void JSR(Core cpu)
    {
        cpu.Registers.PC = cpu.Address;
    }
}
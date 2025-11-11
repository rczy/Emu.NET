namespace CPU.MOS6502.Machinery.Instructions.SingleByte;

static class Execution
{
    public static bool Implied(Core cpu, Operation op) // 2 cycles
    {
        cpu.Data = cpu.Bus.Read(cpu.Registers.PC); // dummy read
        op(cpu);
        return true;
    }
}
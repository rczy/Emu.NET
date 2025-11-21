namespace CPU.MOS6502.Machinery.Instructions.Store;

static class Operations
{
    public static void STA(Core cpu)
    {
        cpu.Bus.Write(cpu.Address, cpu.Registers.A);
    }
    
    public static void STX(Core cpu)
    {
        cpu.Bus.Write(cpu.Address, cpu.Registers.X);
    }
    
    public static void STY(Core cpu)
    {
        cpu.Bus.Write(cpu.Address, cpu.Registers.Y);
    }
}
namespace CPU.MOS6502.Machinery;

using Instructions;

using FST = Instructions.FlowAndStack;
using INT = Instructions.Internal;
using RMW = Instructions.ReadModifyWrite;
using SBT = Instructions.SingleByte;
using STR = Instructions.Store;

public class Decoder
{
    public record Instruction(Steps Steps, Operation Op);
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

    internal void AddInstruction(byte opcode, Operation op, Steps steps)
    {
        InstructionTable[opcode] = new Instruction(steps, op);
    }

    private void LoadInstructionTable()
    {
        for (int i = 0; i < InstructionTable.Length; i++)
            AddInstruction((byte)i, SBT.Operations.XXX, SBT.Execution.Implied);
        
        AddInstruction(0x00, FST.Operations.BRK, FST.Execution.Break);
        AddInstruction(0x01, INT.Operations.ORA, INT.Execution.IndirectX);
        AddInstruction(0x05, INT.Operations.ORA, INT.Execution.ZeroPage);
        AddInstruction(0x06, RMW.Operations.ASL, RMW.Execution.ZeroPage);
        AddInstruction(0x08, FST.Operations.PHP, FST.Execution.Push);
        AddInstruction(0x09, INT.Operations.ORA, INT.Execution.Immediate);
        AddInstruction(0x0A, SBT.Operations.ASL, SBT.Execution.Implied);
        AddInstruction(0x0D, INT.Operations.ORA, INT.Execution.Absolute);
        AddInstruction(0x0E, RMW.Operations.ASL, RMW.Execution.Absolute);
        
        AddInstruction(0x10, FST.Operations.BPL, FST.Execution.Branch);
        AddInstruction(0x11, INT.Operations.ORA, INT.Execution.IndirectY);
        AddInstruction(0x15, INT.Operations.ORA, INT.Execution.ZeroPageX);
        AddInstruction(0x16, RMW.Operations.ASL, RMW.Execution.ZeroPageX);
        AddInstruction(0x18, SBT.Operations.CLC, SBT.Execution.Implied);
        AddInstruction(0x19, INT.Operations.ORA, INT.Execution.AbsoluteY);
        AddInstruction(0x1D, INT.Operations.ORA, INT.Execution.AbsoluteX);
        AddInstruction(0x1E, RMW.Operations.ASL, RMW.Execution.AbsoluteX);
        
        AddInstruction(0x20, FST.Operations.JSR, FST.Execution.JumpToSubroutine);
        AddInstruction(0x21, INT.Operations.AND, INT.Execution.IndirectX);
        AddInstruction(0x24, INT.Operations.BIT, INT.Execution.ZeroPage);
        AddInstruction(0x25, INT.Operations.AND, INT.Execution.ZeroPage);
        AddInstruction(0x26, RMW.Operations.ROL, RMW.Execution.ZeroPage);
        AddInstruction(0x28, FST.Operations.PLP, FST.Execution.Pull);
        AddInstruction(0x29, INT.Operations.AND, INT.Execution.Immediate);
        AddInstruction(0x2A, SBT.Operations.ROL, SBT.Execution.Implied);
        AddInstruction(0x2C, INT.Operations.BIT, INT.Execution.Absolute);
        AddInstruction(0x2D, INT.Operations.AND, INT.Execution.Absolute);
        AddInstruction(0x2E, RMW.Operations.ROL, RMW.Execution.Absolute);
        
        AddInstruction(0x30, FST.Operations.BMI, FST.Execution.Branch);
        AddInstruction(0x31, INT.Operations.AND, INT.Execution.IndirectY);
        AddInstruction(0x35, INT.Operations.AND, INT.Execution.ZeroPageX);
        AddInstruction(0x36, RMW.Operations.ROL, RMW.Execution.ZeroPageX);
        AddInstruction(0x38, SBT.Operations.SEC, SBT.Execution.Implied);
        AddInstruction(0x39, INT.Operations.AND, INT.Execution.AbsoluteY);
        AddInstruction(0x3D, INT.Operations.AND, INT.Execution.AbsoluteX);
        AddInstruction(0x3E, RMW.Operations.ROL, RMW.Execution.AbsoluteX);
        
        AddInstruction(0x40, FST.Operations.RTI, FST.Execution.ReturnFromInterrupt);
        AddInstruction(0x41, INT.Operations.EOR, INT.Execution.IndirectX);
        AddInstruction(0x45, INT.Operations.EOR, INT.Execution.ZeroPage);
        AddInstruction(0x46, RMW.Operations.LSR, RMW.Execution.ZeroPage);
        AddInstruction(0x48, FST.Operations.PHA, FST.Execution.Push);
        AddInstruction(0x49, INT.Operations.EOR, INT.Execution.Immediate);
        AddInstruction(0x4A, SBT.Operations.LSR, SBT.Execution.Implied);
        AddInstruction(0x4C, FST.Operations.JMP, FST.Execution.JumpAbsolute);
        AddInstruction(0x4D, INT.Operations.EOR, INT.Execution.Absolute);
        AddInstruction(0x4E, RMW.Operations.LSR, RMW.Execution.Absolute);
        
        AddInstruction(0x50, FST.Operations.BVC, FST.Execution.Branch);
        AddInstruction(0x51, INT.Operations.EOR, INT.Execution.IndirectY);
        AddInstruction(0x55, INT.Operations.EOR, INT.Execution.ZeroPageX);
        AddInstruction(0x56, RMW.Operations.LSR, RMW.Execution.ZeroPageX);
        AddInstruction(0x58, SBT.Operations.CLI, SBT.Execution.Implied);
        AddInstruction(0x59, INT.Operations.EOR, INT.Execution.AbsoluteY);
        AddInstruction(0x5D, INT.Operations.EOR, INT.Execution.AbsoluteX);
        AddInstruction(0x5E, RMW.Operations.LSR, RMW.Execution.AbsoluteX);
        
        AddInstruction(0x60, FST.Operations.RTS, FST.Execution.ReturnFromSubroutine);
        AddInstruction(0x61, INT.Operations.ADC, INT.Execution.IndirectX);
        AddInstruction(0x65, INT.Operations.ADC, INT.Execution.ZeroPage);
        AddInstruction(0x66, RMW.Operations.ROR, RMW.Execution.ZeroPage);
        AddInstruction(0x68, FST.Operations.PLA, FST.Execution.Pull);
        AddInstruction(0x69, INT.Operations.ADC, INT.Execution.Immediate);
        AddInstruction(0x6A, SBT.Operations.ROR, SBT.Execution.Implied);
        AddInstruction(0x6C, FST.Operations.JMP, FST.Execution.JumpIndirect);
        AddInstruction(0x6D, INT.Operations.ADC, INT.Execution.Absolute);
        AddInstruction(0x6E, RMW.Operations.ROR, RMW.Execution.Absolute);
        
        AddInstruction(0x70, FST.Operations.BVS, FST.Execution.Branch);
        AddInstruction(0x71, INT.Operations.ADC, INT.Execution.IndirectY);
        AddInstruction(0x75, INT.Operations.ADC, INT.Execution.ZeroPageX);
        AddInstruction(0x76, RMW.Operations.ROR, RMW.Execution.ZeroPageX);
        AddInstruction(0x78, SBT.Operations.SEI, SBT.Execution.Implied);
        AddInstruction(0x79, INT.Operations.ADC, INT.Execution.AbsoluteY);
        AddInstruction(0x7D, INT.Operations.ADC, INT.Execution.AbsoluteX);
        AddInstruction(0x7E, RMW.Operations.ROR, RMW.Execution.AbsoluteX);
        
        AddInstruction(0x81, STR.Operations.STA, STR.Execution.IndirectX);
        AddInstruction(0x84, STR.Operations.STY, STR.Execution.ZeroPage);
        AddInstruction(0x85, STR.Operations.STA, STR.Execution.ZeroPage);
        AddInstruction(0x86, STR.Operations.STX, STR.Execution.ZeroPage);
        AddInstruction(0x88, SBT.Operations.DEY, SBT.Execution.Implied);
        AddInstruction(0x8A, SBT.Operations.TXA, SBT.Execution.Implied);
        AddInstruction(0x8C, STR.Operations.STY, STR.Execution.Absolute);
        AddInstruction(0x8D, STR.Operations.STA, STR.Execution.Absolute);
        AddInstruction(0x8E, STR.Operations.STX, STR.Execution.Absolute);
        
        AddInstruction(0x90, FST.Operations.BCC, FST.Execution.Branch);
        AddInstruction(0x91, STR.Operations.STA, STR.Execution.IndirectY);
        AddInstruction(0x94, STR.Operations.STY, STR.Execution.ZeroPageX);
        AddInstruction(0x95, STR.Operations.STA, STR.Execution.ZeroPageX);
        AddInstruction(0x96, STR.Operations.STX, STR.Execution.ZeroPageY);
        AddInstruction(0x98, SBT.Operations.TYA, SBT.Execution.Implied);
        AddInstruction(0x99, STR.Operations.STA, STR.Execution.AbsoluteY);
        AddInstruction(0x9A, SBT.Operations.TXS, SBT.Execution.Implied);
        AddInstruction(0x9D, STR.Operations.STA, STR.Execution.AbsoluteX);
        
        AddInstruction(0xA0, INT.Operations.LDY, INT.Execution.Immediate);
        AddInstruction(0xA1, INT.Operations.LDA, INT.Execution.IndirectX);
        AddInstruction(0xA2, INT.Operations.LDX, INT.Execution.Immediate);
        AddInstruction(0xA4, INT.Operations.LDY, INT.Execution.ZeroPage);
        AddInstruction(0xA5, INT.Operations.LDA, INT.Execution.ZeroPage);
        AddInstruction(0xA6, INT.Operations.LDX, INT.Execution.ZeroPage);
        AddInstruction(0xA8, SBT.Operations.TAY, SBT.Execution.Implied);
        AddInstruction(0xA9, INT.Operations.LDA, INT.Execution.Immediate);
        AddInstruction(0xAA, SBT.Operations.TAX, SBT.Execution.Implied);
        AddInstruction(0xAC, INT.Operations.LDY, INT.Execution.Absolute);
        AddInstruction(0xAD, INT.Operations.LDA, INT.Execution.Absolute);
        AddInstruction(0xAE, INT.Operations.LDX, INT.Execution.Absolute);
        
        AddInstruction(0xB0, FST.Operations.BCS, FST.Execution.Branch);
        AddInstruction(0xB1, INT.Operations.LDA, INT.Execution.IndirectY);
        AddInstruction(0xB4, INT.Operations.LDY, INT.Execution.ZeroPageX);
        AddInstruction(0xB5, INT.Operations.LDA, INT.Execution.ZeroPageX);
        AddInstruction(0xB6, INT.Operations.LDX, INT.Execution.ZeroPageY);
        AddInstruction(0xB8, SBT.Operations.CLV, SBT.Execution.Implied);
        AddInstruction(0xB9, INT.Operations.LDA, INT.Execution.AbsoluteY);
        AddInstruction(0xBA, SBT.Operations.TSX, SBT.Execution.Implied);
        AddInstruction(0xBC, INT.Operations.LDY, INT.Execution.AbsoluteX);
        AddInstruction(0xBD, INT.Operations.LDA, INT.Execution.AbsoluteX);
        AddInstruction(0xBE, INT.Operations.LDX, INT.Execution.AbsoluteY);
        
        AddInstruction(0xC0, INT.Operations.CPY, INT.Execution.Immediate);
        AddInstruction(0xC1, INT.Operations.CMP, INT.Execution.IndirectX);
        AddInstruction(0xC4, INT.Operations.CPY, INT.Execution.ZeroPage);
        AddInstruction(0xC5, INT.Operations.CMP, INT.Execution.ZeroPage);
        AddInstruction(0xC6, RMW.Operations.DEC, RMW.Execution.ZeroPage);
        AddInstruction(0xC8, SBT.Operations.INY, SBT.Execution.Implied);
        AddInstruction(0xC9, INT.Operations.CMP, INT.Execution.Immediate);
        AddInstruction(0xCA, SBT.Operations.DEX, SBT.Execution.Implied);
        AddInstruction(0xCC, INT.Operations.CPY, INT.Execution.Absolute);
        AddInstruction(0xCD, INT.Operations.CMP, INT.Execution.Absolute);
        AddInstruction(0xCE, RMW.Operations.DEC, RMW.Execution.Absolute);
        
        AddInstruction(0xD0, FST.Operations.BNE, FST.Execution.Branch);
        AddInstruction(0xD1, INT.Operations.CMP, INT.Execution.IndirectY);
        AddInstruction(0xD5, INT.Operations.CMP, INT.Execution.ZeroPageX);
        AddInstruction(0xD6, RMW.Operations.DEC, RMW.Execution.ZeroPageX);
        AddInstruction(0xD8, SBT.Operations.CLD, SBT.Execution.Implied);
        AddInstruction(0xD9, INT.Operations.CMP, INT.Execution.AbsoluteY);
        AddInstruction(0xDD, INT.Operations.CMP, INT.Execution.AbsoluteX);
        AddInstruction(0xDE, RMW.Operations.DEC, RMW.Execution.AbsoluteX);
        
        AddInstruction(0xE0, INT.Operations.CPX, INT.Execution.Immediate);
        AddInstruction(0xE1, INT.Operations.SBC, INT.Execution.IndirectX);
        AddInstruction(0xE4, INT.Operations.CPX, INT.Execution.ZeroPage);
        AddInstruction(0xE5, INT.Operations.SBC, INT.Execution.ZeroPage);
        AddInstruction(0xE6, RMW.Operations.INC, RMW.Execution.ZeroPage);
        AddInstruction(0xE8, SBT.Operations.INX, SBT.Execution.Implied);
        AddInstruction(0xE9, INT.Operations.SBC, INT.Execution.Immediate);
        AddInstruction(0xEA, SBT.Operations.NOP, SBT.Execution.Implied);
        AddInstruction(0xEC, INT.Operations.CPX, INT.Execution.Absolute);
        AddInstruction(0xED, INT.Operations.SBC, INT.Execution.Absolute);
        AddInstruction(0xEE, RMW.Operations.INC, RMW.Execution.Absolute);
        
        AddInstruction(0xF0, FST.Operations.BEQ, FST.Execution.Branch);
        AddInstruction(0xF1, INT.Operations.SBC, INT.Execution.IndirectY);
        AddInstruction(0xF5, INT.Operations.SBC, INT.Execution.ZeroPageX);
        AddInstruction(0xF6, RMW.Operations.INC, RMW.Execution.ZeroPageX);
        AddInstruction(0xF8, SBT.Operations.SED, SBT.Execution.Implied);
        AddInstruction(0xF9, INT.Operations.SBC, INT.Execution.AbsoluteY);
        AddInstruction(0xFD, INT.Operations.SBC, INT.Execution.AbsoluteX);
        AddInstruction(0xFE, RMW.Operations.INC, RMW.Execution.AbsoluteX);
    }
}
// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using CPU.MOS6502;

int carry = 34;
int result = 255 + carry;

Console.WriteLine(result);
Console.WriteLine((byte)result);
Console.WriteLine((byte)(result & 0xFF));


Core cpu = new(new Bus());

cpu.Address.Full = 0xABCD;

Console.WriteLine($"ADDR: {cpu.Address.Full:X4} LO: {cpu.Address.Low:X2} HI: {cpu.Address.High:X2}");

cpu.Address.High = 0x12;
cpu.Address.Low = 0x34;
Console.WriteLine($"ADDR: {(ushort)cpu.Address:X4} LO: {cpu.Address.Low:X2} HI: {cpu.Address.High:X2}");

cpu.Address.High = 0x00;
cpu.Address.Low = 0xFF;
cpu.Address.Low += 3;
Console.WriteLine($"ADDR: {(ushort)cpu.Address:X4} HI: {cpu.Address.High:X2} LO: {cpu.Address.Low:X2}");

cpu.Address.High = 0x00;
cpu.Address.Low = 0xFF;
cpu.Address.Full++;
Console.WriteLine($"ADDR: {(ushort)cpu.Address:X4} HI: {cpu.Address.High:X2} LO: {cpu.Address.Low:X2}");

/*
Stopwatch sw = Stopwatch.StartNew();
int i;
for (i = 0; i < 2000000; i++)
{
    //Console.WriteLine($"tick: {i} cycles: {cpu.Cycles}");
    cpu.Tick();
}
sw.Stop();
Console.WriteLine($"tick: {i} cycles: {cpu.Cycles} elapsed: {sw.Elapsed.TotalMilliseconds} ms");
*/

Console.WriteLine($"Sequence: {cpu.Decoder.CurrentInstruction.Steps.Method.Name}, operation: {cpu.Decoder.CurrentInstruction.Op.Method.Name}");
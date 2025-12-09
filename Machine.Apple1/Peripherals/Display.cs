using Device.PIA;
using Device.PIA.Internals;

namespace Machine.Apple1.Peripherals;

public class Display : IPeripheral
{
    public Display()
    {
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Green;
    }
    
    public void Connect(Port port)
    {
    }
    
    public void Reset()
    {
        Console.Clear();
    }
    
    public byte OnRead()
    {
        return 0; // do nothing;
    }
    
    public void OnWrite(byte data)
    {
        if (data == (byte)'\r')
        {
            Console.WriteLine();
        }
        else if (data >= 32)
        {
            if (Console.CursorLeft == 40)
                Console.WriteLine();
            if (data > 95)
                data -= 32;
            Console.Write((char)data);
        }
    }
}
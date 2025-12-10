using Device.PIA;
using Device.PIA.Internals;

namespace Machine.Apple1.Peripherals;

public class Display : IPeripheral
{
    private readonly ConsoleColor _originalColor;
    private readonly System.Timers.Timer _cursorTimer = new();
    private bool _isCursorVisible;
    
    public Display()
    {
        Console.CursorVisible = false;
        _originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        
        _cursorTimer.Interval = 500;
        _cursorTimer.AutoReset = true;
        _cursorTimer.Enabled = true;
        _cursorTimer.Elapsed += (_, _) => ToggleCursor();
    }
    
    public void Connect(Port port)
    {
    }
    
    public void Reset()
    {
        Console.Clear();
    }

    public void Restore()
    {
        Console.ForegroundColor = _originalColor;
    }
    
    public byte OnRead()
    {
        return 0; // do nothing;
    }
    
    public void OnWrite(byte data)
    {
        if (_isCursorVisible)
            ToggleCursor();
        
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
    
    private void ToggleCursor()
    {
        _isCursorVisible = !_isCursorVisible;
        Console.Write(_isCursorVisible ? '@' : ' ');
        Console.CursorLeft--;
    }
}
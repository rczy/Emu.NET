using Device.PIA;
using Device.PIA.Internals;

namespace Machine.Apple1.Peripherals;

public class Display : IPeripheral
{
    private readonly ConsoleColor _originalColor;
    private readonly System.Diagnostics.Stopwatch _cursorTimer = new();
    private bool _isCursorVisible;
    
    public Display()
    {
        Console.CursorVisible = false;
        
        _originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
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
    
    public void HandleCursorBlinking()
    {
        if (!_cursorTimer.IsRunning)
        {
            _cursorTimer.Start();
            return;
        }
        
        if (_cursorTimer.ElapsedMilliseconds < 500)
        {
            return;
        }
        
        _cursorTimer.Stop();
        _cursorTimer.Reset();
        ToggleCursor();
    }
    
    private void ToggleCursor()
    {
        _isCursorVisible = !_isCursorVisible;
        Console.Write(_isCursorVisible ? '@' : ' ');
        Console.CursorLeft -= Console.CursorLeft > 0 ? 1 : 0;
    }
}
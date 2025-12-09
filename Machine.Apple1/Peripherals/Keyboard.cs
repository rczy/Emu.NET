using Device.PIA;
using Device.PIA.Internals;

namespace Machine.Apple1.Peripherals;

public class Keyboard : IPeripheral
{
    private Port? _port;
    private ConsoleKeyInfo? _keyInfo;
    
    public void Connect(Port port)
    {
        _port = port;
    }
    
    public void Reset()
    {
        // do nothing
    }
    
    public byte OnRead()
    {
        byte ascii = (_keyInfo?.Key) switch
        {
            ConsoleKey.Enter => (byte)'\r',
            ConsoleKey.Backspace => (byte)'_',
            null => 0,
            _ => (byte)char.ToUpper(_keyInfo?.KeyChar ?? ' '),
        };
        
        return (byte)(ascii | 0x80);
    }
    
    public void OnWrite(byte data)
    {
        // do nothing
    }
    
    public void HandleKeypress(ref EmulationState status)
    {
        if (!Console.KeyAvailable) return;
        
        while (Console.KeyAvailable)
            _keyInfo = Console.ReadKey(intercept: true);

        switch (_keyInfo?.Key)
        {
            case ConsoleKey.F4:
                status = EmulationState.Stopped;
                break;
            case ConsoleKey.F5:
                status = EmulationState.ResetRequested;
                break;
            default:
            {
                if (_port is not null) // strobe PIA
                {
                    _port.InterruptControl.C1 = true;
                    _port.InterruptControl.C1 = false;
                }
                break;
            }
        }
    }
}
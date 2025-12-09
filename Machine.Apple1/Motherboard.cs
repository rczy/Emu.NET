using CPU.MOS6502;
using Device.PIA;
using Machine.Apple1.Devices;
using Machine.Apple1.Peripherals;

namespace Machine.Apple1;

/*

Devices on the bus:
- RAM (32K)
- ROM (8K)
- PIA (Peripheral Interface Adapter)

Memory map of the connected devices:

+-----------------+ 0x0000
|                 |
|       RAM       |
|                 | 0x7FFF
+-----------------+ 0x8000
|                 |
|      Unused     |
|                 | 0xD00F
+-----------------+ 0xD010
|                 |
|       PIA       |
|                 | 0xD013
+-----------------+ 0xD014
|                 |
|      Unused     |
|                 | 0xDFFF
+-----------------+ 0xE000
|                 |
|       ROM       |
|                 | 0xFFFF
+-----------------+

*/
public class Motherboard
{
    private readonly Bus _bus;
    private readonly Core _cpu;
    private readonly Adapter _pia;
    private readonly RAM _ram;
    private readonly ROM _rom;
    private readonly Keyboard _keyboard;
    private readonly Display _display;
    
    private EmulationState _state;

    public Motherboard()
    {
        _bus = new Bus();
        _cpu = new Core(_bus);

        _ram = new RAM();
        _rom = new ROM();
        _pia = new Adapter();
        
        _bus.Connect(_ram, new Bus.AddressRange(0x0000, 0x7FFF));
        _bus.Connect(_rom, new Bus.AddressRange(0xE000, 0xFFFF), 0x1FFF);
        _bus.Connect(_pia, new Bus.AddressRange(0xD010, 0xD013), 0x03);
        
        _keyboard = new Keyboard();
        _display = new Display();
        
        _pia.Connect(PortSection.A, _keyboard);
        _pia.Connect(PortSection.B, _display);
    }

    public void RunEmulationLoop()
    {
        _state = EmulationState.ResetRequested;

        for (;;)
        {
            _keyboard.HandleKeypress(ref _state);
            
            if (_state == EmulationState.ResetRequested)
            {
                Reset();
                _state = EmulationState.Running;
            }
            else if (_state == EmulationState.Stopped)
            {
                break;
            }

            _cpu.Tick();
            _pia.Enable();
        }
    }

    private void Reset()
    {
        _cpu.Reset();
        _pia.Reset();
        _display.Reset();
    }
}
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
    
}
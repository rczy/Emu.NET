# PIA Test Suite

This project contains the unit tests for the [PIA](../Device.PIA/README.md).
All of these tests use the **xUnit** framework.

## Tests
These unit tests ensure that the PIA implementation is working as expected:
- data read from or written to addresses is routed to the correct register/peripheral
- port or data direction register is selected by control register bit 2
- data direction register masks peripheral data
- reset clears all registers and initializes state
- low-high or high-low transitions are detected on input control lines depending on configuration
- interrupt lines (**IRQA**, **IRQB**) can be masked
- interrupt flags are cleared when data is read from data register
- control line 2 (**C2**) value cannot be changed manually by the peripheral in output mode
- C2 value can be set in manual mode
- C2 is restored in handshake mode either by C1 active transition or enable pulse

## How to Run
To run the tests, execute the following command from the solution root:
```shell
dotnet test Device.PIA.Tests
```
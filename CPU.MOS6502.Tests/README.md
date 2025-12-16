# MOS Technology 6502 Test Suite

This project contains the integration and unit tests for the [6502 CPU](../CPU.MOS6502/README.md).
All of these tests use the **xUnit** framework.

## Integration Tests
The integration tests consist of three programs written in 6502 assembly:
the functional and interrupt tests are created by Klaus Dormann and the decimal mode test is by Bruce Clark.

All the integration tests were assembled using the **AS65 assembler** by Frank A. Kingswood.
The produced binaries and listing files can be found in the `binaries` directory.

For more information check out Klaus Dormann's [GitHub repository](https://github.com/Klaus2m5/6502_65C02_functional_tests).

## Unit Tests
The unit tests make sure that the different parts of the 6502 CPU are working in isolation as needed:
- various devices can be connected to the bus and the reads/writes are valid
- the cycle-accurate execution sequences are correct for each type of instructions
- all the operations produce the expected outcomes
- IRQ, NMI and RES interrupt lines are working correctly
- the interrupt sequence execution is correct
- the CPU can be stalled only in read cycles (RDY is ignored during write cycles)

## How to Run
To run the tests, execute the following commands from the solution root:

Run all tests:
```shell
dotnet test CPU.MOS6502.Tests
```

Run unit tests only:
```shell
dotnet test CPU.MOS6502.Tests --filter Category=Unit
```

Run integration tests only (note: these take longer to execute):
```shell
dotnet test CPU.MOS6502.Tests --filter Category=Integration
```
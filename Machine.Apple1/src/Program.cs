using System.CommandLine;
using System.Globalization;

namespace Machine.Apple1;

public static class Program
{
    private const char AddressSeparator = ':';
    private static readonly RootCommand RootCommand = new("Emu.NET - Apple I emulator");
    private static readonly Option<string[]> BinaryOption = new("--binary")
    {
        Description = $"The binary to load into RAM. Use optional '{AddressSeparator}' after path to specify address in hex.",
        HelpName = $"path{AddressSeparator}hexaddress",
        AllowMultipleArgumentsPerToken = true,
    };
    
    public static int Main(string[] args)
    {
        RootCommand.Options.Add(BinaryOption);
        RootCommand.SetAction(Run);
        
        return RootCommand.Parse(args).Invoke();
    }

    private static int Run(ParseResult parseResult)
    {
        if (parseResult.Errors.Count > 0)
        {
            foreach (var error in parseResult.Errors)
                Console.Error.WriteLine(error.Message);
            return 1;
        }
        
        var machine = new Motherboard();
        
        foreach (var option in parseResult.GetValue(BinaryOption) ?? [])
        {
            var (binary, address) = SplitBinaryOption(option);
            if (!machine.LoadProgram(binary, address))
            {
                machine.CleanUp();
                return 1;
            }
        }
        
        machine.RunEmulationLoop();
        machine.CleanUp();
        
        return 0;
    }

    private static (string, ushort) SplitBinaryOption(string optionString)
    {
        var parts = optionString.Split(AddressSeparator);
        var bin = parts[0];
        
        if (parts.Length <= 1)
            return (bin, 0);

        int.TryParse(parts[1], NumberStyles.HexNumber, null, out var addr);
        return (bin, (ushort)addr);
    }
}
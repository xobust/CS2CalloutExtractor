using System.CommandLine;
using System.Text;
using System.Text.Json;
using SteamDatabase.ValvePak;
using CS2CalloutExtractor;

var pakFilePathOption = new Argument<string>(
    "pakFilePath",
    "Path to the .vpk file to extract callouts from")
{
    Arity = ArgumentArity.ExactlyOne
};
var formatOption = new Option<string>(
    "--format",
    () => "json",
    "Output format: 'json' or 'csv'")
{
    IsRequired = false
};
var outputOption = new Option<string?>(
    "--output",
    "Path to the output file (optional). If not specified, output will be printed to the console.")
{
    IsRequired = false
};

var rootCommand = new RootCommand("CS2 Callout Extractor CLI"); 
rootCommand.AddArgument(pakFilePathOption);
rootCommand.AddOption(formatOption);
rootCommand.AddOption(outputOption);

rootCommand.SetHandler((pakFilePath, format, output) =>
{
    using var package = new Package();

    Console.WriteLine($"Reading {pakFilePath}");
    package.SetFileName(pakFilePath);
    package.Read(File.OpenRead(pakFilePath));

    // Get the localized names from the package
    var localizationCsv = "place-localization.csv";
    var localizedNames = new Dictionary<string, string>();

    if (File.Exists(localizationCsv))
    {
        using var reader = new StreamReader(localizationCsv);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(',');
            if (parts.Length == 2)
            {
                localizedNames[parts[0]] = parts[1];
            }
        }
    }
    else
    {
        Console.WriteLine($"Localization file {localizationCsv} not found. Exiting.");
        return;
    }

    // Read callouts
    var callouts = new ReadCallouts(package, localizedNames).Read().ToList();

    // Generate output
    if (format == "json")
    {
        var jsonOutput = JsonSerializer.Serialize(callouts, new JsonSerializerOptions { WriteIndented = true,  IncludeFields = true });

        if (output != null)
        {
            File.WriteAllText(output, jsonOutput);
            Console.WriteLine($"Output written to {output}");
        }
        else
        {
            Console.WriteLine(jsonOutput);
        }
    }
    else if (format == "csv")
    {
        var csvOutput = new StringBuilder();
        csvOutput.AppendLine("Name,Localization,MinBound,MaxBound");

        foreach (var callout in callouts)
        {
            csvOutput.AppendLine($"{callout.Name},{callout.EnglishName},{callout.MinBound},{callout.MaxBound}");
        }

        if (output != null)
        {
            File.WriteAllText(output, csvOutput.ToString());
            Console.WriteLine($"Output written to {output}");
        }
        else
        {
            Console.WriteLine(csvOutput.ToString());
        }
    }
    else
    {
        Console.WriteLine("Invalid format specified. Use 'json', 'csv', or 'console'.");
    }
}, pakFilePathOption, formatOption, outputOption);

await rootCommand.InvokeAsync(args);
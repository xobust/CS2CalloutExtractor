# CounterStrike 2 Callout Extractor

Extracts callouts from `.vpk` maps using [ValveResourceFormat](https://github.com/ValveResourceFormat/ValveResourceFormat). The callouts are defined in [env_cs_place](https://developer.valvesoftware.com/wiki/Env_cs_place) entities. English localizations for the place names have been copied from the valve wiki. 


## CLI

The `CS2CalloutExtractor.Cli` is a command-line tool for extracting callouts from `.vpk` files and outputting them in JSON or CSV format.

### Installation

Download the latest release for your platform from the [Releases](https://github.com/your-repo/zone-extract/releases) page.

### Usage

Run the CLI with the following options:

```bash
CS2CalloutExtractor.Cli <pakFilePath> [--format <json|csv>] [--output <file>]
```

### Options

- **`pakFilePath`** (required): Path to the `.vpk` file to extract callouts from.
- **`--format`** (optional): Output format. Can be `json` or `csv`. Defaults to `json`.
- **`--output`** (optional): Path to the output file. If not specified, the output will be printed to the console.

### Examples

#### Print JSON to Console

```bash
CS2CalloutExtractor.Cli "path/to/file.vpk" --format json
```

#### Write CSV to File

```bash
CS2CalloutExtractor.Cli "path/to/file.vpk" --format csv --output callouts.csv
```


## Library

The `CS2CalloutExtractor` library provides functionality to extract callouts from `.vpk` files. It can be used in other .NET projects to programmatically access callout data.

### Installation

You can install the library via NuGet:

```bash
dotnet add package CS2CalloutExtractor
```

### Usage

Here’s an example of how to use the library:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using SteamDatabase.ValvePak;
using CS2CalloutExtractor;


using var package = new Package();
package.SetFileName("path/to/file.vpk");
package.Read(File.OpenRead("path/to/file.vpk"));

var localizedNames = new Dictionary<string, string>
{
    { "place_name_1", "Localized Name 1" },
    { "place_name_2", "Localized Name 2" }
};

var callouts = new ReadCallouts(package, localizedNames).Read();

foreach (var callout in callouts)
{
    Console.WriteLine($"Name: {callout.Name}, MinBound: {callout.MinBound}, MaxBound: {callout.MaxBound}");
}
```


## Acknowledgements

Thanks to [@ValveResourceFormat](https://github.com/ValveResourceFormat) for making a great library that was used to extract the correct data. 

The solution was heavily inspired by [@hjbdev's](https://github.com/hjbdev) [cs2-vmap-tools](https://github.com/hjbdev/cs2-vmap-tools), 
but I liked the challenge to be able to do it without decompiling the full map.
His blogpost [Counter-Strike 2: Where are the callouts stored?](https://hjb.dev/posts/counter-strike-2-where-are-all-the-callouts-2) was a great help.

## Development

### Prerequisites

- .NET 9 SDK or later
- Visual Studio Code or another IDE

### Building the Project

To build the library and CLI:

```bash
dotnet build
```

### Running the CLI Locally

```bash
dotnet run --project CS2CalloutExtractor.Cli -- <pakFilePath> [--format <json|csv>] [--output <file>]
```

---

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.



## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

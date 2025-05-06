
# CounterStrike 2 Callout Extractor

Extracts callouts from `.vpk` maps using [ValveResourceFormat](https://github.com/ValveResourceFormat/ValveResourceFormat). The callouts are defined in [env_cs_place](https://developer.valvesoftware.com/wiki/Env_cs_place) entities. English localizations for the place names have been copied from the valve wiki. 


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

using SteamDatabase.ValvePak;

using CS2CalloutExtractor;

using var package = new Package();
string pakFilePath;

// Get filname from command line arguments
if (args.Length == 0) {
    
    // read from stdin
    Console.WriteLine("No pak file specified. Please provide a pak file path as an argument.");
    pakFilePath = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\game\csgo\maps\de_train.vpk";
}else{
    // read from command line argument
    pakFilePath = args[0];
}
Console.WriteLine($"Reading {pakFilePath}");

package.SetFileName(pakFilePath);
// Can also pass in a stream
package.Read(File.OpenRead($"{pakFilePath}"));


// Get the localized names from the package
var localizationCsv = "place-localization.csv";
var localizedNames = new Dictionary<string, string>();
using (var reader = new StreamReader(localizationCsv))
{
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

// print the localized names
localizedNames.ToList().ForEach(e => Console.WriteLine($"Name: {e.Key}, Localization: {e.Value}"));

new ReadCallouts(package, localizedNames).Read()
    .ToList()
    .ForEach(e => Console.WriteLine($"Name: {e.Name}, Localization: {e.EnglishName}, Bounds: {e.Bounds[0]} - {e.Bounds[1]}"));
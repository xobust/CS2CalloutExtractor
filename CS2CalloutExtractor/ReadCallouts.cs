using System.Numerics;
using SteamDatabase.ValvePak;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.ResourceTypes.RubikonPhysics;
using ValveResourceFormat.ResourceTypes.RubikonPhysics.Shapes;
using ValveResourceFormat.Utils;

namespace CS2CalloutExtractor;

/// <summary>
/// Reads callouts from a CS2 package
/// </summary>
/// <param name="package"></param>
/// <param name="localizedNames"></param>
public class ReadCallouts(Package package, Dictionary<string, string>? localizedNames = null)
{
    private PackageEntry ventsEntry = package.Entries.Where(e => e.Key.Contains("vents_c"))
        .First().Value.First();
    private List<PackageEntry> vmdlEntries = package.Entries.Where(e => e.Key.Contains("vmdl_c"))
        .First().Value;

    public IEnumerable<Callout> Read()
    {
        List<EntityLump.Entity> placeEntities = GetPlaceEntities();

        return placeEntities
            .Select(e => new
            {
                Name = (string)e.GetProperty("place_name").Value,
                Model = (string)e.GetProperty("model").Value,
                Origin = EntityTransformHelper.ParseVector((string)e.GetProperty("origin").Value),
            })
            .Select(e =>
            {
                Resource? modelResource = ReadModel(e.Model);
                if (modelResource == null)
                {
                    Console.Error.WriteLine($"Failed to read model {e.Model} for callout {e.Name}");
                    return null;
                }
                (Vector3 minBound, Vector3 maxBound) = GetModelBounds(modelResource, e.Origin);
                return new Callout
                {
                    Name = e.Name,
                    EnglishName = localizedNames?.GetValueOrDefault(e.Name.ToLowerInvariant(), ""),
                    MinBound = minBound,
                    MaxBound = maxBound,
                };
            }).OfType<Callout>();
    }

    private (Vector3 min, Vector3 max) GetModelBounds(Resource modelResource, Vector3 origin)
    {
        var physData = (PhysAggregateData)modelResource.GetBlockByType(BlockType.PHYS);
        if (physData == null)
        {
            throw new Exception("Failed to read phys data.");
        }
        Shape shape = physData.Parts.First().Shape;

        if (shape.Hulls == null)
        {
            throw new Exception("Failed to read hulls.");
        }

        Hull? hull = shape.Hulls.FirstOrDefault()?.Shape;

        if (hull == null)
        {
            throw new Exception("Failed to read hull.");
        }

        // Get the min and max bounds of the hull
        Vector3 min = hull.Value.Min + origin;
        Vector3 max = hull.Value.Max + origin;

        return (min, max);
    }

    private Resource? ReadModel(string modelFile)
    {
        modelFile = modelFile.Replace("vmdl", "vmdl_c");
        modelFile = modelFile.Replace("\\", "/");

        PackageEntry? modelEntry = vmdlEntries
            .Where(e => e.GetFullPath() == modelFile)
            .FirstOrDefault();
        if (modelEntry == null)
        {
            return null;
        }

        Resource resource = ReadEntry(modelEntry);

        return resource;
    }

    private List<EntityLump.Entity> GetPlaceEntities()
    {
        Resource resource = ReadEntry(ventsEntry);
        if (resource == null)
        {
            throw new Exception("Failed to read vents entry.");
        }

        EntityLump entityLump = (EntityLump)resource.DataBlock;
        List<EntityLump.Entity> entities = entityLump.GetEntities().ToList();

        // Find env_cs_place
        var placeEntities = entities.Where(e =>
            e.ContainsKey("classname")
            && e.GetProperty("classname").Value.ToString() == "env_cs_place")
            .ToList();

        return placeEntities;
    }

    private Resource ReadEntry(PackageEntry modelEntry)
    {
        byte[] read;
        package.ReadEntry(modelEntry, out read);
        Resource resource = new Resource();
        resource.Read(new MemoryStream(read));
        return resource;
    }
}
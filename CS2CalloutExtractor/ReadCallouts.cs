
using System.Numerics;
using SteamDatabase.ValvePak;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;
using ValveResourceFormat.ResourceTypes.RubikonPhysics;
using ValveResourceFormat.ResourceTypes.RubikonPhysics.Shapes;
using ValveResourceFormat.Utils;

namespace CS2CalloutExtractor;

public class ReadCallouts(Package package, Dictionary<string, string> localizedNames)
{

    private PackageEntry ventsEntry = package.Entries.Where( e => e.Key.Contains("vents_c"))
        .First().Value.First();
    private List<PackageEntry> vmdlEntries = package.Entries.Where( e => e.Key.Contains("vmdl_c"))
        .First().Value;

  
    public IEnumerable<Callout> Read()
    {
        List<EntityLump.Entity> placeEnteties = GetPlaceEnteties();

        List<Callout> callcouts = placeEnteties
            .Select(e => new
            {
                Name = (string) e.GetProperty("place_name").Value,
                Model = (string) e.GetProperty("model").Value,
                Origin = EntityTransformHelper.ParseVector((string)e.GetProperty("origin").Value),
            })
            .Select(e => new Callout
            {
                Name = e.Name,
                EnglishName = localizedNames.GetValueOrDefault(e.Name.ToLowerInvariant(), null),
                Bounds = GetModelBounds(ReadModel(e.Model), e.Origin)
                    .ToArray(),
            })
            .ToList();

        return callcouts;
    }

    private List<Vector3> GetModelBounds(Resource modelResource, Vector3 origin)
    {
        var physData = (PhysAggregateData)modelResource.GetBlockByType(BlockType.PHYS);
        if (physData == null)
        {
            throw new Exception("Failed to read phys data.");
        }
        Shape shape = physData.Parts.First().Shape;

        if(shape.Hulls == null)
        {
            throw new Exception("Failed to read hulls.");
        }

        Hull? hull = shape.Hulls.FirstOrDefault()?.Shape;
                
        if (hull == null)
        {
            throw new Exception("Failed to read hull.");
        }

        // Get the min and max bounds of the hull
        Vector3 min = hull.Value.Min;
        Vector3 max = hull.Value.Max;
        min += origin;
        max += origin;

        return [min, max];
    }


    private Resource ReadModel(string modelFile)
    {
        modelFile = modelFile.Replace("vmdl", "vmdl_c");

        PackageEntry modelEntry = vmdlEntries
            .Where(e => e.GetFullPath() == modelFile)
            .First();
        Resource resource = ReadEntry(modelEntry);

        return resource;
    }


    private List<EntityLump.Entity> GetPlaceEnteties()
    {
        Resource resource = ReadEntry(ventsEntry);
        if (resource == null)
        {
            throw new Exception("Failed to read vents entry.");
        }

        EntityLump entityLump = (EntityLump)resource.DataBlock;
        List<EntityLump.Entity> entities = entityLump.GetEntities().ToList();

        //find env_cs_place
        var placeEnteties = entities.Where(e => 
            e.ContainsKey("classname")
            && e.GetProperty("classname").Value.ToString() == "env_cs_place")
            .ToList();

        return placeEnteties;
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

using System.Numerics;

namespace CS2CalloutExtractor;

public record Callout
{
    public required string Name { get; init; }

    public string? EnglishName  { get; init; } = null;

    /// <summary>
    /// The bounds of the callout, in the format [min, max].
    /// The min and max are Vector3 objects representing the minimum and maximum coordinates of the callout's bounding box.
    /// </summary>
    public Vector3[] Bounds { get; init; } = new Vector3[2];
}

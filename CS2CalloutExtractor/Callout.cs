using System.Numerics;

namespace CS2CalloutExtractor;

public record Callout
{
    public required string Name { get; init; }

    public string? EnglishName { get; init; } = null;

    /// <summary>
    /// The minimum bound of the callout's bounding box.
    /// </summary>
    public Vector3 MinBound { get; init; }

    /// <summary>
    /// The maximum bound of the callout's bounding box.
    /// </summary>
    public Vector3 MaxBound { get; init; }
}

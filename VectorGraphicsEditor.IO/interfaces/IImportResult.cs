using Geometry;

namespace IO.importers;

public class ImportResult
{
    public IEnumerable<IShape> Figures { get; internal init; } = null!;
    // public IEnumerable<IShapeOfPoints> Points { get; internal init; } = null!;
}
using IO.importers;

namespace IO.interfaces;

public interface IImporter
{
    public ImportResult ImportFrom(string path);
}
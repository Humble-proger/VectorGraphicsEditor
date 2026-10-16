using Avalonia.Controls;

namespace IO.interfaces;

public interface IExporter
{
    public void Export(Canvas canvas, string path);
}
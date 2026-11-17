using Core.Abstractions;
using Core.Events;

internal sealed class ConsoleSink : INotificationSink
{
    public void Notify(CatalogChangedEventArgs change) =>
        Console.WriteLine($"DI: {change.Kind} {change.Id}");
}

internal sealed class FileSink(string path) : INotificationSink
{
    public void Notify(CatalogChangedEventArgs change)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
            File.AppendAllText(path, $"DI: {change.Kind} {change.Id}{Environment.NewLine}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Лог недоступний: {ex.Message}");
        }
    }
}

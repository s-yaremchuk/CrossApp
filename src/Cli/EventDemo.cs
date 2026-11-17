using Core.Abstractions;
using Core.Dto;
using Core.Events;
using Core.Import;
using Core.Services;

internal static class EventDemo
{
    public static int Run(IBookStore store, INotificationSink? sink = null)
    {
        var service = new LendingService(store, sink);
        var path = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
        int changes = 0;
        void Log(object? sender, CatalogChangedEventArgs e)
        {
            var line = $"{e.At:HH:mm:ss} {e.Kind} {e.Id} issued={e.IsIssued}";
            Console.WriteLine(line);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.AppendAllText(path, line + Environment.NewLine);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Console.Error.WriteLine($"Лог недоступний: {ex.Message}");
            }
        }
        EventHandler<CatalogChangedEventArgs> count = (_, _) =>
            Console.WriteLine($"Лічильник після логера: {++changes}");
        service.Changed += Log;
        service.Changed += count;
        var copy = service.AddBook(Guid.NewGuid().ToString("N"),
            new BookDto("B-1", "9780141439518", "Гордість і упередження", 1813));
        service.IssueCopy(copy.Id);
        try { service.IssueCopy(copy.Id); }
        catch (InvalidOperationException ex) { Console.WriteLine($"Відмова: {ex.Message}; подій: {changes}"); }
        service.ReturnCopy(copy.Id);
        Console.WriteLine($"Вільних: {service.Search(c => !c.IsIssued).Count}; подій: {changes}");
        service.Changed -= Log;
        service.IssueCopy(copy.Id);
        Console.WriteLine($"Після відписки логера: видано={service.Find(copy.Id)!.IsIssued}; подій={changes}");
        service.Changed -= count;
        service.ReturnCopy(copy.Id);
        Console.WriteLine($"Після обох відписок: видано={service.Find(copy.Id)!.IsIssued}; подій={changes}");
        var result = BookCsvImporter.Load("data/sample.csv", message => Console.WriteLine($"Action: {message}"));
        Console.WriteLine($"Імпорт: {result.Items.Count}, пропусків: {result.Errors.Count}");
        Console.WriteLine($"Журнал: {path}");
        return 0;
    }
}

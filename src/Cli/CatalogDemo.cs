using Core.Dto;
using Core.Services;

internal static class CatalogDemo
{
    public static void Run(LendingService service)
    {
        Console.WriteLine($"До додавання: {service.All().Count}");
        var book = new BookDto("B-003", "9780141187761", "1984", 1949);
        var copy = service.AddBook(Guid.NewGuid().ToString("N"), book);
        service.IssueCopy(copy.Id);
        Console.WriteLine($"Знайдено за id: {service.Find(copy.Id)!.Id}");
        Console.WriteLine($"Видано: {service.Find(copy.Id)!.IsIssued}");
        var issued = service.Search(item => item.IsIssued);
        Console.WriteLine($"Пошук виданих: {issued.Count}");
        try { service.AddBook(copy.Id, book); }
        catch (InvalidOperationException ex) { Console.WriteLine($"Відмова: {ex.Message}"); }
        service.ReturnCopy(copy.Id);
        Console.WriteLine($"Після повернення видано: {service.Find(copy.Id)!.IsIssued}");
        foreach (var entry in service.All())
            Console.WriteLine($"  {entry.Id}  {entry.ToDto().Book.Title}");
        Console.WriteLine($"Усього: {service.All().Count}");
    }
}

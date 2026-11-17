using Core.Domain;
using Core.Dto;
using Core.Services;
using Core.Storage;

internal static class StorageDemo
{
    public static int Run(string action, string path)
    {
        var catalog = new CatalogService(new JsonLoanRepository(path));
        const string id = "L-01";
        var date = new DateOnly(2026, 10, 12);
        try
        {
            switch (action)
            {
                case "issue":
                    var book = new BookDto("B-1", "9780141439518", "Гордість і упередження", 1813);
                    catalog.Issue(id, BookCopy.Create("C-01", book), "R-1", date);
                    break;
                case "return":
                    catalog.Return(id, date.AddDays(1));
                    break;
                case "show":
                    break;
                default:
                    Console.Error.WriteLine("Операції: issue, show, return.");
                    return 1;
            }
            var loan = catalog.GetById(id) ?? throw new KeyNotFoundException($"Видачу {id} не знайдено.");
            Console.WriteLine($"{loan.Id} | {loan.Copy.Id} | {loan.ReaderId} | {loan.Status}");
            return 0;
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or
            KeyNotFoundException or IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }
}

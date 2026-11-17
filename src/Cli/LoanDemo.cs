using Core.Domain;
using Core.Dto;
using Core.Import;

internal static class LoanDemo
{
    public static int Run()
    {
        var book = new BookDto("B-1", "9780141439518", "Гордість і упередження", 1813);
        var copy = BookCopy.Create("C-01", book);
        var register = new LoanRegister();
        var today = new DateOnly(2026, 10, 12);
        var loan = register.Issue("L-01", copy, "R-1", today);
        Console.WriteLine($"Видано {copy.Id}: {loan.Status}, читач {loan.ReaderId}.");
        Reject(() => register.Issue("L-02", copy, "R-2", today));
        Reject(() => loan.Close(today.AddDays(-1)));
        Reject(() => BookCopy.Create("C-02", book with { Isbn = " " }));
        Console.WriteLine($"Після відмов: {loan.Status}, примірник видано: {copy.IsIssued}.");
        loan.Close(today.AddDays(1));
        Reject(() => loan.Close(today.AddDays(2)));
        var restored = Loan.FromDto(loan.ToDto());
        Console.WriteLine($"Відновлено {restored.Id}: {restored.Status}, повернено {restored.ReturnedOn:yyyy-MM-dd}.");
        Reject(() => Loan.FromDto(loan.ToDto() with { ReturnedOn = today.AddDays(-1) }));

        for (int i = 1; i <= 5; i++)
            register.Issue($"N-{i}", BookCopy.Create($"N-{i}", book), "R-1", today);
        var sixth = BookCopy.Create("N-6", book);
        Reject(() => register.Issue("N-6", sixth, "R-1", today));
        register.Loans[1].Close(today);
        register.Issue("N-6", sixth, "R-1", today);
        Console.WriteLine("Після повернення читач отримав N-6.");

        var imported = CopyImporter.FromBooks(BookCsvImporter.Load("data/copies.csv"));
        Console.WriteLine($"Імпорт примірників: {imported.Items.Count}, помилок: {imported.Errors.Count}.");
        foreach (var error in imported.Errors)
            Console.WriteLine(error);
        return 0;
    }

    private static void Reject(Action action)
    {
        try { action(); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            return;
        }
        throw new InvalidOperationException("Очікувана доменна перевірка не спрацювала.");
    }
}

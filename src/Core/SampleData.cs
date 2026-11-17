using Core.Domain;
using Core.Dto;

namespace Core;

public static class SampleData
{
    public static IEnumerable<BookCopy> Copies()
    {
        BookDto[] books =
        [
            new("B-001", "9780141439518", "Гордість і упередження", 1813),
            new("B-002", "9780140449136", "Одіссея", 1996),
            new("B-003", "9780141187761", "1984", 1949),
            new("B-004", "9780141439600", "Джейн Ейр", 1847),
            new("B-005", "9780142437230", "Дон Кіхот", 1605),
            new("B-006", "9780140449266", "Злочин і кара", 1866),
            new("B-007", "9780142437209", "Мобі Дік", 1851),
            new("B-008", "9780141439556", "Буремний перевал", 1847)
        ];
        foreach (var book in books)
            for (int number = 1; number <= 2; number++)
                yield return BookCopy.Create($"{book.Id}-{number}", book);
    }
}

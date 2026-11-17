using Core.Domain;
using Core.Dto;

namespace Core.Import;

public static class CopyImporter
{
    public static ImportResult<BookCopy> FromBooks(ImportResult<BookDto> source)
    {
        var copies = new List<BookCopy>();
        var errors = source.Errors.ToList();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < source.Items.Count; i++)
        {
            var book = source.Items[i];
            try
            {
                var copy = BookCopy.Create(book.Id, book);
                if (!ids.Add(copy.Id))
                    throw new ArgumentException($"Повторний інвентарний номер {copy.Id}.");
                copies.Add(copy);
            }
            catch (ArgumentException ex)
            {
                var location = source.SourceLines.Count == source.Items.Count
                    ? $"рядок {source.SourceLines[i]}" : $"запис {i + 1}";
                errors.Add($"{location}: {ex.Message}");
            }
        }
        return new(copies.AsReadOnly(), errors.AsReadOnly());
    }
}

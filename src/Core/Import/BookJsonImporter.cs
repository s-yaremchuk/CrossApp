using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Dto;

namespace Core.Import;

public static class BookJsonImporter
{
    public static ImportResult<LibraryRecord> Load(string path)
    {
        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
            throw new JsonException("Коренем JSON має бути масив книг.");

        var books = new List<LibraryRecord>();
        var errors = new List<string>();
        int index = 0;
        foreach (var element in document.RootElement.EnumerateArray())
        {
            index++;
            try
            {
                var book = element.Deserialize(BookJsonContext.Default.BookDto);
                if (book is null || string.IsNullOrWhiteSpace(book.Id) || string.IsNullOrWhiteSpace(book.Isbn)
                    || string.IsNullOrWhiteSpace(book.Title) || book.Year < 1450 || book.Year > DateTime.Now.Year)
                    errors.Add($"елемент {index}: перевірте id, ISBN, назву і рік");
                else
                    books.Add(book);
            }
            catch (JsonException)
            {
                errors.Add($"елемент {index}: неправильний тип поля або запису");
            }
        }
        return new(books.AsReadOnly(), errors.AsReadOnly());
    }
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(BookDto))]
internal partial class BookJsonContext : JsonSerializerContext;

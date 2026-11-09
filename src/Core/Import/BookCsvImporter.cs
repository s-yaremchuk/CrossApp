using System.Globalization;
using System.Text;
using Core.Dto;

namespace Core.Import;

public static class BookCsvImporter
{
    private const char Separator = ';';
    private const string Header = "id;isbn;title;year";

    public static ImportResult<BookDto> Load(string path)
    {
        var result = Read(path, allowMixed: false);
        return new(result.Items.Cast<BookDto>().ToArray(), result.Errors);
    }

    public static ImportResult<LibraryRecord> LoadCatalog(string path) => Read(path, allowMixed: true);

    private static ImportResult<LibraryRecord> Read(string path, bool allowMixed)
    {
        var items = new List<LibraryRecord>();
        bool mixed = false;
        var errors = new List<string>();
        string[] lines = File.ReadAllLines(path, Encoding.UTF8);
        bool firstDataLine = true;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (firstDataLine)
            {
                firstDataLine = false;
                string normalized = string.Join(Separator, line.Split(Separator, StringSplitOptions.TrimEntries));
                mixed = allowMixed && normalized.Equals("type;fields", StringComparison.OrdinalIgnoreCase);
                if (mixed)
                    continue;
                if (string.Equals(normalized, Header, StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            switch (ParseLine(line, mixed))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {i + 1}: {failed.Reason}");
                    break;
            }
        }

        return new ImportResult<LibraryRecord>(items.AsReadOnly(), errors.AsReadOnly());
    }

    private static ParseOutcome ParseLine(string line, bool mixed)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        if (mixed)
        {
            return parts switch
            {
                ["B", .. var fields] => ParseBook(fields),
                ["R", var id, var name, .. var extra] when id.Length > 0 && name.Length > 0 && extra.Length <= 1
                    => new ParseOk(new ReaderDto(id, name, extra.FirstOrDefault() is { Length: > 0 } email ? email : null)),
                ["R", ..] => new ParseFailed("очікую R;id;ім’я[;email]"),
                _ => new ParseFailed($"невідомий тип '{parts[0]}'")
            };
        }
        return ParseBook(parts);
    }

    private static ParseOutcome ParseBook(string[] parts)
    {
        return parts switch
        {
            { Length: < 4 } => new ParseFailed($"очікую 4 колонки, отримав {parts.Length}"),
            ["", _, _, _] or [_, "", _, _] or [_, _, "", _]
                => new ParseFailed("ідентифікатор, ISBN або назва порожні"),
            [var id, var isbn, var title, var text]
                when int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int year)
                     && year is >= 1450 && year <= DateTime.Now.Year
                => new ParseOk(new BookDto(id, isbn, title, year)),
            [_, _, _, var text]
                => new ParseFailed($"рік '{text}' має бути цілим числом від 1450 до {DateTime.Now.Year}"),
            _ => new ParseFailed($"занадто багато колонок: {parts.Length}")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseOk(LibraryRecord Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}

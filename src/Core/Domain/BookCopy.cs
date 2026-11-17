using Core.Dto;

namespace Core.Domain;

public sealed class BookCopy
{
    private readonly BookDto _book;
    public string Id { get; }
    public string Isbn => _book.Isbn;
    public bool IsIssued { get; private set; }

    private BookCopy(string id, BookDto book) => (Id, _book) = (id, book);

    public static BookCopy Create(string id, BookDto book)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(book);
        if (string.IsNullOrWhiteSpace(book.Isbn))
            throw new ArgumentException($"Для примірника {id} потрібен ISBN.", nameof(book));
        if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Id))
            throw new ArgumentException($"Для примірника {id} потрібні назва та код видання.", nameof(book));
        if (book.Year < 1450 || book.Year > DateTime.Today.Year)
            throw new ArgumentOutOfRangeException(nameof(book), book.Year, "Рік видання поза допустимими межами.");
        return new BookCopy(id.Trim(), book);
    }

    public BookCopyDto ToDto() => new(Id, _book, IsIssued);

    public static BookCopy FromDto(BookCopyDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var copy = Create(dto.Id, dto.Book);
        if (dto.IsIssued)
            copy.Issue();
        return copy;
    }

    internal void Issue()
    {
        if (IsIssued)
            throw new InvalidOperationException($"Примірник {Id} вже видано.");
        IsIssued = true;
    }

    internal void Return() => IsIssued = false;
}

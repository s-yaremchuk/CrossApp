using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public BookCopy AddBook(string id, BookDto book)
    {
        var copy = BookCopy.Create(id, book);
        _store.Add(copy);
        return copy;
    }

    public void IssueCopy(string id)
    {
        var copy = Required(id);
        copy.Issue();
        _store.Update(copy);
    }

    public void ReturnCopy(string id)
    {
        var copy = Required(id);
        copy.Return();
        _store.Update(copy);
    }

    public IReadOnlyList<BookCopy> All() => _store.List();
    public BookCopy? Find(string id) => _store.GetById(id);
    public IReadOnlyList<BookCopy> Search(Func<BookCopy, bool> predicate) =>
        _store.List().Where(predicate).ToArray();
    public bool Remove(string id) => _store.Remove(id);

    private BookCopy Required(string id) => Find(id) ??
        throw new KeyNotFoundException($"Примірник {id} не знайдено.");
}

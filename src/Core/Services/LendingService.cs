using Core.Events;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Services;

public sealed class LendingService(IBookStore store, INotificationSink? sink = null)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public event EventHandler<CatalogChangedEventArgs>? Changed;

    private void OnChanged(BookCopy copy, CatalogChangeKind kind)
    {
        var change = new CatalogChangedEventArgs(copy.Id, kind, copy.IsIssued);
        Changed?.Invoke(this, change);
        sink?.Notify(change);
    }

    public BookCopy AddBook(string id, BookDto book)
    {
        var copy = BookCopy.Create(id, book);
        _store.Add(copy);
        OnChanged(copy, CatalogChangeKind.Added);
        return copy;
    }

    public void IssueCopy(string id)
    {
        var copy = Required(id);
        copy.Issue();
        _store.Update(copy);
        OnChanged(copy, CatalogChangeKind.Issued);
    }

    public void ReturnCopy(string id)
    {
        var copy = Required(id);
        copy.Return();
        _store.Update(copy);
        OnChanged(copy, CatalogChangeKind.Returned);
    }

    public IReadOnlyList<BookCopy> All() => _store.List();
    public BookCopy? Find(string id) => _store.GetById(id);
    public IReadOnlyList<BookCopy> Search(Func<BookCopy, bool> predicate) =>
        _store.List().Where(predicate).ToArray();
    public bool Remove(string id)
    {
        var copy = Find(id);
        if (copy is null || !_store.Remove(id)) return false;
        OnChanged(copy, CatalogChangeKind.Removed);
        return true;
    }

    private BookCopy Required(string id) => Find(id) ??
        throw new KeyNotFoundException($"Примірник {id} не знайдено.");
}

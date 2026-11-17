using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class CachingBookStore(IBookStore inner) : IBookStore
{
    private readonly Dictionary<string, BookCopyDto?> _cache = new(StringComparer.OrdinalIgnoreCase);
    public int Hits { get; private set; }

    public BookCopy? GetById(string id)
    {
        if (_cache.TryGetValue(id, out var dto)) Hits++;
        else _cache[id] = dto = inner.GetById(id)?.ToDto();
        return dto is null ? null : BookCopy.FromDto(dto);
    }

    public IReadOnlyList<BookCopy> List() => inner.List();
    public void Add(BookCopy copy) { inner.Add(copy); _cache.Remove(copy.Id); }
    public void Update(BookCopy copy) { inner.Update(copy); _cache.Remove(copy.Id); }
    public bool Remove(string id)
    {
        bool removed = inner.Remove(id);
        _cache.Remove(id);
        return removed;
    }
}

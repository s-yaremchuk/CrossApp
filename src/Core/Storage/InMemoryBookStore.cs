using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class InMemoryBookStore(IEnumerable<BookCopy>? seed = null) : IBookStore
{
    private readonly Dictionary<string, BookCopyDto> _items = (seed ?? [])
        .ToDictionary(copy => copy.Id, copy => copy.ToDto(), StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<BookCopy> List() => _items.Values.Select(BookCopy.FromDto).ToArray();
    public BookCopy? GetById(string id) => _items.TryGetValue(id, out var dto) ? BookCopy.FromDto(dto) : null;

    public void Add(BookCopy copy)
    {
        ArgumentNullException.ThrowIfNull(copy);
        if (!_items.TryAdd(copy.Id, copy.ToDto()))
            throw new InvalidOperationException($"Примірник {copy.Id} вже є в каталозі.");
    }

    public void Update(BookCopy copy)
    {
        ArgumentNullException.ThrowIfNull(copy);
        if (!_items.ContainsKey(copy.Id))
            throw new KeyNotFoundException($"Примірник {copy.Id} не знайдено.");
        _items[copy.Id] = copy.ToDto();
    }

    public bool Remove(string id) => _items.Remove(id);
}

using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileBookStore(string path) : IBookStore
{
    private readonly string _path = Path.GetFullPath(path);
    private InMemoryBookStore? _cache;
    private string? _version;

    private InMemoryBookStore Load()
    {
        if (_cache is not null) return _cache;
        _version = ReadFile();
        var rows = _version is null ? [] :
            JsonSerializer.Deserialize(_version, CatalogJson.Default.BookCopyDtoArray)
            ?? throw new InvalidDataException("Очікується масив примірників.");
        _cache = new InMemoryBookStore(rows.Select(BookCopy.FromDto));
        return _cache;
    }

    private string? ReadFile() => File.Exists(_path) ? File.ReadAllText(_path) : null;

    public IReadOnlyList<BookCopy> List() => Load().List();
    public BookCopy? GetById(string id) => Load().GetById(id);
    public void Add(BookCopy copy) => Change(items => { items.Add(copy); return true; });
    public void Update(BookCopy copy) => Change(items => { items.Update(copy); return true; });
    public bool Remove(string id) => Change(items => items.Remove(id));

    private bool Change(Func<InMemoryBookStore, bool> change)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        using var writer = new FileStream(_path + ".lock", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        var next = new InMemoryBookStore(Load().List());
        if (ReadFile() != _version)
            throw new IOException("Каталог змінився в іншому процесі. Повторіть запуск.");
        if (!change(next)) return false;

        var json = JsonSerializer.Serialize(next.List().Select(c => c.ToDto()).ToArray(),
            CatalogJson.Default.BookCopyDtoArray);
        string temp = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temp, json);
            File.Move(temp, _path, overwrite: true);
        }
        finally { File.Delete(temp); }
        _cache = next;
        _version = json;
        return true;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(BookCopyDto[]))]
internal partial class CatalogJson : JsonSerializerContext { }

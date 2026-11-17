using Core.Domain;

namespace Core.Abstractions;

public interface IBookStore
{
    IReadOnlyList<BookCopy> List();
    BookCopy? GetById(string id);
    void Add(BookCopy copy);
    void Update(BookCopy copy);
    bool Remove(string id);
}

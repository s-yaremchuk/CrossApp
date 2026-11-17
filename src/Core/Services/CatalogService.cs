using Core.Domain;
using Core.Storage;

namespace Core.Services;

public sealed class CatalogService(ILoanRepository repository)
{
    public Loan? GetById(string id) => repository.GetById(id);

    public void Issue(string id, BookCopy copy, string readerId, DateOnly date) =>
        repository.Update(register => register.Issue(id, BookCopy.FromDto(copy.ToDto()), readerId, date));

    public void Return(string id, DateOnly date)
    {
        repository.Update(register =>
        {
            var loan = register.Loans.FirstOrDefault(item => item.Id == id)
                ?? throw new KeyNotFoundException($"Видачу {id} не знайдено.");
            loan.Close(date);
        });
    }
}

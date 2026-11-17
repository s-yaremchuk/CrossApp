using Core.Domain;

namespace Core.Storage;

public interface ILoanRepository
{
    Loan? GetById(string id);
    void Update(Action<LoanRegister> change);
}

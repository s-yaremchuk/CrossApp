namespace Core.Domain;

public sealed class LoanRegister
{
    private readonly List<Loan> _loans = [];
    public IReadOnlyList<Loan> Loans => _loans.AsReadOnly();

    internal void Restore(Core.Dto.LoanDto snapshot)
    {
        var loan = Loan.FromDto(snapshot);
        if (_loans.Any(item => item.Id == loan.Id))
            throw new InvalidOperationException($"Повторний номер видачі {loan.Id}.");
        if (loan.Status == LoanStatus.Open)
            EnsureAvailable(loan.Copy.Id, loan.ReaderId);
        _loans.Add(loan);
    }

    public Loan Issue(string id, BookCopy copy, string readerId, DateOnly issuedOn)
    {
        ArgumentNullException.ThrowIfNull(copy);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        readerId = readerId.Trim();
        if (_loans.Any(loan => loan.Id == id.Trim()))
            throw new InvalidOperationException($"Номер видачі {id} вже використано.");
        EnsureAvailable(copy.Id, readerId);
        var loan = Loan.Open(id, copy, readerId, issuedOn);
        _loans.Add(loan);
        return loan;
    }
    private void EnsureAvailable(string copyId, string readerId)
    {
        if (_loans.Any(loan => loan.Copy.Id == copyId && loan.Status == LoanStatus.Open))
            throw new InvalidOperationException($"Примірник {copyId} вже видано.");
        if (_loans.Count(loan => loan.ReaderId == readerId && loan.Status == LoanStatus.Open) >= 5)
            throw new InvalidOperationException($"Читач {readerId} уже має 5 відкритих видач.");
    }
}

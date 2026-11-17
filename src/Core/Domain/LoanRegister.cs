namespace Core.Domain;

public sealed class LoanRegister
{
    private readonly List<Loan> _loans = [];
    public IReadOnlyList<Loan> Loans => _loans.AsReadOnly();

    public Loan Issue(string id, BookCopy copy, string readerId, DateOnly issuedOn)
    {
        ArgumentNullException.ThrowIfNull(copy);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        readerId = readerId.Trim();
        if (_loans.Any(loan => loan.Id == id.Trim()))
            throw new InvalidOperationException($"Номер видачі {id} вже використано.");
        if (_loans.Any(loan => loan.Copy.Id == copy.Id && loan.Status == LoanStatus.Open))
            throw new InvalidOperationException($"Примірник {copy.Id} вже видано.");
        if (_loans.Count(loan => loan.ReaderId == readerId && loan.Status == LoanStatus.Open) >= 5)
            throw new InvalidOperationException($"Читач {readerId} уже має 5 відкритих видач.");
        var loan = Loan.Open(id, copy, readerId, issuedOn);
        _loans.Add(loan);
        return loan;
    }
}

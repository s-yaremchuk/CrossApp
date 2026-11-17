using Core.Dto;

namespace Core.Domain;

public enum LoanStatus { Open, Closed }

public sealed class Loan
{
    public string Id { get; }
    public BookCopy Copy { get; }
    public string ReaderId { get; }
    public DateOnly IssuedOn { get; }
    public DateOnly? ReturnedOn { get; private set; }
    public LoanStatus Status => ReturnedOn.HasValue ? LoanStatus.Closed : LoanStatus.Open;

    private Loan(string id, BookCopy copy, string readerId, DateOnly issuedOn) =>
        (Id, Copy, ReaderId, IssuedOn) = (id, copy, readerId, issuedOn);

    internal static Loan Open(string id, BookCopy copy, string readerId, DateOnly issuedOn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(readerId);
        ArgumentNullException.ThrowIfNull(copy);
        copy.Issue();
        return new Loan(id.Trim(), copy, readerId.Trim(), issuedOn);
    }

    public void Close(DateOnly returnedOn)
    {
        ReturnedOn = Status switch
        {
            LoanStatus.Closed => throw new InvalidOperationException($"Видачу {Id} вже закрито."),
            _ when returnedOn < IssuedOn => throw new ArgumentOutOfRangeException(nameof(returnedOn),
                returnedOn, $"Повернення примірника {Copy.Id} раніше дати видачі {IssuedOn:yyyy-MM-dd}."),
            LoanStatus.Open => returnedOn,
            _ => throw new InvalidOperationException($"Невідомий стан видачі {Id}.")
        };
        Copy.Return();
    }

    public LoanDto ToDto() => new(Id, Copy.Id, Copy.ToDto().Book, ReaderId, IssuedOn, ReturnedOn);

    public static Loan FromDto(LoanDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var loan = Open(dto.Id, BookCopy.Create(dto.CopyId, dto.Book), dto.ReaderId, dto.IssuedOn);
        if (dto.ReturnedOn is DateOnly returnedOn)
            loan.Close(returnedOn);
        return loan;
    }
}

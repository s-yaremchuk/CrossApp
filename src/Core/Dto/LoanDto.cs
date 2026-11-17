namespace Core.Dto;


public sealed record BookCopyDto(string Id, BookDto Book, bool IsIssued = false);

public sealed record LoanDto(string Id, string CopyId, BookDto Book, string ReaderId,
    DateOnly IssuedOn, DateOnly? ReturnedOn);

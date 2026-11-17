namespace Core.Dto;

public sealed record ImportResult<T>(IReadOnlyList<T> Items, IReadOnlyList<string> Errors)
{
    public IReadOnlyList<int> SourceLines { get; init; } = [];
}

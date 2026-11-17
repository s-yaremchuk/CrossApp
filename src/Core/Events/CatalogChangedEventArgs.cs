namespace Core.Events;

public enum CatalogChangeKind { Added, Issued, Returned, Removed }

public sealed class CatalogChangedEventArgs(string id, CatalogChangeKind kind, bool isIssued) : EventArgs
{
    public string Id { get; } = id;
    public CatalogChangeKind Kind { get; } = kind;
    public bool IsIssued { get; } = isIssued;
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}

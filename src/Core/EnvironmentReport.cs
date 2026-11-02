namespace Core;

public sealed record EnvironmentReport(
    string OsDescription,
    string OsVersion,
    string ClrVersion,
    string FrameworkDescription,
    string ProcessArchitecture,
    string DetectedRid,
    string ReportedRid,
    string BaseDirectory,
    string CurrentDirectory,
    string BuildNote);

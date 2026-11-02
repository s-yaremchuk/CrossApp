using System.Runtime.InteropServices;

namespace Core;

public static class EnvironmentInfo
{
    public static EnvironmentReport Collect() => new(
        RuntimeInformation.OSDescription,
        Environment.OSVersion.ToString(),
        Environment.Version.ToString(),
        RuntimeInformation.FrameworkDescription,
        RuntimeInformation.ProcessArchitecture.ToString(),
        DetectRid(),
        RuntimeInformation.RuntimeIdentifier,
        AppContext.BaseDirectory,
        Environment.CurrentDirectory,
        BuildNote);

#if NET10_0_OR_GREATER
    private const string BuildNote = "збірка Core під net10.0";
#else
    private const string BuildNote = "збірка Core під net8.0";
#endif

    private static string DetectRid()
    {
        string os =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "unknown";

        string arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => "unknown"
        };

        return $"{os}-{arch}";
    }
}

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Core;

Console.OutputEncoding = Encoding.UTF8;
EnvironmentReport report = EnvironmentInfo.Collect();

if (args.Contains("--json"))
{
    Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    }));
    return;
}

Console.WriteLine("CrossApp — інформація про середовище");
Console.WriteLine("Студент: Яремчук Сергій, ФЕІ-33с");
Console.WriteLine(new string('-', 60));
Console.WriteLine($"ОС                 : {report.OsDescription}");
Console.WriteLine($"ОС (Environment)   : {report.OsVersion}");
Console.WriteLine($"Версія CLR         : {report.ClrVersion}");
Console.WriteLine($"Runtime            : {report.FrameworkDescription}");
Console.WriteLine($"Архітектура        : {report.ProcessArchitecture}");
Console.WriteLine($"RID (визначено)    : {report.DetectedRid}");
Console.WriteLine($"RID (від .NET)     : {report.ReportedRid}");
Console.WriteLine($"Каталог застосунку : {report.BaseDirectory}");
Console.WriteLine($"Поточний каталог   : {report.CurrentDirectory}");
Console.WriteLine($"Ціль бібліотеки    : {report.BuildNote}");
Console.WriteLine(new string('-', 60));
Console.WriteLine("Предметна область: Бібліотека (видання, примірники, читачі, видачі)");

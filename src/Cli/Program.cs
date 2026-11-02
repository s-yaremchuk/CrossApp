using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Core;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = Encoding.UTF8;

if (args is ["--environment"] or ["--json"])
{
    EnvironmentReport report = EnvironmentInfo.Collect();

    if (args.Contains("--json"))
    {
        Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }));
        return 0;
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
    return 0;
}

if (args.Length > 1)
{
    Console.Error.WriteLine("Використання: Cli [шлях.csv] | --environment | --json");
    return 1;
}

string path = args.Length == 1 ? args[0] : Path.Combine("data", "sample.csv");
try
{
    string fullPath = Path.GetFullPath(path);
    if (!File.Exists(fullPath))
    {
        Console.Error.WriteLine($"Файл не знайдено: {fullPath}");
        return 1;
    }

    ImportResult<BookDto> result = BookCsvImporter.Load(fullPath);
    Console.WriteLine($"Завантажено записів: {result.Items.Count}");
    foreach (BookDto book in result.Items.Take(5))
        Console.WriteLine($"  {book.Id,-6} {book.Isbn,-13} {book.Title,-30} {book.Year,4}");

    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string error in result.Errors)
        Console.WriteLine($"  ! {error}");

    return 0;
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
{
    Console.Error.WriteLine($"Не вдалося прочитати файл '{path}': {ex.Message}");
    return 1;
}

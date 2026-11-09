using System.Text;
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
        Console.WriteLine(JsonSerializer.Serialize(report, ReportJsonContext.Default.EnvironmentReport));
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
    Console.Error.WriteLine("Використання: Cli [шлях.csv|шлях.json] | --environment | --json");
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

    ImportResult<LibraryRecord> result = Path.GetExtension(fullPath).ToLowerInvariant() switch
    {
        ".csv" => BookCsvImporter.LoadCatalog(fullPath),
        ".json" => BookJsonImporter.Load(fullPath),
        _ => throw new NotSupportedException("Підтримуються файли .csv та .json")
    };
    Console.WriteLine($"Завантажено записів: {result.Items.Count}");
    foreach (var item in result.Items.Take(5))
    {
        switch (item)
        {
            case BookDto book:
                Console.WriteLine($"  {book.Id,-6} {book.Isbn,-13} {book.Title,-30} {book.Year,4}");
                break;
            case ReaderDto reader:
                Console.WriteLine($"  Читач {reader.Id}: {reader.FullName} {reader.Email}");
                break;
        }
    }

    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string error in result.Errors)
        Console.WriteLine($"  ! {error}");

    int total = result.Items.Count + result.Errors.Count;
    double percent = total == 0 ? 0 : 100.0 * result.Errors.Count / total;
    Console.WriteLine($"Усього: {total} | прийнято: {result.Items.Count} | пропущено: {result.Errors.Count} | помилок: {percent:F2}%");
    return 0;
}
catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
{
    Console.Error.WriteLine($"Не вдалося прочитати файл '{path}': {ex.Message}");
    return 1;
}

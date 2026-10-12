using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

// Забезпечення коректного відображення кирилиці в консолі
Console.OutputEncoding = Encoding.UTF8;

if (args.Contains("--json"))
{
    var info = new
    {
        project = "CrossApp — практикум з крос-платформного програмування",
        student = "Яремчук Сергій, ФЕІ-33с",
        osDescription = RuntimeInformation.OSDescription,
        osVersion = Environment.OSVersion.ToString(),
        processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
        clrVersion = Environment.Version.ToString(),
        runtime = RuntimeInformation.FrameworkDescription,
        baseDirectory = AppContext.BaseDirectory,
        currentDirectory = Environment.CurrentDirectory,
        domain = "Бібліотека (видання, примірники, читачі, видачі)"
    };

    var options = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    Console.WriteLine(JsonSerializer.Serialize(info, options));
    return;
}

Console.WriteLine("CrossApp — практикум з крос-платформного програмування");
Console.WriteLine("Студент: Яремчук Сергій, ФЕІ-33с");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"OC (OSDescription) : {RuntimeInformation.OSDescription}");
Console.WriteLine($"OC (Environment)   : {Environment.OSVersion}");
Console.WriteLine($"Архітектура процесу: {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"Версія .NET (CLR)  : {Environment.Version}");
Console.WriteLine($"Runtime            : {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"Каталог застосунку : {AppContext.BaseDirectory}");
Console.WriteLine($"Поточний каталог   : {Environment.CurrentDirectory}");
Console.WriteLine(new string('-', 52));
Console.WriteLine("Предметна область: Бібліотека (видання, примірники, читачі, видачі)");
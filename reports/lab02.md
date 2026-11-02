# Лабораторна робота 2

Студент: Яремчук Сергій, ФЕІ-33с. Предметна область: бібліотека.

Мета: відокремити спільний код від точки входу, підключити бібліотеку,
перевірити multi-targeting та порівняти режими публікації.

## Структура рішення

```text
CrossApp/
  CrossApp.slnx
  README.md
  reports/lab02.md
  src/
    Core/
      Core.csproj
      EnvironmentInfo.cs
      EnvironmentReport.cs
    Cli/
      Cli.csproj
      Program.cs
```

Використовується наявний формат solution `.slnx`, який підтримується SDK 10.
Залежність одностороння: Cli → Core. Core не посилається на Cli й не друкує дані.
Домовленість на наступні роботи: `Core/Dto` - формати даних (лаба 3),
`Core/Domain` - сутності з поведінкою (лаба 4), `Core/Storage` - сховища (лаба 5).
Ці каталоги створюються разом із першими типами, без порожніх заготовок.

## Хід роботи та команди

Усі команди виконуються з кореня CrossApp у PowerShell:

```powershell
dotnet new classlib -n Core -o src/Core -f net10.0
dotnet sln add src/Core/Core.csproj
dotnet add src/Cli/Cli.csproj reference src/Core/Core.csproj
dotnet sln list
dotnet build
dotnet build src/Core/Core.csproj
dotnet run --project src/Cli
dotnet run --project src/Cli -- --json
```

Згенерований Class1.cs видалено. У Cli.csproj є ProjectReference на Core.
У Core.csproj встановлено `<TargetFrameworks>net8.0;net10.0</TargetFrameworks>`.
SDK 10.0.401 успішно зібрав обидва TFM (0 помилок, 0 попереджень).
Результати: `src/Core/bin/Debug/net8.0/Core.dll` та `src/Core/bin/Debug/net10.0/Core.dll`.
Директива `#if NET10_0_OR_GREATER` змінює BuildNote залежно від TFM бібліотеки.
Cli залишається net10.0 і використовує відповідну збірку Core.

```csharp
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
```

Цей код у Core, бо його можна повторно використати в Api або іншому інтерфейсі.
EnvironmentReport - sealed record з рівністю за значенням; він представляє зібрані дані.
Cli лише викликає Collect, обирає JSON/текст та форматує результат.
DetectRid складає RID із назви ОС та архітектури процесу, ReportedRid повідомляє .NET.

## Публікація і вимірювання

```powershell
dotnet publish src/Cli -c Release -f net10.0 -r win-x64 --self-contained true -o src/Cli/bin/publish/lab02/self-contained
dotnet publish src/Cli -c Release -f net10.0 -r win-x64 --self-contained false -o src/Cli/bin/publish/lab02/framework-dependent
(Get-ChildItem src/Cli/bin/publish/lab02/self-contained -Recurse -File | Measure-Object Length -Sum).Sum / 1MB
(Get-ChildItem src/Cli/bin/publish/lab02/framework-dependent -Recurse -File | Measure-Object Length -Sum).Sum / 1MB
```

Окремі каталоги не дають змішатися файлам runtime двох режимів.
Фактичні результати на Windows x64, SDK 10.0.401, runtime 10.0.12:

| RID | Режим | Байтів | МіБ (байти / 1048576) | Потрібен встановлений runtime |
| --- | --- | ---: | ---: | --- |
| win-x64 | self-contained | 80,588,118 | 76.855 | ні |
| win-x64 | framework-dependent | 203,766 | 0.194 | так, .NET 10 |

Self-contained включає .NET Runtime, тому більший та прив'язаний до RID;
framework-dependent містить застосунок і його залежності, тому потребує встановленого .NET 10.
win-x64 виконуваний файл призначений для Windows x64, а не Linux.
Розмір може змінитися після оновлення SDK/runtime або зміни коду.

## Запуск із publish

```powershell
Push-Location src/Cli/bin/publish/lab02/self-contained
.\Cli.exe
Pop-Location
Push-Location src/Cli/bin/publish/lab02/framework-dependent
.\Cli.exe
Pop-Location
```

Обидва виконувані файли запущено напряму, без dotnet run; перевірено також JSON і RID.
Вивід self-contained:

```text
CrossApp — інформація про середовище
Студент: Яремчук Сергій, ФЕІ-33с
------------------------------------------------------------
ОС                 : Microsoft Windows 10.0.26200
ОС (Environment)   : Microsoft Windows NT 10.0.26200.0
Версія CLR         : 10.0.12
Runtime            : .NET 10.0.12
Архітектура        : X64
RID (визначено)    : win-x64
RID (від .NET)     : win-x64
Каталог застосунку : C:\A_University\1sem3kurs\CrossProg\CrossApp\src\Cli\bin\publish\lab02\self-contained\
Поточний каталог   : C:\A_University\1sem3kurs\CrossProg\CrossApp\src\Cli\bin\publish\lab02\self-contained
Ціль бібліотеки    : збірка Core під net10.0
------------------------------------------------------------
Предметна область: Бібліотека (видання, примірники, читачі, видачі)
```

Для framework-dependent дані середовища ті самі; шляхи вказують на його власний каталог.

## Перевірка та демонстрація

1. `dotnet sln list`: два проєкти, Cli.csproj містить ProjectReference.
2. Відкрити EnvironmentInfo.cs і Program.cs: збирання даних відокремлено від друку.
3. `dotnet build` та `dotnet run --project src/Cli`.
4. Показати обидва TFM у Core.csproj та обидва каталоги збірки Core.
5. Виконати команди publish і запустити Cli.exe з відповідного каталогу.
6. Пояснити таблицю розмірів та потребу у встановленому runtime.

Додатково для самоперевірки тимчасово додано Core → Cli: збірка відхилила циклічну залежність.
Посилання видалено. bin/obj/publish ігноруються Git та не входять до коміту.

## Короткі відповіді для захисту

- ProjectReference посилається на проєкт рішення; PackageReference - на пакет NuGet.
- Core не може посилатися на Cli: залежності утворюють цикл та змішують бібліотеку з інтерфейсом.
- TFM визначає цільову платформу .NET; RID - конкретну ОС і архітектуру.
- Release використовують для поширення; Debug зручний для налагодження.
- Multi-targeting компілює ту саму бібліотеку окремо для кількох TFM.
- Core є бібліотекою без Main: її збирають, а запускають Cli.

Коміт і тег: `lab02`. Репозиторій: https://github.com/s-yaremchuk/CrossApp

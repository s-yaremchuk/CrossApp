# CrossApp — Бібліотека

**Для захисту з додатковими завданнями** використовуй теги `lab02-full` і `lab03-full`. Команди та результати: [лабораторна 2](reports/lab02-extra.md), [лабораторна 3](reports/lab03-extra.md). Теги lab02 та lab03 позначають початкові версії.

Поточний етап: **лабораторна 3**. Звіти: [лаба 2](reports/lab02.md), [лаба 3](reports/lab03.md).
Без аргументів консоль імпортує CSV; інформація про середовище доступна через `--environment` або `--json`.
Розділи 1–5 нижче зберігають історичні результати першої лабораторної.

## Швидкий запуск поточної версії

Потрібні Git і .NET SDK 10. Після клонування команди PowerShell виконуються з кореня `CrossApp`:

```powershell
git clone https://github.com/s-yaremchuk/CrossApp.git
cd CrossApp
dotnet build
dotnet run --project src/Cli -- data/sample.csv
dotnet run --project src/Cli -- --environment
dotnet run --project src/Cli -- --json
```

Без аргументів запускається імпорт `data/sample.csv`. `--json` виводить інформацію
про середовище, а для імпорту книг передай шлях до data/sample.json. Під час першої збірки потрібен доступ до
NuGet для відновлення відсутніх targeting packs, зокрема net8.0 для Core.

| Лабораторна | Коміт | Тег | Докладний звіт |
| --- | --- | --- | --- |
| 1 | [779fc06](https://github.com/s-yaremchuk/CrossApp/commit/779fc06) | [lab01](https://github.com/s-yaremchuk/CrossApp/tree/lab01) | Розділи 1–5 цього README |
| 2 | [db486ce](https://github.com/s-yaremchuk/CrossApp/commit/db486ce) | [lab02](https://github.com/s-yaremchuk/CrossApp/tree/lab02) | [reports/lab02.md](reports/lab02.md) |
| 3 | [028c680](https://github.com/s-yaremchuk/CrossApp/commit/028c680) | [lab03](https://github.com/s-yaremchuk/CrossApp/tree/lab03) | [reports/lab03.md](reports/lab03.md) |

Наскрізний семестровий проєкт з дисципліни «Крос-платформне програмування».

---

## 1. Предметна область: Бібліотека

- **Призначення:** Облік видач примірників видань (книг) читачам та їх своєчасного повернення.
- **Основні сутності:**
  - `Book` — бібліографічне видання (назва, автор, ISBN, рік).
  - `BookCopy` — фізичний примірник видання (інвентарний номер, стан, наявність).
  - `Reader` — читач бібліотеки (читацький квиток, ПІБ, контакти).
  - `Loan` — факт видачі примірника читачу (дата видачі, термін повернення, факт повернення).

---

## 2. Середовище розробки

- **ОС:** Windows 11 Pro x64 (Build 10.0.26200)
- **.NET SDK:** 10.0.401
- **.NET Runtime:** .NET 10.0.12
- **RID машини розробника:** `win-x64`
- **Середовище/Редактор:** Visual Studio Code / Visual Studio 2026

---

Бібліотека підходить для наскрізного проєкту: має пов'язані сутності та зрозумілі
операції видачі й повернення, на яких можна поступово реалізувати DTO, доменні
правила, зберігання, звіти й API.

## 3. Лабораторна 1: створення та запуск

Цей розділ описує стан **тега lab01**. У поточній версії поведінка запуску інша;
актуальні команди наведено на початку README.

Структура після першої роботи: `CrossApp.slnx`, `.gitignore`, `README.md`,
`src/Cli/Cli.csproj` і `src/Cli/Program.cs`. Наявний `.slnx` є форматом solution
і виконує роль `.sln` із прикладів методички.

Послідовність створення проєкту (довідково, повторно в готовому репозиторії не виконувати):

```powershell
dotnet --info
dotnet --list-sdks
git --version
mkdir CrossApp
cd CrossApp
git init
dotnet new gitignore
dotnet new sln -n CrossApp
mkdir src
dotnet new console -n Cli -o src/Cli -f net10.0
dotnet sln add src/Cli/Cli.csproj
dotnet sln list
```

Технічний момент: SDK і runtime мають різні ролі; для збірки net10.0 потрібен SDK 10.
Для кирилиці консоль налаштована на UTF-8, а bin/obj виключено через .gitignore.

Для демонстрації першої роботи спочатку виконайте `git switch --detach lab01`.

### Збірка проєкту:

```bash
dotnet build
```

### Запуск у звичайному режимі (табличний вивід):

```bash
dotnet run --project src/Cli
```

### Запуск із прапорцем `--json` (додаткове завдання):

```bash
dotnet run --project src/Cli -- --json
```

---

## 4. Історичний вивід лабораторної 1

### Звичайний вивід:

Шляхи нижче збережено з початкового README; на іншому комп’ютері вони відрізнятимуться.

```text
CrossApp — практикум з крос-платформного програмування
Студент: Яремчук Сергій, ФЕІ-33с
----------------------------------------------------
OC (OSDescription) : Microsoft Windows 10.0.26200
OC (Environment)   : Microsoft Windows NT 10.0.26200.0
Архітектура процесу: X64
Версія .NET (CLR)  : 10.0.12
Runtime            : .NET 10.0.12
Каталог застосунку : C:\A_University\1sem3kurs\CrossProg\Lab1\src\Cli\bin\Debug\net10.0\
Поточний каталог   : C:\A_University\1sem3kurs\CrossProg\Lab1
----------------------------------------------------
Предметна область: Бібліотека (видання, примірники, читачі, видачі)
```

### JSON-вивід (`--json`):

```json
{"project":"CrossApp — практикум з крос-платформного програмування","student":"Яремчук Сергій, ФЕІ-33с","osDescription":"Microsoft Windows 10.0.26200","osVersion":"Microsoft Windows NT 10.0.26200.0","processArchitecture":"X64","clrVersion":"10.0.12","runtime":".NET 10.0.12","baseDirectory":"C:\\A_University\\1sem3kurs\\CrossProg\\Lab1\\src\\Cli\\bin\\Debug\\net10.0\\","currentDirectory":"C:\\A_University\\1sem3kurs\\CrossProg\\Lab1","domain":"Бібліотека (видання, примірники, читачі, видачі)"}
```

---

## 5. Додаткові завдання лабораторної 1

### 1. Порівняння розмірів Self-Contained публікації

Результати, зафіксовані в лабораторній 1 (тег `lab01`), для двох RID:

- **Windows (win-x64):**
  ```bash
  dotnet publish src/Cli -c Release -r win-x64 --self-contained true
  ```
  Розмір каталогу `publish`: **76.83 MB**
  Запуск бінарника: `.\src\Cli\bin\Release\net10.0\win-x64\publish\Cli.exe`

- **Linux (linux-x64):**
  ```bash
  dotnet publish src/Cli -c Release -r linux-x64 --self-contained true
  ```
  Розмір каталогу `publish`: **78.79 MB**

### 2. Аргумент командного рядка `--json`

Реалізовано перевірку прапорця `--json` за допомогою `System.Text.Json` з виводом структурованих даних одним рядком із підтримкою кодування кирилиці.

## 6. Лабораторна 2: Core + Cli, multi-targeting, публікація

Попередні розділи фіксують результати лабораторної 1. Поточний solution - `CrossApp.slnx`.
Core (`net8.0;net10.0`) отримує дані середовища; Cli (`net10.0`) форматує їх.
Залежність: `Cli → Core`. Докладний [звіт, структура, команди й сценарій захисту](reports/lab02.md).

### Хід роботи та структура solution на тегу lab02

1. Створено class library Core, додано до solution та підключено в Cli.
2. Видалено Class1.cs; збирання інформації про середовище перенесено в Core.
3. У Program.cs залишено виклик Core та форматування тексту/JSON.
4. Увімкнено multi-targeting Core, перевірено збірку обох TFM.
5. Виконано дві публікації, виміряно розміри та запущено Cli.exe напряму.

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

Команди додавання бібліотеки (вже виконані під час реалізації):

```powershell
dotnet new classlib -n Core -o src/Core -f net10.0
dotnet sln add src/Core/Core.csproj
dotnet add src/Cli/Cli.csproj reference src/Core/Core.csproj
```

У Cli.csproj є `<ProjectReference Include="..\Core\Core.csproj" />`.
Core не посилається на Cli. Зворотне посилання було тимчасово перевірено:
збірка виявила цикл, після чого посилання видалено.

У Core.csproj встановлено `<TargetFrameworks>net8.0;net10.0</TargetFrameworks>`.
Multi-targeting виконано успішно з SDK 10.0.401: обидва Core.dll збираються без
попереджень у `src/Core/bin/Debug/net8.0` та `src/Core/bin/Debug/net10.0`.
Cli залишається net10.0. `#if NET10_0_OR_GREATER` задає BuildNote для відповідного TFM.

Збирання даних розташоване у Core для повторного використання майбутніми Api/Web;
EnvironmentReport є record із результатами, а EnvironmentInfo відповідає за їх отримання:

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

Каталоги Core: `Core/Dto` - формати даних з лабораторної 3, `Core/Domain` - сутності з лабораторної 4,
`Core/Storage` - сховища з лабораторної 5. Створюються разом із першими типами.

| RID | Режим | Байтів | МіБ | Потрібен runtime |
| --- | --- | ---: | ---: | --- |
| win-x64 | self-contained | 80,588,118 | 76.855 | ні |
| win-x64 | framework-dependent | 203,766 | 0.194 | так, .NET 10 |

Self-contained включає runtime; framework-dependent потребує встановленого .NET 10.
Ці розміри виміряно для стану `lab02`, Windows x64, SDK 10.0.401, runtime 10.0.12.
Для інших версій коду/SDK вони можуть відрізнятися; 1 МіБ = 1 048 576 байтів.

### Збірка, публікація та запуск лабораторної 2

Перед цими командами перейдіть на `lab02`, щоб без аргументів отримати звіт середовища:

```powershell
git switch --detach lab02
dotnet sln list
dotnet build
dotnet build src/Core/Core.csproj
dotnet run --project src/Cli
dotnet publish src/Cli -c Release -f net10.0 -r win-x64 --self-contained true -o src/Cli/bin/publish/lab02/self-contained
dotnet publish src/Cli -c Release -f net10.0 -r win-x64 --self-contained false -o src/Cli/bin/publish/lab02/framework-dependent
.\src\Cli\bin\publish\lab02\self-contained\Cli.exe
.\src\Cli\bin\publish\lab02\framework-dependent\Cli.exe
```

Вимірювання та запуск саме з каталогів publish:

```powershell
(Get-ChildItem src/Cli/bin/publish/lab02/self-contained -Recurse -File | Measure-Object Length -Sum).Sum / 1MB
(Get-ChildItem src/Cli/bin/publish/lab02/framework-dependent -Recurse -File | Measure-Object Length -Sum).Sum / 1MB
Push-Location src/Cli/bin/publish/lab02/self-contained
.\Cli.exe
Pop-Location
Push-Location src/Cli/bin/publish/lab02/framework-dependent
.\Cli.exe
Pop-Location
```

Окремі каталоги запобігають змішуванню двох режимів. Обидва Cli.exe перевірено.
Вивід self-contained на етапі lab02:

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

Під час запуску framework-dependent змінюються шляхи до його каталогу;
дані про ОС, runtime та RID збігаються. bin/obj/publish не входять до Git.
Після перемикання етапу публікацію слід зібрати повторно.

Сценарій захисту: показати solution і ProjectReference, пояснити розподіл Core/Cli,
запустити консоль, обидва publish та порівняти таблицю розмірів і потребу у runtime.

## 7. Лабораторна 3: records, pattern matching, CSV

Повний [звіт із ходом роботи, поясненнями патернів, виводом та сценарієм захисту](reports/lab03.md).

```text
src/Core/Dto/BookDto.cs            - книга, необов'язковий Author
src/Core/Dto/ReaderDto.cs          - читач, необов'язковий Email
src/Core/Dto/ImportResult.cs       - прийняті записи та помилки
src/Core/Import/BookCsvImporter.cs - імпорт і ParseLine зі switch expression
src/Cli/Program.cs                - аргументи, виклики Core, вивід
```

### Record-типи та nullable

```csharp
public sealed record BookDto(
    string Id, string Isbn, string Title, int Year, string? Author = null);
public sealed record ReaderDto(string Id, string FullName, string? Email = null);
public sealed record ImportResult<T>(IReadOnlyList<T> Items, IReadOnlyList<string> Errors);
```

- BookDto: Id - обов'язковий рядковий ключ (наприклад B-001); Isbn і Title обов'язкові,
  тому non-nullable й додатково перевіряються на порожній вміст. Year - int, від 1450
  до поточного року включно. Author може бути невідомий; у поточному CSV його немає,
  тому значення null є очікуваним, а не помилкою.
- ReaderDto: Id та FullName потрібні для ідентифікації читача; Email може бути відсутнім.
  Цей DTO описує другий вид даних бібліотеки, його імпорт у цій роботі не реалізується.
- ImportResult<T>: Items та Errors завжди існують, навіть якщо порожні.
  Імпортер повертає колекції через AsReadOnly, щоб викликач не змінював результат.

Record обрано, бо рядок файлу представляє дані, які зручно порівнювати за значенням,
а не об'єкт із власним життєвим циклом. Наприклад, `book with { Author = "Автор" }`
створює копію, а початковий record залишається без змін.

### Розбір через switch expression

```csharp
    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            { Length: < 4 } => new ParseFailed($"очікую 4 колонки, отримав {parts.Length}"),
            ["", _, _, _] or [_, "", _, _] or [_, _, "", _]
                => new ParseFailed("ідентифікатор, ISBN або назва порожні"),
            [var id, var isbn, var title, var text]
                when int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int year)
                     && year is >= 1450 && year <= DateTime.Now.Year
                => new ParseOk(new BookDto(id, isbn, title, year)),
            [_, _, _, var text]
                => new ParseFailed($"рік '{text}' має бути цілим числом від 1450 до {DateTime.Now.Year}"),
            _ => new ParseFailed($"занадто багато колонок: {parts.Length}")
        };
    }
```

Використано такі патерни:

- Властивостей і реляційний `{ Length: < 4 }`: недостатня кількість колонок.
- Списків та константний `""`: порожнє обов'язкове поле; `or` об'єднує три варіанти.
- Списків із `var` і охоронною умовою `when`: перевірка TryParse та меж року.
- Реляційний `year is >= 1450`: нижня межа допустимого року.
- Discard `_`: будь-яке значення колонки або остання гілка для зайвих колонок.

Приватні ParseOk і ParseFailed успадковують ParseOutcome. У Load патерн типу
`case ParseOk ok` додає книгу, а `case ParseFailed failed` - причину з номером рядка.
Одна помилка не перериває імпорт. Файлові помилки обробляє Cli й повертає код 1.
У Program.cs немає Split або int.Parse: лише аргументи, виклик Core та вивід.

### Формат даних

CSV: UTF-8, роздільник `;`, колонки `id;isbn;title;year`, необов'язковий заголовок.
Author відсутній у CSV і залишається null. Порожні рядки та коментарі `#` ігноруються.
Рік - ціле число від 1450 до поточного року; використано InvariantCulture.
Назви з `;`, CSV-лапки та багаторядкові поля не підтримуються в цьому простому форматі.
Пошкоджені рядки повертаються як помилки з фізичними номерами рядків.
`data/valid.csv` містить 10 правильних записів, а `data/sample.csv` - ті самі 10
і 3 навмисно пошкоджені рядки: недостатньо колонок, нечисловий рік, порожня назва.

### Хід роботи та перевірка

Створено DTO → ImportResult → Load/ParseLine у Core → CSV із тестовими дефектами →
виклик і вивід у Cli → перевірка трьох обов'язкових сценаріїв.
Namespace файлові (`Core.Dto`, `Core.Import`); парсер відсутній у Program.cs.

```powershell
git switch --detach lab03
dotnet build
dotnet run --project src/Cli
dotnet run --project src/Cli -- data/sample.csv
dotnet run --project src/Cli -- data/valid.csv
dotnet run --project src/Cli -- data/missing.csv
$LASTEXITCODE
dotnet run --project src/Cli -- --environment
```

Очікувано: sample.csv - 10 книг, 3 помилки (рядки 12, 13, 14);
valid.csv - 10 книг, 0 помилок; неіснуючий файл - повідомлення з повним шляхом і код 1.
При успішному читанні, включно з частково пошкодженим CSV, код виходу 0.
Консоль показує перші 5 книг і всі причини пропуску. Перевірено 14 сценаріїв CLI.

Правильний файл, код виходу 0:

```text
Завантажено записів: 10
  B-001  9780141439518 Гордість і упередження         1813
  B-002  9780140449136 Одіссея                        1996
  B-003  9780141187761 1984                           1949
  B-004  9780141439600 Джейн Ейр                      1847
  B-005  9780142437230 Дон Кіхот                      1605
Пропущено рядків: 0
```

Файл із пошкодженнями, код виходу 0 (імпорт виконаний, дефекти перелічені):

```text
Завантажено записів: 10
  B-001  9780141439518 Гордість і упередження         1813
  B-002  9780140449136 Одіссея                        1996
  B-003  9780141187761 1984                           1949
  B-004  9780141439600 Джейн Ейр                      1847
  B-005  9780142437230 Дон Кіхот                      1605
Пропущено рядків: 3
  ! рядок 12: очікую 4 колонки, отримав 3
  ! рядок 13: рік 'невідомо' має бути цілим числом від 1450 до 2026
  ! рядок 14: ідентифікатор, ISBN або назва порожні
```

Неіснуючий файл, stderr і код виходу 1:

```text
Файл не знайдено: C:\A_University\1sem3kurs\CrossProg\CrossApp\data\missing.csv
```

Сценарій захисту: показати records і ParseLine, пояснити патерни, запустити обидва CSV,
дописати один зламаний рядок і показати нову помилку, потім перевірити неіснуючий шлях.
Після демонстрації видалити лише власний доданий рядок перед перемиканням тегів.

### Перемикання для захисту

З кореня репозиторію, за чистого `git status`. На іншому комп'ютері після клонування
або оновлення виконайте `git fetch origin --tags`:

```powershell
git switch --detach lab01
dotnet run --project src/Cli

git switch --detach lab02
dotnet run --project src/Cli
# Команди publish та запуск Cli.exe: розділ 6 або reports/lab02.md

git switch --detach lab03
dotnet run --project src/Cli -- data/sample.csv

git switch main
```

Теги зберігають окремі завершені етапи. У detached HEAD можна демонструвати код;
для подальшої роботи поверніться на main. Після редагування прикладів на захисті
приберіть власні демонстраційні зміни перед наступним перемиканням.

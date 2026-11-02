# CrossApp — Бібліотека

Поточний етап: **лабораторна 3**. Звіти: [лаба 2](reports/lab02.md), [лаба 3](reports/lab03.md).
Без аргументів консоль імпортує CSV; інформація про середовище доступна через `--environment` або `--json`.
Розділи 1–5 нижче зберігають історичні результати першої лабораторної.

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

## 3. Інструкція із запуску

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

## 4. Вивід програми

### Звичайний вивід:
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

## 5. Додаткові завдання

### 1. Порівняння розмірів Self-Contained публікації
Виконано публікацію під дві цільові платформи (RID):
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

Каталоги Core: `Dto` - формати даних з лабораторної 3, `Domain` - сутності з лабораторної 4,
`Storage` - сховища з лабораторної 5. Створюються разом із першими типами.

| RID | Режим | Байтів | МіБ | Потрібен runtime |
| --- | --- | ---: | ---: | --- |
| win-x64 | self-contained | 80,588,118 | 76.855 | ні |
| win-x64 | framework-dependent | 203,766 | 0.194 | так, .NET 10 |

Self-contained включає runtime; framework-dependent потребує встановленого .NET 10.

```powershell
dotnet build
dotnet run --project src/Cli
dotnet publish src/Cli -c Release -f net10.0 -r win-x64 --self-contained true -o src/Cli/bin/publish/lab02/self-contained
dotnet publish src/Cli -c Release -f net10.0 -r win-x64 --self-contained false -o src/Cli/bin/publish/lab02/framework-dependent
.\src\Cli\bin\publish\lab02\self-contained\Cli.exe
.\src\Cli\bin\publish\lab02\framework-dependent\Cli.exe
```

### Перемикання між лабораторними

Перед перемиканням `git status` має бути чистим. Теги позначають завершені роботи:

```powershell
git switch --detach lab01
git switch --detach lab02
dotnet build
dotnet run --project src/Cli
git switch main
```

Кожен наступний коміт продовжує попередню лабораторну. bin/obj/publish не версіонуються;
публікацію потрібного етапу слід повторно зібрати після перемикання.


## 7. Лабораторна 3: records, pattern matching, CSV

Повний [звіт із ходом роботи, поясненнями патернів, виводом та сценарієм захисту](reports/lab03.md).

```text
src/Core/Dto/BookDto.cs            - книга, необов'язковий Author
src/Core/Dto/ReaderDto.cs          - читач, необов'язковий Email
src/Core/Dto/ImportResult.cs       - прийняті записи та помилки
src/Core/Import/BookCsvImporter.cs - імпорт і ParseLine зі switch expression
src/Cli/Program.cs                - аргументи, виклики Core, вивід
```

CSV: UTF-8, роздільник `;`, колонки `id;isbn;title;year`, необов'язковий заголовок.
Author відсутній у CSV і залишається null. Порожні рядки та коментарі `#` ігноруються.
Рік - ціле число від 1450 до поточного року; використано InvariantCulture.
Назви з `;`, CSV-лапки та багаторядкові поля не підтримуються в цьому простому форматі.
Пошкоджені рядки повертаються як помилки з фізичними номерами рядків.

```powershell
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

### Перемикання для захисту

З кореня репозиторію, за чистого `git status`:

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

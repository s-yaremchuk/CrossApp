# CrossApp — Бібліотека

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

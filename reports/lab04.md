# Лабораторна 4 — доменна модель бібліотеки

## Мета і хід роботи

BookCopy представляє фізичний примірник. Кожна Loan пов’язує один примірник з одним читачем і фіксує дату видачі. Реєстр зберігає історію та рахує відкриті видачі читача. Коли читач повертає книгу, Loan.Close одночасно фіксує дату й звільняє примірник.

### 1. Створення і зміна стану

Конструктори закриті. BookCopy.Create перевіряє інвентарний номер та обов’язкові поля книги. Нижче наведено перевірку ISBN із фабрики.

~~~csharp
if (string.IsNullOrWhiteSpace(book.Isbn))
    throw new ArgumentException($"Для примірника {id} потрібен ISBN.", nameof(book));
~~~

Під час відкриття видачі Loan.Open викликає BookCopy.Issue. Перевірка перед присвоєнням зберігає початковий стан у разі відмови.

~~~csharp
internal void Issue()
{
    if (IsIssued)
        throw new InvalidOperationException($"Примірник {Id} вже видано.");
    IsIssued = true;
}
~~~

| Правило | Виняток | Метод |
| --- | --- | --- |
| Заповнені номер примірника, ISBN, назва і код книги | ArgumentException | BookCopy.Create |
| Рік у межах від 1450 до поточного | ArgumentOutOfRangeException | BookCopy.Create |
| Номер видачі та код читача заповнені | ArgumentException | LoanRegister.Issue / Loan.Open |
| Номер видачі унікальний, примірник вільний | InvalidOperationException | LoanRegister.Issue |
| До п’яти відкритих видач на читача | InvalidOperationException | LoanRegister.Issue |
| Повернення в день видачі або пізніше | ArgumentOutOfRangeException | Loan.Close |
| Закриття можливе лише для відкритої видачі | InvalidOperationException | Loan.Close |

### 2. Перетворення сутності на DTO

BookDto із третьої роботи переносить бібліографічні дані. BookCopy має власний інвентарний номер і стан. Дві фізичні книги можуть мати однаковий ISBN. LoanDto зберігає дати й ідентифікатори. ToDto створює знімок, а FromDto відтворює створення та закриття через доменні методи. Пошкоджена дата у знімку спричиняє ту саму відмову, що й у звичайному сценарії.

### 3. Результати запуску

~~~text
Видано C-01: Open, читач R-1.
InvalidOperationException: Примірник C-01 вже видано.
Після відмов: Open, примірник видано: True.
InvalidOperationException: Видачу L-01 вже закрито.
Відновлено L-01: Closed, повернено 2026-10-13.
~~~

### 4. Додаткові завдання

Імпорт. CopyImporter.FromBooks приймає ImportResult<BookDto>. Для набору примірників колонка id є інвентарним номером. Імпортер зберігає помилки CSV і додає доменні помилки з фізичними номерами рядків. У data/copies.csv прийнято один примірник. Рядок 3 має замало колонок, у рядку 4 повторено C-01.

Ліміт читача. Після п’ятої відкритої видачі реєстр відхиляє шосту. Після одного повернення читач отримує N-6. Правило належить реєстру або сервісу, оскільки окремий примірник не знає про інші видачі читача.

Переходи стану. LoanStatus має значення Open та Closed. Вираз switch у Close дозволяє перехід Open → Closed після перевірки дати. Повторне закриття дає InvalidOperationException.

## План демонстрації на захисті

### 1. Відкрити роботу та показати сутності

~~~powershell
Set-Location C:/A_University/1sem3kurs/CrossProg/CrossApp
git switch --detach lab04
dotnet build
Get-Content -Encoding utf8 src/Core/Domain/BookCopy.cs
~~~

Покажіть приватний конструктор, поле _book, властивість Id лише для читання та IsIssued з приватним set.

### 2. Запустити основні й додаткові сценарії

~~~powershell
dotnet run --no-build --project src/Cli -- --loans
Get-Content -Encoding utf8 data/copies.csv
Get-Content -Encoding utf8 src/Core/Domain/LoanRegister.cs
Get-Content -Encoding utf8 src/Core/Domain/Loan.cs
~~~

Покажіть повторну видачу, раннє повернення та незмінений стан Open. Потім покажіть відмову при шостій видачі, успіх після повернення й обидві помилки імпорту. У Close знайдіть switch, у FromDto — виклики Open і Close.

### 3. Перевірити доступ до властивості

У PowerShell створіть тимчасовий файл із прямим присвоєнням і зберіть Cli. Після повідомлення CS0272 видаліть демонстраційний файл та повторіть збірку.

~~~powershell
$probe = "src/Cli/Probe$([guid]::NewGuid().ToString('N')).cs"
try {
    'class Probe { void Try(Core.Domain.BookCopy c) { c.IsIssued = true; } }' |
        Set-Content -LiteralPath $probe
    dotnet build src/Cli
} finally { Remove-Item -LiteralPath $probe }
dotnet build
Select-String -Path README.md -Pattern '^## Лабораторна 4' -Context 0,20
git switch main
~~~

### Запитання та висновок

Що таке інваріант? Це правило, яке зберігається після кожної дозволеної операції. Навіщо закритий конструктор? Фабрика перевіряє дані до створення. Як розрізнити винятки? Некоректна дата є помилкою аргументу, повторне закриття є помилкою стану.

Успішне повернення змінює стан видачі та примірника. Відхилена операція зберігає попередній стан. Відновлення з DTO застосовує ті самі перевірки.

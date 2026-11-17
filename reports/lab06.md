# Лабораторна 6 — доменні події бібліотеки

## Мета та хід роботи

LendingService повідомляє про додавання, видачу, повернення та вилучення примірника. CatalogChangedEventArgs містить Id, Kind, IsIssued і час At. Доменні методи завершують перевірки, сховище зберігає стан, потім сервіс викликає OnChanged.

~~~csharp
public event EventHandler<CatalogChangedEventArgs>? Changed;
private void OnChanged(BookCopy copy, CatalogChangeKind kind)
{
    var change = new CatalogChangedEventArgs(copy.Id, kind, copy.IsIssued);
    Changed?.Invoke(this, change);
    sink?.Notify(change);
}
~~~

Cli підписує локальний метод Log і збережену лямбду count. Перший друкує повідомлення та дописує його в logs/app.log. Другий захоплює змінну changes й збільшує її. Обробники викликаються в порядку підписки. Логер обробляє помилки доступу до файла, тому така помилка дозволяє наступному підписнику виконатися.

Після додавання та видачі changes дорівнює 2. Повторна видача відхиляється, changes лишається 2. Повернення дає третю подію. Після -= Log наступна видача збільшує changes до 4, а рядок у журналі не з’являється. Після -= count повернення змінює стан, changes лишається 4.

~~~text
Added <id> issued=False
Issued <id> issued=True
Returned <id> issued=False
~~~

Це три рядки app.log від одного запуску. Час та випадковий id залежать від запуску. Наприкінці програма друкує абсолютний шлях журналу.

## Додаткові завдання

INotificationSink приймається сервісом через конструктор. ConsoleSink та FileSink створюються в Program.cs. Подія дозволяє підписати декілька спостерігачів і керувати підписками під час роботи. Інтерфейс робить одержувача явною залежністю конструктора. Обидва варіанти залишають конкретне виведення у Cli.

BookCsvImporter.Load має необов’язковий параметр Action<string> onSkipped. Для кожного пропущеного рядка він зберігає помилку та передає повідомлення callback-функції. Попередні виклики Load(path) працюють без callback.

Search приймає Func<BookCopy, bool>, бо потребує результату true або false для відбору. Action отримує повідомлення без результату. Подія повідомляє про вже виконану зміну.

Обробник async-події з EventHandler повертає void. Видавець не може очікувати завершення такого обробника. Виняток після await потребує оброблення всередині async void. Для послідовних операцій із очікуванням придатний окремий контракт із Task.

## План демонстрації на захисті

### 1. Відкрийте роботу та знайдіть оголошення

~~~powershell
Set-Location C:/A_University/1sem3kurs/CrossProg/CrossApp
git switch --detach lab06
dotnet build
Get-Content src/Core/Events/CatalogChangedEventArgs.cs
Get-Content src/Core/Services/LendingService.cs
~~~

Скажіть «Подія описує вже виконану зміну примірника. Тут є номер, тип зміни, новий стан та час». У LendingService покажіть event, OnChanged і рядок OnChanged після _store.Update. Поясніть «Якщо перевірка або запис завершиться винятком, цей рядок не виконається».

### 2. Покажіть підписки та запустіть сценарій

~~~powershell
Get-Content -Encoding UTF8 src/Cli/EventDemo.cs
dotnet run --no-build --project src/Cli -- --events
~~~

Покажіть service.Changed += Log та += count. Скажіть «Спочатку працює логер. Потім лямбда збільшує захоплену змінну changes». У виводі знайдіть Added, Issued та «Лічильник після логера». Перед рядком відмови лічильник дорівнює 2. Після відмови він також дорівнює 2. Поясніть «Невдала повторна видача не породила подію».

### 3. Зіставте журнал та відписки

~~~powershell
Get-Content src/Cli/bin/Debug/net10.0/logs/app.log -Tail 3
Select-String -Path src/Cli/EventDemo.cs -Pattern 'Changed -=' -Context 0,3
~~~

Покажіть три останні рядки Added, Issued, Returned. У коді після -= Log є ще одна видача. У консолі для неї працює лише лічильник і друкується видано=True. Після другої відписки повернення дає видано=False та незмінений лічильник 4. Скажіть «Відписка змінює одержувачів повідомлення. Доменна операція продовжує змінювати стан».

### 4. Продемонструйте делегати та обидва sink

~~~powershell
Select-String -Path src/Cli/EventDemo.cs -Pattern 'service.Search|BookCsvImporter.Load' -Context 0,1
dotnet run --no-build --project src/Cli -- --events --sink-console
dotnet run --no-build --project src/Cli -- --events --sink-file
Get-Content src/Cli/bin/Debug/net10.0/logs/sink.log -Tail 5
Get-Content src/Core/Abstractions/INotificationSink.cs
git switch main
~~~

Покажіть результат пошуку вільних примірників. Скажіть «Func повертає bool для кожного примірника». Знайдіть три рядки Action для помилок CSV. Поясніть «Action отримує текст кожного пропуску». Для ConsoleSink покажіть префікс DI. Для FileSink відкрийте sink.log. Там п’ять повідомлень від одного запуску, оскільки залежність sink продовжує отримувати зміни після відписки подій.

### Питання та висновок

Зовнішній код може підписуватися на event, а піднімати її може клас-видавець. Оператор ?.Invoke дозволяє виклик за відсутності підписників. Якщо довільний підписник кине необроблений виняток, наступні підписники не виконаються, хоча стан уже збережено. Небезпечну роботу слід обробляти всередині підписника. На тижні 10 одержувачем стане логер API, на тижні 12 — компонент інтерфейсу користувача.

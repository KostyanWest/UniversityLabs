Лабораторные работы № 1, 3, 4, 5, 6 и 8
=====================================

Lab 1
-----

* Всё `namespase Sem2Lab1` - игра 2048 (класс `Program` в `Sem2Lab1.cs`).

Lab 3
-----

* Созданы два класса: `Field` (`Field.cs`) и `Cell` (`Cell.cs`).
* В классе `Cell` есть свойство `Value`.
* В классе `Field` есть индексатор.
* Класс `Filed` имеет статический метод `Create`.
* В классе `Filed` перегружен метод `CheckValueChange`.

Lab 4
-----

* В классе `FieldDesktop` (`FieldDesktop.cs`) и `CellDesktop` (`CellDesktop.cs`) используется библиотека GDI для рисования игры на рабочем столе.
* `Sem2Lab4.cs` использует динамическую библиотеку `Sem2Lab4dll.dll`, написанную на C++ (исходный код - `dllmain.cpp`). Функции `IntegralLeft` и `IntegralRight` используют соглашение о вызове StdCall, а `IntegralCenter` и `IntegralTrapec` - Cdecl.

Lab 5
-----

* От абстрактного класса `Field` созданы производные классы `FieldConsole` (`FieldConsole.cs`) и `FiledDesktop` (`FiledDesktop.cs`); от абстрактного класса `Cell` созданы производные классы `CellConsole` (`CellConsole.cs`) и `CellDesktop` (`CellDesktop.cs`).
* Созданы перечисления `EDirection` и `EFieldFlags` (`Field.cs`). Первое используется в методе `Filed.Move`, второе - в `Field.Create`.
* Создана структура `Rect` (`FiledDesktop.cs`), используется в работе с библиотекой GDI.

Lab 6
-----

* Создан инерфейс `ISimpleGame` (`ISimpleGame.cs`), реализующийся в классе `Field`.
* Класс `FieldDesktop` реализует стандартный интерфейс `IDisposable`.
* Стандартные интерфейсы для преобразования и сравнения объектов используются в [Lab7](../Sem2Lab7).

Lab 8
-----

* Класс `Field` имеет события `Win` и `Lose` и реализует событие `ISimpleGame.GameEnd`, обрабатываемые анонимным методом в классе `Program` (`Sem2Lab1.cs`).
* Класс `Cell` имеет событие `ValueChange`, обрабатываемое методом `Field.CheckValueChange`.
* Метод `Field.Move` и свойство `Cell.Value` генерируют исключение `ArgumentOutOfRangeException`; методы `FiledDesktop.Render` и `CellDesktop.Render` генерируют исключение `ObjectDisposedException`.
* Обработка исключений есть в [Lab7](../Sem2Lab7).
* Методы `Integral*` в классе `Program` (`Sem2Lab4.cs`) принимают в качестве первого параметра делегат.
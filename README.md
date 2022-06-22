# ExGTF
ExGTF - расширение файла предназначенное для создания файла в процессе выполнения программы.

### Описание работы
Для работы с ExGTF необходимо 2 основные составляющие: шаблон с расширением .exgtf и словарь значений
Для начала работы необходимо выполнить инициализацию ExGTFReader
```sh
var pathToTemplate = "../temp.exgtf";
var dictionary = new Dictionary<string, object>
            {
                { "userName", "dima" }, 
                { "password", "123" }, 
                { "users", new[]{"1", "2"} }
            });
            
var er = new ExGTFReader(pathToTemplate, dictionary);
```
После чего вызвать функцию построения файла:
```sh
er.Create(pathToCompeteFile); //где pathToCompeteFile - путь до итогого выражения
```
Если нужно специфическое название файла и нет возможности вставить в шаблон, можно вызвать создание следующим образом:
```sh
er.Create(pathToCompeteFile, fileName); //где pathToCompeteFile - путь до итогого выражения
```

### Шаблон
Пример шаблона выглядит следующим образом:
```sh
@fileName@.cs
using System;

namespace EGTF.Reader
{
    public class Class1
    {
        //Console.WriteLine("Hello World");
        //<#userName#>
        //Console.WriteLine("Дмитрий-123");
        Console.WriteLine("<$userName$>-<$password$>");
        [#Console.WriteLine("<$users$>-<$users$>");#]
        [<i:!{1..10}!Console.WriteLine("{!i!}");>]
        [##<i:%$users$%
        //hello
        Console.WriteLine("{%i%}-<$userName$>");
        >##]
    }
}
```
Где результатом будет:
```sh
using System;

namespace EGTF.Reader
{
    public class Class1
    {
        //Console.WriteLine("Hello World");
        Console.WriteLine("123")
        //Console.WriteLine("Дмитрий-123");
        Console.WriteLine("dima-123");
        Console.WriteLine("1-1");
        Console.WriteLine("2-2");
        Console.WriteLine("1");
        Console.WriteLine("2");
        Console.WriteLine("3");
        Console.WriteLine("4");
        Console.WriteLine("5");
        Console.WriteLine("6");
        Console.WriteLine("7");
        Console.WriteLine("8");
        Console.WriteLine("9");
        //hello
        Console.WriteLine("1-1");
        //hello
        Console.WriteLine("2-2");
    }
}
```

#### Описание функций
* ```<#userName#>``` -> подставляет значение из словаря и вставляет на новую строку
* ```<$userName$>``` -> вставляет значение в строку, без переноса
* ```[#<i:<$users$>Console.WriteLine("{%i%}-{%i%}");>#]``` -> делает обход массива users
* ```[<i:!{1..10}!Console.WriteLine("{!i!}");>]``` -> вставляет строки на основе массива указанного в !{n..m}! от n до m
* ```^!``` -> комментарий. Не отображается в итоговом файле

Для обхода массива желательно использовать следующий вариант:
 ```
[##<i:%$users$%
//hello
Console.WriteLine("{%i%}-<$userName$>");
>##]
``` 
Где {%i%} -> внутренний элемент массива, а <$userName$> - значение из словаря

Для работы с условиями можно использовать следующий вариант:
```
[??{<$table_rus_name$>==%123%}
//Да
??]
[??{<$table_rus_name$>!=%123%}
//Нет
??]
```
где ```<$name$>``` - Название переменной в словаре, а ```%123%``` - локальное значение 
Прошу прощения! Вот тесты в формате, как в примере:

# Список тестов

**Идентификаторы и ключевые слова**
- Разбор идентификаторов: `variable _value item1`
- Ключевые слова: `begin end var if then else for while do break continue function return write read struct arr`
- Объявление переменных: `var pi : number = 3.14; var name : string = "Vladimir"; var l : bool = false;` — `var`, `string`, `boolean`, `number`
- Условный оператор и else: `if (age >= 18) then begin write("Hello"); end else begin write("Hi"); end` — `if`, `then`, `begin`, `end`, `else`
- Цикл FOR с break: `for i from 1 to 10 do begin break; end` — `for`, `from`, `to`, `do`, `begin`, `end`, `break`
- Цикл WHILE с continue: `while (true) do begin continue; end` — `while`, `do`, `begin`, `end`, `continue`
- Объявление функции с return: `function add(a : number, b : number) : number begin return a + b; end` — `function`, `begin`, `end`, `return`
- Ввод-вывод: `write("Hello"); read(input);` — `write`, `read`
- Объявления массива: `var numbers : arr[10] of number;` — `var`, `arr`, `of`
- Объявление структуры: `var point : struct { x : number, y : number };` — `var`, `struct`

**Литералы**
- Разбор литералов целых чисел: `0 999 025`
- Разбор вещественных литералов: `3.14 02.0`
- Разбор строк: `"Count: " "Hello" ""`
- Разбор escape-последовательностей: `"path\\file" "line1\\nline2" "tab\\tchar" "quote\\\"end" "carriage\\r"`
- Логические значения: `true false`
- Литералы массивов: `[]`
- Литералы структур: `{ x : 10, y : 20 }` — скобки `{}`, разделители `:`, `,`

**Операторы, разделители и скобки**
- Разбор разделителей: `; , :`
- Арифметические операторы: `a + b - c * d / e % f ^ g` — `+`, `-`, `*`, `/`, `%`, `^`
- Логические операторы: `x || y @ z !flag` — `||`, `@`, `!`
- Операторы сравнения: `a == b != c < d > e <= f >= g` — `==`, `!=`, `<`, `>`, `<=`, `>=`
- Оператор присваивания: `x = y` — `=`
- Выражение в скобках: `(true)` — скобки `()`
- Доступ к полям структуры: `myObject.field` — оператор `.`
- Индексация массива: `myArray[0]` — скобки `[]`

**Пробелы и комментарии**
- Пропуск пробельных символов: `' '`, `'\t'`, `'\r'`, `'\n'`, `'\f'`
- Однострочные комментарии: `## это комментарий`
- Многострочные комментарии: `/# комментарий на несколько строк #/`
- Вложенные комментарии: `/# внешний /# вложенный #/ комментарий #/`
# Список тестов

**Идентификаторы и ключевые слова**

- [x] Разбор идентификаторов: `variable _value item1`
- [x] Объявление переменных: `var pi : number = 3.14; var name : string = "Vladimir"; var l : bool = false;` - `var`,
  `string`, `boolean`, `number`
- [x] Условный оператор и else: `if (age >= 18) then begin write("Hello"); end else begin write("Hi"); end` - `if`,
  `then`, `begin`, `end`, `else`
- [x] Цикл FOR с break: `for i from 1 to 10 do begin break; end` - `for`, `from`, `to`, `do`, `begin`, `end`, `break`
- [x] Цикл WHILE с continue: `while (true) do begin continue; end` - `while`, `do`, `begin`, `end`, `continue`
- [x] Объявление функции с return: `function add(a : number, b : number) : number begin return a + b; end` - `function`,
  `begin`, `end`, `return`
- [x] Ввод-вывод: `write("Hello"); read(input);` - `write`, `read`
- [x] Объявления массива: `var numbers : arr[10] of number;` - `var`, `arr`, `of`
- [x] Объявление структуры: `var point : struct { x : number, y : number };` - `var`, `struct`
- [x] Разбор индентификатора, начинающегося с цифры: `1fdas`
- [x] Разбор индентификатора на кириллице: `sdкоммент`

**Литералы**

- [x] Разбор литералов целых чисел: `0 999 025`
- [x] Разбор вещественных литералов: `3.14 02.0`
- [x] Разбор строк: `"Count: " ""`
- [x] Разбор escape-последовательностей: `"path\\file" "line1\\nline2" "tab\\tchar" "quote\\\"end" "carriage\\r"`
- [x] Логические значения: `true false`

**Операторы, разделители и скобки**

- [x] Арифметические операторы: `a + b - c * d / e % f ^ g` - `+`, `-`, `*`, `/`, `%`, `^`
- [x] Логические операторы: `x || y @ z !flag` - `||`, `@`, `!`
- [x] Операторы сравнения: `a == b != c < d > e <= f >= g` - `==`, `!=`, `<`, `>`, `<=`, `>=`
- [x] Оператор присваивания: `x = y` - `=`
- [x] Выражение в скобках: `(true)` - скобки `()`
- [x] Доступ к полям структуры: `myObject.field` - оператор `.`
- [x] Индексация массива: `myArray[0]` - скобки `[]`
- [x] Литералы структур: `{ x : 10, y : 20 };` - скобки `{}`, разделители `:`, `,`, `;`

**Пробелы и комментарии**

- [x] Пропуск пробельных символов: `' '`, `'\t'`, `'\r'`, `'\n'`, `'\f'`
- [x] Однострочные комментарии: `## это комментарий`
- [x] Многострочные комментарии: `/# комментарий на несколько строк #/`
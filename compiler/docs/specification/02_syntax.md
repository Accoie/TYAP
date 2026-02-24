# Грамматика языка Pascal++
## Операторы

Арифметические операторы:

| Символы     | Операция                  |
|-------------|---------------------------|
| `+`         | Сложение или унарный "+"  |
| `-`         | Вычитание или унарный "-" |
| `*`         | Умножение                 |
| `/`         | Деление                   |
| `%`         | Остаток от деления        |
| `^`         | Возведение в степень      |

Логические операторы:

| Символы     | Операция                  |
|-------------|---------------------------|
| `@`         | логическое "и"            |
| `\|\|`      | логическое "или"          |
| `!`         | логическое отрицание      |

Операторы сравнения:

| Символы     | Операция                  |
|-------------|---------------------------|
| `==`        | равно                     |
| `!=`        | не равно                  |	
| `<`         | меньше                    |
| `>`         | больше                    |
| `<=`        | меньше или равно          |
| `>=`        | больше или равно          |

## Приоритет операторов

| Приоритет (по убыванию) | Операторы              | Ассоциативность |
|-------------------------|------------------------|-----------------|
| 9                       | "||" (логическое ИЛИ)  | Левая           |
| 8                       | @ (логическое И)       | Левая           |
| 7                       | ==, !=, <, >, <=, >=   | Левая           |
| 6                       | бинарные + -           | Левая           |
| 5                       | *, /, %                | Левая           |
| 4                       | ^ (степень)            | Правая          |	
| 3                       | Унарные +, -, !        | Правая          |
| 2                       | Вызов функции          | Левая           |
| 1                       | ( ) (группировка)      | -               |

## Встроенные функции

| Функция         | Описание                             |
|-----------------|--------------------------------------|
| abs(x)          | Возвращает модуль числа              |
| min(x, y, ...)  | Возвращает наименьшее из чисел       |
| max(x, y, ...)  | Возвращает наибольшее из чисел       |
| round(x)        | Округляет до ближайшего целого       |
| ceil(x)         | Ближайшее целое, большее или равное  |
| floor(x)        | Ближайшее целое, меньшее или равное  |
| power(x, y)     | возводит число x в степень y         |

## Выбранные дополнения
### Встроенные функции для чисел:
	- abs(x) — возвращает модуль числа
	- min(x, y, ...) — возвращает наименьшее из переданных чисел
	- max(x, y, ...) — возвращает наибольшее из переданных чисел

### Встроенные функции для строк
    - len(str) - возвращает длину строки в числовом виде
    - decompose(str, ",")  - разбивает строку по заданному разделителю и возвращает массив строк
    - getsymbol(str, 5) - возвращает символ под индексом 5 в изначальной строке

### Операция возведения в степень
	- ^ (степень)	
	- power(x, y)

### Операции округления
	- round(x) - Округляет до ближайшего целого	
	- ceil(x) - Ближайшее целое, большее или равное
	- floor(x) - Ближайшее целое, меньшее или равное

## Массивы

- Тип массива: `arr of <базовый_тип>`, где базовый тип — `number`, `string` или `boolean`.
- Литерал массива: список выражений в квадратных скобках, например `[1, 2, a+b]`, `[]`.
- Доступ к элементу: выражение с индексацией `myArray[index]` (чтение и запись). Индекс — выражение типа `number`.

## Структуры

- Тип структуры задаётся перечислением полей: `struct { поле1 : тип1, поле2 : тип2, ... }`.
- Литерал структуры: инициализация всех полей в фигурных скобках: `{ поле1 : выражение1, поле2 : выражение2 }`.
- Доступ к полю на чтение и запись: `myObject.field` (значение)

# Грамматика языка в нотации EBNF

	```
		(* ==================== *)
		(* Все виды операторов в спеке 01_lexemes.md *)
		(* ==================== *)

		(* ==================== *)
		(* Основные выражения   *)
		(* ==================== *)
			
			expression = logical_or_expression ;

			logical_or_expression = logical_and_expression, { logical_or_operator, logical_and_expression } ;

			logical_and_expression = comparison_expression, { logical_and_operator, comparison_expression } ;

			comparison_expression = additive_expression, [ comparison_operator, additive_expression ] ;

			additive_expression = term_expression, { additive_operator, term_expression } ;

			term_expression = factor_expression, { multiplicative_operator, factor_expression } ;

			factor_expression = [ unary_operator ], exponentiation_expression ;

			exponentiation_expression = postfix_expression, [ "^", exponentiation_expression ] ;

			postfix_expression = primary_expression, { index_access | member_access_dot } ;

			index_access = "[", expression, "]" ;

			member_access_dot = ".", identifier ;

			primary_expression = literal | variable_access | function_call | array_literal | struct_literal | "(", expression, ")" ;

		(* ==================== *)
		(* Доступ к переменным  *)
		(* ==================== *)

			variable_access = identifier ;

		(* ==================== *)
		(* Функции и вызовы     *)
		(* ==================== *)

			function_call = function_name, "(", [ argument_list ], ")" ;

			function_name = builtin_function_name | identifier ;

			builtin_function_name = "abs" 
				  | "min" 
				  | "max" 
				  | "round" 
				  | "ceil" 
				  | "floor" 
				  | "power" ;
                  | "len" ;
                  | "decompose" ;
                  | "getsymbol"

			argument_list = expression, { ",", expression } ;


        (* ====================     *)
        (*  Инициализация программы *)
        (* ====================     *)

            program = "begin", { statement }, "end" ;

        (* ==================== *)
        (* Инструкции          *)
        (* ==================== *)

            statement = variable_declaration
                      | function_declaration    
                      | assignment_statement
                      | for_statement
                      | output_statement
                      | input_statement
                      | break_statement
                      | continue_statement
                      | return_statement
                      | if_statement
                      | while_statement
                      | expression_statement  
                      | block ;

            block = "begin", { statement }, "end" ;

        (* ==================== *)
        (* Объявления переменных *)
        (* ==================== *)

            variable_declaration = "var", identifier, ":", type, [ "=", expression ], ";"
                        | "string", identifier, [ "=", expression ], ";"
                        | "boolean", identifier, [ "=", expression ], ";" ;

            expression_statement = expression, ";" ;

        (* ==================== *)
        (* Объявление функций *)
        (* ==================== *)

            function_declaration = "function", function_name, "(", [ parameter_list ], ")", [ ":", type ], block ;

            parameter_list = parameter, { ",", parameter } ;

            parameter = identifier, ":", type ;

            type = base_type | array_type | struct_type ;

            base_type = "number" | "string" | "boolean" ;

            array_type = "arr", "of", base_type ;

            struct_type = "struct", "{", field_list, "}" ;

            field_list = field_decl, { ",", field_decl } ;

            field_decl = identifier, ":", type ;

            array_literal = "[", [ expression, { ",", expression } ], "]" ;

            struct_literal = "{", field_initializer_list, "}" ;

            field_initializer_list = identifier, ":", expression, { ",", identifier, ":", expression } ;

        (* ==================== *)
        (* Операторы управления *)
        (* ==================== *)

            if_statement = "if", "(", expression, ")", "then", block, [ "else", block ] ;

            for_statement = "for", identifier, "from", expression, "to", expression, "do", statement ;

            while_statement = "while", "(", expression, ")", "do", statement ;

        (* ==================== *)
        (* Ввод-вывод          *)
        (* ==================== *)

            output_statement = "write", "(", argument_list, ")", ";" ;

            input_statement = "read", "(", identifier, ")", ";" ;

        (* ==================== *)
        (* Контроль потоков     *)
        (* ==================== *)

            break_statement = "break", ";" ;

            continue_statement = "continue", ";" ;

            return_statement = "return", [ expression ], ";" ;

        (* ==================== *)
        (* Присваивание         *)
        (* ==================== *)

            assignment_statement = left_hand_side, "=", expression, ";" ;

            left_hand_side = identifier, { index_access | member_access_dot } ;

        (* ==================== *)
        (* Описание литералов, идентификаторов и др. лексем см в 01_lexemes.md  *)
        (* ==================== *)
    ```

namespace CompilerLexer;

public enum TokenType
{
    /// <summary>
    /// Идентификаторы (имена переменных, функций и т.д.)
    /// </summary>
    Identifier,

    /// <summary>
    /// Целочисленные литералы (0, 100, 999)
    /// </summary>
    Integer,

    /// <summary>
    /// Вещественные литералы (3.14, 0.5, 2.0)
    /// </summary>
    Float,

    /// <summary>
    /// Строковые литералы в двойных кавычках
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Оператор сложения.
    /// </summary>
    PlusSign,

    /// <summary>
    /// Оператор вычитания.
    /// </summary>
    MinusSign,

    /// <summary>
    /// Оператор умножения.
    /// </summary>
    MultiplySign,

    /// <summary>
    /// Оператор деления.
    /// </summary>
    DivideSign,

    /// <summary>
    ///  Оператор возведения в степень.
    /// </summary>
    ExponentiationSign,

    /// <summary>
    /// Оператор деления по модулю.
    /// </summary>
    ModuloSign,

    /// <summary>
    /// Оператор сравнения равно (==)
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения не равно (!=)
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Логический оператор ИЛИ (||)
    /// </summary>
    LogicalOr,

    /// <summary>
    /// Логический оператор И (@)
    /// </summary>
    LogicalAnd,

    /// <summary>
    /// Логический оператор НЕ (!)
    /// </summary>
    LogicalNot,

    /// <summary>
    /// Оператор присваивания (=)
    /// </summary>
    Assign,

    /// <summary>
    /// Левая квадратная скобка [
    /// </summary>
    LBracket,

    /// <summary>
    /// Правая квадратная скобка ]
    /// </summary>
    RBracket,

    /// <summary>
    /// Точка с запятой ; (конец инструкции)
    /// </summary>
    Semicolon,

    /// <summary>
    /// Запятая , (разделитель в списках)
    /// </summary>
    Comma,

    /// <summary>
    /// Двоеточие : (разделитель типа)
    /// </summary>
    Colon,

    /// <summary>
    /// Открывающая круглая скобка '('.
    /// </summary>
    LParen,

    /// <summary>
    /// Закрывающая круглая скобка ')'.
    /// </summary>
    RParen,

    /// <summary>
    /// Левая фигурная скобка {
    /// </summary>
    LBrace,

    /// <summary>
    /// Правая фигурная скобка }
    /// </summary>
    RBrace,

    /// <summary>
    /// Комментарии (однострочные ## и многострочные /# ... #/)
    /// </summary>
    Comment,

    /// <summary>
    /// Конец файла.
    /// </summary>
    EndOfFile,

    /// <summary>
    /// Недопустимая лексема.
    /// </summary>
    Error,

    /// <summary>
    /// Ключевое слово begin (начало блока кода)
    /// </summary>
    Begin,

    /// <summary>
    /// Ключевое слово end (конец блока кода)
    /// </summary>
    End,

    /// <summary>
    /// Ключевое слово var (объявление переменной)
    /// </summary>
    Var,

    /// <summary>
    /// Ключевое слово number (числовой тип)
    /// </summary>
    NumberType,

    /// <summary>
    /// Ключевое слово string (строковый тип)
    /// </summary>
    StringType,

    /// <summary>
    /// Ключевое слово boolean (логический тип)
    /// </summary>
    Boolean,

    /// <summary>
    /// Ключевое слово if (начало условного оператора)
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово then (условие выполнено)
    /// </summary>
    Then,

    /// <summary>
    /// Ключевое слово else (иначе)
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово for (начало цикла)
    /// </summary>
    For,

    /// <summary>
    /// Ключевое слово from (начальное значение)
    /// </summary>
    From,

    /// <summary>
    /// Ключевое слово to (конечное значение)
    /// </summary>
    To,

    /// <summary>
    /// Ключевое слово do (начало исполняемого блока)
    /// </summary>
    Do,

    /// <summary>
    /// Ключевое слово while (цикл while)
    /// </summary>
    While,

    /// <summary>
    /// Ключевое слово write (вывод на экран)
    /// </summary>
    Output,

    /// <summary>
    /// Ключевое слово read (ввод данных)
    /// </summary>
    Input,

    /// <summary>
    /// Ключевое слово true (логическое значение истина)
    /// </summary>
    True,

    /// <summary>
    /// Ключевое слово false (логическое значение ложь)
    /// </summary>
    False,

    /// <summary>
    /// Ключевое слово break (выход из цикла)
    /// </summary>
    Break,

    /// <summary>
    /// Ключевое слово continue (продолжить цикл)
    /// </summary>
    Continue,

    /// <summary>
    /// Ключевое слово return (вернуть значение функции)
    /// </summary>
    Return,

    /// <summary>
    /// Ключевое слово function (объявление функции)
    /// </summary>
    Function,

    /// <summary>
    /// Ключевое слово arr (объявление массива)
    /// </summary>
    Array,

    /// <summary>
    /// Ключевое слово of
    /// </summary>
    Of,

    /// <summary>
    /// Ключевое слово struct (объявление структуры)
    /// </summary>
    Structure,

    /// <summary>
    /// Оператор доступа к полю структуры (точка)
    /// </summary>
    Dot,
}
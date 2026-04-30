#language: ru
Функциональность: встроенные функции
    Сценарий: функция abs_f
        Пусть я скомпилировал программу "features/builtin_functions/abs_f_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        abs_f(-5.0) = 5
        abs_f(10.0) = 10
        abs_f(-3.14) = 3.14
        abs_f(0.0) = 0

        """

    Сценарий: функция min_f
        Пусть я скомпилировал программу "features/builtin_functions/min_f_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        min_f(3.0, -5.0) = -5
        min_f(10.0, 5.0) = 5
        min_f(-2.5, -3.5) = -3.5
        min_f(0.0, 0.0) = 0

        """

    Сценарий: функция max_f
        Пусть я скомпилировал программу "features/builtin_functions/max_f_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        max_f(3.0, -5.0) = 3
        max_f(10.0, 5.0) = 10
        max_f(-2.5, -3.5) = -2.5
        max_f(0.0, 0.0) = 0

        """

    Сценарий: функция round
        Пусть я скомпилировал программу "features/builtin_functions/round_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        round(3.1) = 3
        round(3.7) = 4
        round(-2.3) = -2
        round(-2.8) = -3
        round(0.5) = 0

        """

    Сценарий: функция len
        Пусть я скомпилировал программу "features/builtin_functions/len_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        len("Meow") = 4
        len("") = 0
        len("Here's Pascal++") = 15

        """

    Сценарий: функция getsymbol
        Пусть я скомпилировал программу "features/builtin_functions/getsymbol_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        getsymbol("Meow", 0) = M
        getsymbol("Meow", 3) = w
        getsymbol("Hello", 1) = e

        """

    Сценарий: функция tostring_i
        Пусть я скомпилировал программу "features/builtin_functions/tostring_i_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        tostring_i(42) = 42
        tostring_i(0) = 0
        tostring_i(-100) = -100

        """

    Сценарий: функция tostring_f
        Пусть я скомпилировал программу "features/builtin_functions/tostring_f_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        tostring_f(3.14) = 3.14
        tostring_f(0.0) = 0
        tostring_f(-2.5) = -2.5

        """

    Сценарий: функция len c кириллическими символами
        Пусть я скомпилировал программу "features/unicode_characters/len_cyrillic_unicode.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        len("Мяу") = 3
        len("Это Pascal++") = 12

        """

    Сценарий: функция getsymbol с кириллическими символами
        Пусть я скомпилировал программу "features/unicode_characters/getsymbol_cyrillic_unicode.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        getsymbol("Мяу", 0) = М
        getsymbol("Мяу", 2) = у
        getsymbol("Привет", 1) = р

        """

    Сценарий: функция len с китайскими иероглифами
        Пусть я скомпилировал программу "features/unicode_characters/len_unicode_hieroglyphs.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        len("喵") = 1
        len("这是Pascal++") = 10

        """

    Сценарий: функция getsymbol с китайскими иероглифами
        Пусть я скомпилировал программу "features/unicode_characters/getsymbol_unicode_hieroglyphs.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        getsymbol("喵", 0) = 喵
        getsymbol("喵喵喵", 2) = 喵
        getsymbol("这是Pascal++", 1) = 是

        """

    Сценарий: функция len с эмодзи
        Пусть я скомпилировал программу "features/unicode_characters/len_unicode_emoji.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        len("🐈🐈🐈") = 3
        len("💻Pascal++") = 9

        """
        
    Сценарий: функция getsymbol с эмодзи
        Пусть я скомпилировал программу "features/unicode_characters/getsymbol_unicode_emoji.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        getsymbol("🐈🐈🐈", 0) = 🐈
        getsymbol("🐈🐈🐈", 2) = 🐈
        getsymbol("🖖Hello🖖", 0) = 🖖

        """
#language: ru
Функциональность: встроенные функции
    Сценарий: функция abs
        Пусть я скомпилировал программу "features/builtin_functions/abs_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        abs(-5) = 5
        abs(10) = 10
        abs(-3.14) = 3.14
        abs(0) = 0

        """

    Сценарий: функции min и max
        Пусть я скомпилировал программу "features/builtin_functions/min_max_functions.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        min(3, -5, 0) = -5
        max(10, -5, 8, 2) = 10
        min(3.0, -5.0, 0.0) = -5.0
        max(10.0, -5.0, 8.0, 2.0) = 10.0

        """

    Сценарий: функция round
        Пусть я скомпилировал программу "features/builtin_functions/round_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        round(3.14) = 3
        round(3.7) = 4
        round(-2.3) = -2
        round(-2.8) = -3
        round(0.5) = 1

        """

    Сценарий: строковые функции len, getsymbol, tostring
        Пусть я скомпилировал программу "features/builtin_functions/string_functions.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        len('Hello') = 5
        len('') = 0
        len('Pascal++') = 8
        getsymbol('Hello', 0) = H
        getsymbol('Hello', 4) = o
        tostring(42) = 42
        tostring(3.14) = 3.14

        """

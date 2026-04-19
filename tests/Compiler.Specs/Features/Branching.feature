#language: ru
Функциональность: ветвления
    Сценарий: ветвление if...then
        Пусть я скомпилировал программу "features/branching/if_then.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        x is greater than 5
        
        """
        
    Сценарий: ветвление if...then...else
        Пусть я скомпилировал программу "features/branching/if_then_else.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        x is greater than 5
        y is not greater than 5
        
        """

    Сценарий: вложенные if
        Пусть я скомпилировал программу "features/branching/nested_if.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        x > 20 @ x <= 20

        """

    Сценарий: if с логическими операторами
        Пусть я скомпилировал программу "features/branching/if_with_logical_operations.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        x > 5 @ y > 3 is true
        x > 20 || y > 3 is true
        !(x > 20) is true

        """

    Сценарий: функция может вернуть строку, определённую выражением if-then-else
        Пусть я скомпилировал программу "features/branching/if_in_function.pp"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        5 is positive
        -3 is negative
        0 is zero

        """
        
    Сценарий: отсутствие проблемы "висячего" else
        Пусть я скомпилировал программу "features/branching/no_dangling_else_problem.tig"

        Когда я выполняю программу

        Тогда я увижу вывод:
        """
        no
        
        """
```
begin

    function powerInt(a: float, n: integer) : float
    begin
        var result : float = 1;
        var i : integer = 1;

        while (i <= n) do
        begin
            result = result * a;
            i = i + 1;
        end
        return result;
    end

    var base : float;
    var exp : integer;

    write("Введите число: ");
    read(base);
    write("Введите степень(целое число): ");
    read(exp);

    var res : float = powerInt(base, exp);

    write("Результат: ", res, "\n");
end
```

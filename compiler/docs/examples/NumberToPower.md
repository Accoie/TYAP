```
begin

    function powerInt(a: number, n: number) : number
    begin
        var result : number = 1;
        var i : number = 1;

        while (i <= n) do
        begin
            result = result * a;
            i = i + 1;
        end
        return result;
    end

    var base : number;
    var exp : number;

    write("Введите число: ");
    read(base);
    write("Введите степень: ");
    read(exp);

    var res : number = powerInt(base, exp);

    write("Результат: ", res, "\n");
end
```

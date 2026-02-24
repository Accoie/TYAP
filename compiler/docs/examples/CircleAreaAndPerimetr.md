```
begin
    var n : number = 10;
    var a : arr of number = [0,0,0,0,0,0,0,0,0,0];

    for i from 0 to n-1 do
        write("Введите элемент ", i, ": ");
        read(a[i]);
    end

    for i from 0 to n-2 do
        for j from 0 to n-2 do
            if (a[j] > a[j+1]) then
            begin
                var t : number = a[j];
                a[j] = a[j+1];
                a[j+1] = t;
            end
        end
    end

    write("Отсортированный массив: ");
    for k from 0 to n-1 do
        write(a[k], " ");
    end
end
```

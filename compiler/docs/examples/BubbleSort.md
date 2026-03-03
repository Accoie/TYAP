```
begin
<<<<<<< HEAD
    function bubbleSort(a : arr of number)
    begin
        var n : number = 10;
        for i from 0 to n - 2 do
        begin
            for j from 0 to n - 2 do
=======
    var n : number = 10;
    var a : arr[10] of number = [0,0,0,0,0,0,0,0,0,0];

    for i from 0 to n-1 do
        write("Введите элемент ", i, ": ");
        read(a[i]);
    end

    for i from 0 to n-2 do
        for j from 0 to n-2 do
            if (a[j] > a[j+1]) then
>>>>>>> 4c77b0345bb33d2d1ce0304bde649745ad1c5ea9
            begin
                if (a[j] > a[j + 1]) then
                begin
                    var t : number = a[j];
                    a[j] = a[j + 1];
                    a[j + 1] = t;
                end
            end
        end
    end

    var n : number = 10;
    var a : arr[10] of number = [0,0,0,0,0,0,0,0,0,0];

    for i from 0 to n - 1 do
    begin
        write("Введите элемент ", i, ": ");
        read(arr[i]);
    end

    bubbleSort(arr);

    write("Отсортированный массив:");
    for k from 0 to n - 1 do
    begin
        write(arr[k]);
    end
end
```

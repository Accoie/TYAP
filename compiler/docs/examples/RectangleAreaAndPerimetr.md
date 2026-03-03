```
begin
    var w : number;
    var h : number;

    write("Введите ширину: ");
    read(w);
    write("Введите высоту: ");
    read(h);

    var area : number = w * h;
    var per : number = 2 * (w + h);

    string info = "Площадь: " + tostring(area) + "\nПериметр: " + tostring(per) + "\n";

    write(info);
end
```

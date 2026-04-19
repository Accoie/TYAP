```
begin
    var w : float;
    var h : float;

    write("Введите ширину: ");
    read(w);
    write("Введите высоту: ");
    read(h);

    var area : float = w * h;
    var per : float = 2 * (w + h);
    string info = "Площадь: " + tostring_f(area) + "\nПериметр: " + tostring_f(per) + "\n";

    write(info);
end
```

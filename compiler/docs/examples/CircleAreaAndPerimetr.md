```
begin
    var r : number;
    write("Введите радиус: ");
    read(r);

    var pi : number = 3.1415;

    var l : number = 2 * pi * r;
    var s : number = pi * power(r, 2);

    string mode;
    write("Способ округления (none / ceil / floor): ");
    read(mode);

    if (mode == "ceil") then
    begin
        l = ceil(l);
        s = ceil(s);
    end
    else
    begin
        if (mode == "floor") then
        begin
            l = floor(l);
            s = floor(s);
        end
        else
        begin
            l = l;
            s = s;
        end
    end

    write("Длина окружности: ", l, "\n");
    write("Площадь круга: ", s, "\n");
end

```

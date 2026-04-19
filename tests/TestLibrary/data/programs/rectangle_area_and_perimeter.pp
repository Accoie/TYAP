begin
    var w : float;
    var h : float;

    write("Введите ширину: ");
    read(w);
    write("Введите высоту: ");
    read(h);

    var area : float = w * h;
    var per : float = 2.0 * (w + h);
    var info : string = "Площадь: " + tostring_f(area) + "\nПериметр: " + tostring_f(per);

    write(info);
end
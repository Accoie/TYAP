begin
    var w : float;
    var h : float;

    write("Введите ширину: ");
    read(w);
    write("Введите высоту: ");
    read(h);

    var area : float = w * h;
    var per : float = 2.0 * (w + h);
    
    write("Площадь: ", area);
    write("Периметр: ", per);
end
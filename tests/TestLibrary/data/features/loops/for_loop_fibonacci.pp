begin
    var a: integer = 0;
    var b: integer = 1;
    write(tostring_i(a));
    for i from 1 to 11 do
    begin
        var temp: integer = a + b;
        write(tostring_i(b));
        a = b;
        b = temp;
    end
end

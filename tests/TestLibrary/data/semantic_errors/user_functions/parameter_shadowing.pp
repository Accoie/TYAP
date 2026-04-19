begin
    function testShadow(x: integer)
    begin
        var x: integer = 100;
        write("Inside function: x = ", x);
    end

    var a: integer = 5;
    testShadow(a);
end

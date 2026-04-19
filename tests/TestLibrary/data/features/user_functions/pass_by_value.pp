function testPassByValue(x: integer)
begin
    x = 100;
    write("Inside function: x = ", x);
end

begin
    var a : integer = 5;
    write("Before call: a = ", a);
    testPassByValue(a);
    write("After call: a = ", a);
end

begin
    function square(x: integer) : integer
    begin
        return x * x;
    end
    
    function add(a: integer, b: integer) : integer
    begin
        return a + b;
    end

    write("square(3) + square(4) = ", add(square(3), square(4)));
end

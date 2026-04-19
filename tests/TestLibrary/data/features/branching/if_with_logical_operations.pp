begin
    var x : integer = 10;
    var y : integer = 5;
    
    if (x > 5 @ y > 3) then
    begin
        write("x > 5 @ y > 3 is true");
    end
    else
    begin
        write("x > 5 @ y > 3 is not true");
    end
    
    if (x > 20 || y > 3) then
    begin
        write("x > 20 || y > 3 is true");
    end
    else
    begin
        write("x > 20 || y > 3 is not true");
    end
    
    if (!(x > 20)) then
    begin
        write("!(x > 20) is true");
    end
end

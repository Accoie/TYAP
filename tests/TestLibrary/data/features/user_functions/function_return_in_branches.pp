function absolute(x: integer) : integer
begin
    if (x < 0) then
    begin
        return -x;
    end
    else
    begin
        return x;
    end
end

begin
    write("absolute(-5) = ", absolute(-5));
    write("absolute(10) = ", absolute(10));
end

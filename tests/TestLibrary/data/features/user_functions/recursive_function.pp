function factorial(n: integer) : integer
begin
    if (n == 0) then
    begin
        return 1;
    end
    else
    begin
        return n * factorial(n - 1);
    end
end

begin
    write("factorial(7) = ", factorial(7));
end

begin
    function isEven(n: integer) : integer
    begin
        if (n == 0) then
        begin
            return 1;
        end
        else
        begin
            return isOdd(n - 1);
        end
    end

    function isOdd(n: integer) : integer
    begin
        if (n == 0) then
        begin
            return 0;
        end
        else
        begin
            return isEven(n - 1);
        end
    end

    write(isEven(4));
end

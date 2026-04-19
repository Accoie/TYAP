begin
    function classifyNumber(n: integer) : string
    begin
        if (n > 0) then
        begin
            return "positive";
        end
        else
        begin
            if (n < 0) then
            begin
                return "negative";
            end
            else
            begin
                return "zero";
            end
        end
    end
    write("5 is ", classifyNumber(5));
    write("-3 is ", classifyNumber(-3));
    write("0 is ", classifyNumber(0));
end

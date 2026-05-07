begin
    var i: integer = 0;
    while (i < 5) do
    begin
        i = i + 1;
        if (i == 3) then
        begin
            continue;
        end
        write(tostring_i(i));
    end
end
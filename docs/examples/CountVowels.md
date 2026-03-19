```
begin

    var text : string;
    write("Введите строку: ");
    read(text);

    var n : integer = len(text);
    var vowels : integer = 0;

    for i from 0 to n - 1 do
    begin
        var c : string = getsymbol(text, i);

        if (c == "a") @ (c == "e") @ (c == "i") @ (c == "o") @ (c == "u") @ (c == "A") @ (c == "E") @ (c == "I") @ (c == "O") @ (c == "U") then
        begin
            vowels = vowels + 1;
        end
    end

    write("Количество гласных: ", vowels, "/n");
end
```

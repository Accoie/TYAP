begin
    var text : string;
    write("Введите строку: ");
    read(text);

    var n : integer = len(text);
    var vowels : integer = 0;

    for i from 0 to n - 1 do
    begin
        var c : string = getsymbol(text, i);
        
        var isVowel : integer = (c == "a") || (c == "e") || (c == "i") ||
                                 (c == "o") ||
                                 (c == "u") ||
                                 (c == "A") ||
                                 (c == "E") ||
                                 (c == "I") ||
                                 (c == "O") ||
                                 (c == "U");
        if (isVowel) then
        begin
            vowels = vowels + 1;
        end
    end

    write("Количество гласных: ", tostring_i(vowels));
end
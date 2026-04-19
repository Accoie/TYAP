begin
    function concat_with_space(s1: string, s2: string): string 
    begin
        return s1 + " " + s2;
    end
    
    write(concat_with_space("Hello,", "World!"));
end
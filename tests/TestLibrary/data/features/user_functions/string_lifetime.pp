begin
    function concat_with_space(s1: string, s2: string): string = (
        return s1 + " " + s2;
    )
    
    write(concat_with_space("Hello,", "World!"))
end
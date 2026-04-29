begin
    function testFunc(x: integer)
    begin
        write("ds");
    end
    
    function testFunc(x: integer) : integer
    begin
        write("ds");
        return 32;
    end
    
    testFunc(5);
end

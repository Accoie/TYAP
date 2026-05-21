begin
    function setElement(a : arr[4] of integer, index : integer, value : integer)
    begin
        a[index] = value;
    end

    begin
        var nums : arr[4] of integer;

        setElement(nums, 0, 8);
        setElement(nums, 1, 5);
        setElement(nums, 2, 2);
        setElement(nums, 3, 2);

        for i from 0 to 3 do
        begin
            write(nums[i]);
        end
    end
end
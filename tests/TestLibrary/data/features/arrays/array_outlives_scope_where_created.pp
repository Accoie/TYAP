begin
    var nums : arr[3] of integer;

    begin
        var inner : arr[3] of integer;
        inner = [2, 19, 0];
        nums = inner;
    end

    for i from 0 to 2 do
    begin
        write(nums[i]);
    end
end

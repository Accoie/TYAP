begin
    var nums : arr[3] of integer;
    var numsCopy : arr[3] of integer;

    nums[0] = 7;
    nums[1] = 7;
    nums[2] = 7;

    numsCopy = nums;
    numsCopy[0] = 3;
    numsCopy[1] = 4;

    for i from 0 to 2 do
    begin
        write(nums[i]);
    end
end

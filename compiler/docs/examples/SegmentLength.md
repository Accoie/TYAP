```
begin
    var Point : struct { x : number, y : number };
    var A : Point = { x : 0, y : 0 };
    var B : Point = { x : 0, y : 0 };

    write("Введите координаты точки A:\n");
    write("A.x = ");
    read(A.x);
    write("A.y = ");
    read(A.y);

    write("Введите координаты точки B:\n");
    write("B.x = ");
    read(B.x);
    write("B.y = ");
    read(B.y);

    var dx : number = B.x - A.x;
    var dy : number = B.y - A.y;
    var distance : number = power(dx ^ 2 + dy ^ 2, 0.5);

    write("Расстояние между точками A и B: ", distance);
end
```

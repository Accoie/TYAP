using Compiler;

Console.WriteLine(3.14159d);

return CompilerApplication.Run(
    args: args,
    stdoutWriter: Console.Out,
    stderrWriter: Console.Error
);
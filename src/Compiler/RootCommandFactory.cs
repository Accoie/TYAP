using System.CommandLine;

namespace Compiler;

/// <summary>
/// Фабрика для создания интерфейса командной строки компилятора.
/// Построена на базе библиотеки "System.CommandLine" от Microsoft.
/// </summary>
public static class RootCommandFactory
{
    public static RootCommand Create()
    {
        RootCommand command = new(
            "Сompiling a Pascal++ program into an executable file .NET"
        );

        Argument<string> inputPathArgument = new("input")
        {
            Description = "Path to Pascal++ program source file",
            Arity = ArgumentArity.ExactlyOne,
        };
        command.Add(inputPathArgument);

        Argument<string> outputPathArgument = new("output")
        {
            Description = "Output path for generated executable file .NET",
            Arity = ArgumentArity.ExactlyOne,
        };
        command.Add(outputPathArgument);

        command.SetAction((result) =>
        {
            string inputPath = result.GetRequiredValue(inputPathArgument);
            string outputPath = result.GetRequiredValue(outputPathArgument);

            return Compile(inputPath, outputPath);
        });

        return command;
    }

    private static int Compile(string inputPath, string outputPath)
    {
        CompilerDriver driver = new();
        driver.Compile(inputPath, outputPath);

        return 0;
    }
}
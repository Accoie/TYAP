using Ast.Statements;
using CompilerParser;
using MsilBackend;
using MsilCodegen;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Compiler;

public class CompilerDriver
{
    public void Compile( string inputPath, string outputPath )
    {
        string code = File.ReadAllText( inputPath );

        // Фронтенд компилятора:
        //  1. Лексический анализ
        //  2. Синтаксический анализ
        //  3. Семантический анализ
        Parser parser = new( code );
        BlockStatement program = parser.ParseProgram();

        // Бэкенд компилятора:
        //  1. Генерация MSIL-кода.
        //  2. Сохранение исполняемого файла.
        ExecutableBuilder executableBuilder = new( outputPath );
        MsilCodegenPass codegenPass = new( executableBuilder.ModuleBuilder );
        MethodBuilder mainMethod = codegenPass.GenerateProgramCode( program );
        executableBuilder.Save( mainMethod );
    }
}
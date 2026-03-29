using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace MsilBackend;

/// <summary>
/// Собирает исполняемый модуль для платформы .NET и сохраняет его в файл.
/// </summary>
public class ExecutableBuilder
{
    private readonly string _executablePath;
    private readonly string _runtimeConfigPath;
    private readonly PersistedAssemblyBuilder _assemblyBuilder;
    private readonly ModuleBuilder _moduleBuilder;

    public ExecutableBuilder(string executablePath)
    {
        _executablePath = executablePath;
        _runtimeConfigPath = Path.ChangeExtension(executablePath, "runtimeconfig.json");
        AssemblyName assemblyName = new(Path.GetFileNameWithoutExtension(executablePath));

        _assemblyBuilder = new PersistedAssemblyBuilder(
            assemblyName,
            coreAssembly: typeof(object).Assembly,
            assemblyAttributes:
            null
        );

        _moduleBuilder = _assemblyBuilder.DefineDynamicModule(Path.GetFileName(executablePath));
    }

    public ModuleBuilder ModuleBuilder => _moduleBuilder;

    public void Save(MethodBuilder mainMethod)
    {
        MetadataBuilder metadataBuilder = _assemblyBuilder.GenerateMetadata(
            out BlobBuilder ilStream,
            out BlobBuilder fieldData
        );

        MethodDefinitionHandle mainHandle = GetMethodDefinitionHandle(mainMethod);

        ManagedPEBuilder peBuilder = new(
            header: PEHeaderBuilder.CreateExecutableHeader(),
            metadataRootBuilder: new MetadataRootBuilder(metadataBuilder),
            ilStream: ilStream,
            mappedFieldData: fieldData,
            entryPoint: mainHandle
        );

        CreateExecutableFile(_executablePath, peBuilder);
        SetExecutePermissions(_executablePath);

        RuntimeConfigGenerator.SaveRuntimeConfig(_runtimeConfigPath);
    }

    private static void CreateExecutableFile(
        string executablePath,
        ManagedPEBuilder peBuilder
    )
    {
        BlobBuilder peBlob = new();
        peBuilder.Serialize(peBlob);

        using FileStream fileStream = new(executablePath, FileMode.Create, FileAccess.Write);
        peBlob.WriteContentTo(fileStream);
    }

    /// <summary>
    /// Получает объект, необходимый для установки метода Main() как точки входа исполняемого файла.
    /// </summary>
    private static MethodDefinitionHandle GetMethodDefinitionHandle(MethodBuilder mainMethod)
    {
        // Получаем токен метода Main().
        int token = mainMethod.MetadataToken;
        int rowNumber = token & 0x00FFFFFF;
        MethodDefinitionHandle mainHandle = MetadataTokens.MethodDefinitionHandle(rowNumber);
        return mainHandle;
    }

    /// <summary>
    /// Устанавливает UNIX-права на выполнение программы.
    /// </summary>
    private static void SetExecutePermissions(string executablePath)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            File.SetUnixFileMode(
                executablePath,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                UnixFileMode.OtherRead | UnixFileMode.OtherExecute
            );
        }
    }
}
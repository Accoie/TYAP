namespace CompilerLexer;

public class FileScanner : IScanner
{
    private readonly char[] _fileContent;
    private int _position;

    public FileScanner(string path)
    {
        _fileContent = File.ReadAllText(path).ToCharArray();
        _position = 0;
    }

    public char Peek(int n = 0)
    {
        int peekPosition = _position + n;

        return peekPosition < 0 || peekPosition >= _fileContent.Length ? '\0' : _fileContent[peekPosition];
    }

    public char Read()
    {
        return _position >= _fileContent.Length ? '\0' : _fileContent[_position++];
    }

    public void Advance()
    {
        _position = Math.Min(_position + 1, _fileContent.Length);
    }

    public int GetPosition()
    {
        return _position;
    }

    public bool IsEnd()
    {
        return _position >= _fileContent.Length;
    }
}
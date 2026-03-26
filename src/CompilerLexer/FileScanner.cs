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

        if (peekPosition < 0 || peekPosition >= _fileContent.Length)
        {
            return '\0';
        }

        return _fileContent[peekPosition];
    }

    public char Read()
    {
        if (_position >= _fileContent.Length)
        {
            return '\0';
        }

        return _fileContent[_position++];
    }

    public void Advance()
    {
        _position = Math.Min(_position + 1, _fileContent.Length);
    }

    public int GetPosition() => _position;

    public bool IsEnd() => _position >= _fileContent.Length;
}
namespace CompilerLexer;

public interface IScanner
{
    void Advance();

    bool IsEnd();

    char Peek(int n = 0);
}
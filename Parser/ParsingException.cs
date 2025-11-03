namespace Snip.Parser;

public class ParsingError(string message, int line = -1, int column = -1) : Exception(message)
{
    public int Line { get; } = line;
    public int Column { get; } = column;

    public override string Message => Line >= 0 && Column >= 0
        ? $"{base.Message} (line {Line}, column {Column})"
        : base.Message;
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Snip.Lexer;

public enum TokenType
{
    Number,
    String,
    Boolean,
    Null,
    Undefined,
    Identifier,

    Plus, 
    Minus, 
    Multiply,
    Divide, 
    Modulo,
    Power, 
    Assign,
    PlusAssign,
    MinusAssign,
    MultiplyAssign,
    DivideAssign,
    ModuloAssign,

    Equal,
    NotEqual,
    StrictEqual,
    StrictNotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan, 
    GreaterThanOrEqual, 

    And, // &&
    Or, // ||
    Not, // !

    // Bitwise
    BitwiseAnd, // &
    BitwiseOr, // |
    BitwiseXor, // ^
    BitwiseNot, // ~
    LeftShift, // <<
    RightShift, // >>
    UnsignedRightShift, // >>>

    Increment, // ++
    Decrement, // --

    Semicolon, // ;
    Comma, // ,
    Dot, // .
    Colon, // :
    QuestionMark, // ?
    Arrow, // =>

    LeftParen, // (
    RightParen, // )
    LeftBrace, // {
    RightBrace, // }
    LeftBracket, // [
    RightBracket, // ]

    Let,
    Const,
    Var,
    Function,
    Return,
    If,
    Else,
    For,
    While,
    Do,
    Switch,
    Case,
    Default,
    Break,
    Continue,
    Try,
    Catch,
    Finally,
    Throw,
    New,
    This,
    Super,
    Class,
    Interface,
    Extends,
    Implements,
    Import,
    Export,
    From,
    As,
    Type,
    Enum,
    Public,
    Private,
    Protected,
    Static,
    Readonly,
    Abstract,
    Async,
    Await,
    Typeof,
    Instanceof,
    In,
    Of,

    String_Type, // string
    Number_Type, // number
    Boolean_Type, // boolean
    Any_Type, // any
    Void_Type, // void
    Never_Type, // never
    Unknown_Type, // unknown
    Object_Type, // object

    Newline,
    Whitespace,
    Comment,
    EndOfFile,
    Invalid
}

public class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }
    private int Line { get; set; }
    private int Column { get; set; }
    private int Position { get; set; }

    public Token(TokenType type, string value, int line, int column, int position)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
        Position = position;
    }

    public override string ToString()
    {
        string v = Value == "\n" ? "\\n" : Value;
        return $"({Type}, '{v}', Line: {Line}, Col: {Column})";
    }
}

public class Lexer
{
    private readonly string _source;
    private int _position;
    private int _line;
    private int _column;
    private readonly List<Token> _tokens;

    public override string ToString()
    {
        return _tokens.Aggregate(string.Empty, (current, t) => $"{current}\n{t.ToString()}");
    }

    private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        { "let", TokenType.Let },
        { "const", TokenType.Const },
        { "var", TokenType.Var },
        { "function", TokenType.Function },
        { "return", TokenType.Return },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "for", TokenType.For },
        { "while", TokenType.While },
        { "do", TokenType.Do },
        { "switch", TokenType.Switch },
        { "case", TokenType.Case },
        { "default", TokenType.Default },
        { "break", TokenType.Break },
        { "continue", TokenType.Continue },
        { "try", TokenType.Try },
        { "catch", TokenType.Catch },
        { "finally", TokenType.Finally },
        { "throw", TokenType.Throw },
        { "new", TokenType.New },
        { "this", TokenType.This },
        { "super", TokenType.Super },
        { "class", TokenType.Class },
        { "interface", TokenType.Interface },
        { "extends", TokenType.Extends },
        { "implements", TokenType.Implements },
        { "import", TokenType.Import },
        { "export", TokenType.Export },
        { "from", TokenType.From },
        { "as", TokenType.As },
        { "type", TokenType.Type },
        { "enum", TokenType.Enum },
        { "public", TokenType.Public },
        { "private", TokenType.Private },
        { "protected", TokenType.Protected },
        { "static", TokenType.Static },
        { "readonly", TokenType.Readonly },
        { "abstract", TokenType.Abstract },
        { "async", TokenType.Async },
        { "await", TokenType.Await },
        { "typeof", TokenType.Typeof },
        { "instanceof", TokenType.Instanceof },
        { "in", TokenType.In },
        { "of", TokenType.Of },
        { "true", TokenType.Boolean },
        { "false", TokenType.Boolean },
        { "null", TokenType.Null },
        { "undefined", TokenType.Undefined },
        { "string", TokenType.String_Type },
        { "number", TokenType.Number_Type },
        { "boolean", TokenType.Boolean_Type },
        { "any", TokenType.Any_Type },
        { "void", TokenType.Void_Type },
        { "never", TokenType.Never_Type },
        { "unknown", TokenType.Unknown_Type },
        { "object", TokenType.Object_Type }
    };
    

    public Lexer(string source)
    {
        _source = source;
        _position = 0;
        _line = 1;
        _column = 1;
        _tokens = new List<Token>();
    }

    public List<Token> Tokenize()
    {
        while (!IsAtEnd())
        {
            ScanToken();
        }

        AddToken(TokenType.EndOfFile, "");
        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case ' ':
            case '\r':
            case '\t':
                break;
            case '\n':
                AddToken(TokenType.Newline, "\n");
                _line++;
                _column = 1;
                break;

            case '(':
                AddToken(TokenType.LeftParen, "(");
                break;
            case ')':
                AddToken(TokenType.RightParen, ")");
                break;
            case '{':
                AddToken(TokenType.LeftBrace, "{");
                break;
            case '}':
                AddToken(TokenType.RightBrace, "}");
                break;
            case '[':
                AddToken(TokenType.LeftBracket, "[");
                break;
            case ']':
                AddToken(TokenType.RightBracket, "]");
                break;
            case ',':
                AddToken(TokenType.Comma, ",");
                break;
            case '.':
                AddToken(TokenType.Dot, ".");
                break;
            case ';':
                AddToken(TokenType.Semicolon, ";");
                break;
            case ':':
                AddToken(TokenType.Colon, ":");
                break;
            case '?':
                AddToken(TokenType.QuestionMark, "?");
                break;
            case '~':
                AddToken(TokenType.BitwiseNot, "~");
                break;

            case '+':
                if (Match('+'))
                    AddToken(TokenType.Increment, "++");
                else if (Match('='))
                    AddToken(TokenType.PlusAssign, "+=");
                else
                    AddToken(TokenType.Plus, "+");
                break;
            case '-':
                if (Match('-'))
                    AddToken(TokenType.Decrement, "--");
                else if (Match('='))
                    AddToken(TokenType.MinusAssign, "-=");
                else
                    AddToken(TokenType.Minus, "-");
                break;
            case '*':
                if (Match('*'))
                    AddToken(TokenType.Power, "**");
                else if (Match('='))
                    AddToken(TokenType.MultiplyAssign, "*=");
                else
                    AddToken(TokenType.Multiply, "*");
                break;
            case '/':
                if (Match('/'))
                {
                    ScanSingleLineComment();
                }
                else if (Match('*'))
                {
                    ScanMultiLineComment();
                }
                else if (Match('='))
                {
                    AddToken(TokenType.DivideAssign, "/=");
                }
                else
                {
                    AddToken(TokenType.Divide, "/");
                }

                break;
            case '%':
                if (Match('='))
                    AddToken(TokenType.ModuloAssign, "%=");
                else
                    AddToken(TokenType.Modulo, "%");
                break;
            case '=':
                if (Match('='))
                {
                    if (Match('='))
                        AddToken(TokenType.StrictEqual, "===");
                    else
                        AddToken(TokenType.Equal, "==");
                }
                else if (Match('>'))
                {
                    AddToken(TokenType.Arrow, "=>");
                }
                else
                {
                    AddToken(TokenType.Assign, "=");
                }

                break;
            case '!':
                if (Match('='))
                {
                    if (Match('='))
                        AddToken(TokenType.StrictNotEqual, "!==");
                    else
                        AddToken(TokenType.NotEqual, "!=");
                }
                else
                {
                    AddToken(TokenType.Not, "!");
                }

                break;
            case '<':
                if (Match('<'))
                    AddToken(TokenType.LeftShift, "<<");
                else if (Match('='))
                    AddToken(TokenType.LessThanOrEqual, "<=");
                else
                    AddToken(TokenType.LessThan, "<");
                break;
            case '>':
                if (Match('>'))
                {
                    if (Match('>'))
                        AddToken(TokenType.UnsignedRightShift, ">>>");
                    else
                        AddToken(TokenType.RightShift, ">>");
                }
                else if (Match('='))
                {
                    AddToken(TokenType.GreaterThanOrEqual, ">=");
                }
                else
                {
                    AddToken(TokenType.GreaterThan, ">");
                }

                break;
            case '&':
                if (Match('&'))
                    AddToken(TokenType.And, "&&");
                else
                    AddToken(TokenType.BitwiseAnd, "&");
                break;
            case '|':
                if (Match('|'))
                    AddToken(TokenType.Or, "||");
                else
                    AddToken(TokenType.BitwiseOr, "|");
                break;
            case '^':
                AddToken(TokenType.BitwiseXor, "^");
                break;

            case '"':
                ScanString('"');
                break;
            case '\'':
                ScanString('\'');
                break;
            case '`':
                ScanTemplateString();
                break;

            default:
                if (IsDigit(c))
                {
                    ScanNumber();
                }
                else if (IsAlpha(c))
                {
                    ScanIdentifier();
                }
                else
                {
                    AddToken(TokenType.Invalid, c.ToString());
                }

                break;
        }
    }

    private void ScanString(char quote)
    {
        var value = new StringBuilder();

        while (Peek() != quote && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
                _column = 1;
            }

            if (Peek() == '\\')
            {
                Advance(); // consume backslash
                if (!IsAtEnd())
                {
                    char escaped = Advance();
                    switch (escaped)
                    {
                        case 'n': value.Append('\n'); break;
                        case 't': value.Append('\t'); break;
                        case 'r': value.Append('\r'); break;
                        case '\\': value.Append('\\'); break;
                        case '\'': value.Append('\''); break;
                        case '"': value.Append('"'); break;
                        default: value.Append(escaped); break;
                    }
                }
            }
            else
            {
                value.Append(Advance());
            }
        }

        if (IsAtEnd())
        {
            AddToken(TokenType.Invalid, "Unterminated string");
            return;
        }

        // Consume closing quote
        Advance();
        AddToken(TokenType.String, value.ToString());
    }

    private void ScanTemplateString()
    {
        var value = new StringBuilder();

        while (Peek() != '`' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
                _column = 1;
            }

            value.Append(Advance());
        }

        if (IsAtEnd())
        {
            AddToken(TokenType.Invalid, "Unterminated template string");
            return;
        }

        Advance();
        AddToken(TokenType.String, value.ToString());
    }

    private void ScanNumber()
    {
        var start = _position - 1;

        // Consume digits
        while (IsDigit(Peek()))
            Advance();

        // Look for decimal part
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the '.'
            Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        // Look for scientific notation
        if (Peek() == 'e' || Peek() == 'E')
        {
            Advance();
            if (Peek() == '+' || Peek() == '-')
                Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        var value = _source.Substring(start, _position - start);
        AddToken(TokenType.Number, value);
    }

    private void ScanIdentifier()
    {
        var start = _position - 1;

        while (IsAlphaNumeric(Peek()))
            Advance();

        var text = _source.Substring(start, _position - start);
        var type = Keywords.ContainsKey(text) ? Keywords[text] : TokenType.Identifier;
        AddToken(type, text);
    }

    private void ScanSingleLineComment()
    {
        var start = _position - 2; 

        while (Peek() != '\n' && !IsAtEnd())
            Advance();

        var comment = _source.Substring(start, _position - start);
        AddToken(TokenType.Comment, comment);
    }

    private void ScanMultiLineComment()
    {
        var start = _position - 2; // Include the /*

        while (!IsAtEnd())
        {
            if (Peek() == '*' && PeekNext() == '/')
            {
                Advance(); // consume *
                Advance(); // consume /
                break;
            }

            if (Peek() == '\n')
            {
                _line++;
                _column = 1;
            }

            Advance();
        }

        var comment = _source.Substring(start, _position - start);
        AddToken(TokenType.Comment, comment);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_position] != expected) return false;

        _position++;
        _column++;
        return true;
    }

    private char Advance()
    {
        _column++;
        return _source[_position++];
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return _source[_position];
    }

    private char PeekNext()
    {
        if (_position + 1 >= _source.Length) return '\0';
        return _source[_position + 1];
    }

    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_' || c == '$';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private void AddToken(TokenType type, string value)
    {
        _tokens.Add(new Token(type, value, _line, _column - value.Length, _position - value.Length));
    }
}
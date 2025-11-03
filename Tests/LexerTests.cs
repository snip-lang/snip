using Xunit;
using Snip.Lexer;

namespace Tests;

public class LexerTests
{
    [Fact]
    public void TokenizeNumbers_ShouldHandleIntegers()
    {
        // Arrange
        var input = "42 0 123";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize().Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile).ToList();

        // Assert
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
        Assert.Equal(TokenType.Number, tokens[1].Type);
        Assert.Equal("0", tokens[1].Value);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("123", tokens[2].Value);
    }

    [Fact]
    public void TokenizeNumbers_ShouldHandleFloats()
    {
        // Arrange
        var input = "3.14 0.5 .25";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(4, tokens.Count); // 3 numbers + EOF
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("3.14", tokens[0].Value);
        Assert.Equal(TokenType.Number, tokens[1].Type);
        Assert.Equal("0.5", tokens[1].Value);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(".25", tokens[2].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[3].Type);
    }

    [Fact]
    public void TokenizeNumbers_ShouldHandleScientificNotation()
    {
        // Arrange
        var input = "1e10 2.5E-3 6.02e+23";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(4, tokens.Count); // 3 numbers + EOF
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("1e10", tokens[0].Value);
        Assert.Equal(TokenType.Number, tokens[1].Type);
        Assert.Equal("2.5E-3", tokens[1].Value);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("6.02e+23", tokens[2].Value);
    }

    [Fact]
    public void TokenizeStrings_ShouldHandleDoubleQuotes()
    {
        // Arrange
        var input = "\"hello\" \"world\"";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(3, tokens.Count); // 2 strings + EOF
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("hello", tokens[0].Value);
        Assert.Equal(TokenType.String, tokens[1].Type);
        Assert.Equal("world", tokens[1].Value);
    }

    [Fact]
    public void TokenizeStrings_ShouldHandleSingleQuotes()
    {
        // Arrange
        var input = "'hello' 'world'";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(3, tokens.Count); // 2 strings + EOF
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("hello", tokens[0].Value);
        Assert.Equal(TokenType.String, tokens[1].Type);
        Assert.Equal("world", tokens[1].Value);
    }

    [Fact]
    public void TokenizeStrings_ShouldHandleEscapeSequences()
    {
        // Arrange
        var input = "\"hello\\nworld\\t\\\"\"";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(2, tokens.Count); // 1 string + EOF
        Assert.Equal(TokenType.String, tokens[0].Type);
        Assert.Equal("hello\nworld\t\"", tokens[0].Value);
    }

    [Fact]
    public void TokenizeStrings_ShouldHandleTemplateStrings()
    {
        // Arrange
        var input = "`hello world`";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(2, tokens.Count); // 1 template + EOF
        Assert.Equal(TokenType.TemplateLiteral, tokens[0].Type);
        Assert.Equal("hello world", tokens[0].Value);
    }

    [Fact]
    public void TokenizeComments_ShouldHandleSingleLineComments()
    {
        // Arrange
        var input = "42 // this is a comment\n 24";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(5, tokens.Count); // number, comment, newline, number, EOF
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
        Assert.Equal(TokenType.Comment, tokens[1].Type);
        Assert.Equal("// this is a comment", tokens[1].Value);
        Assert.Equal(TokenType.Newline, tokens[2].Type);
        Assert.Equal(TokenType.Number, tokens[3].Type);
        Assert.Equal("24", tokens[3].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[4].Type);
    }

    [Fact]
    public void TokenizeComments_ShouldHandleMultiLineComments()
    {
        // Arrange
        var input = "42 /* this is a\nmulti-line comment */ 24";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(4, tokens.Count); // number, comment, number, EOF
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal("42", tokens[0].Value);
        Assert.Equal(TokenType.Comment, tokens[1].Type);
        Assert.Equal("/* this is a\nmulti-line comment */", tokens[1].Value);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal("24", tokens[2].Value);
        Assert.Equal(TokenType.EndOfFile, tokens[3].Type);
    }

    [Fact]
    public void TokenizeKeywords_ShouldRecognizeAllKeywords()
    {
        // Test a few key keywords to ensure they are recognized
        var testCases = new[] {
            ("let", TokenType.Let),
            ("const", TokenType.Const),
            ("function", TokenType.Function),
            ("return", TokenType.Return),
            ("if", TokenType.If),
            ("else", TokenType.Else),
            ("true", TokenType.Boolean),
            ("false", TokenType.Boolean),
            ("null", TokenType.Null),
            ("undefined", TokenType.Undefined)
        };

        foreach (var (keyword, expectedType) in testCases)
        {
            var input = keyword;
            var lexer = new Lexer(input);

            // Act
            var tokens = lexer.Tokenize().Where(t => t.Type != TokenType.EndOfFile).ToList();

            // Assert
            Assert.Single(tokens);
            Assert.Equal(expectedType, tokens[0].Type);
            Assert.Equal(keyword, tokens[0].Value);
        }
    }

    [Fact]
    public void TokenizeOperators_ShouldHandleArithmeticOperators()
    {
        // Arrange
        var input = "+ - * / % **";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(7, tokens.Count); // 6 operators + EOF
        Assert.Equal(TokenType.Plus, tokens[0].Type);
        Assert.Equal(TokenType.Minus, tokens[1].Type);
        Assert.Equal(TokenType.Multiply, tokens[2].Type);
        Assert.Equal(TokenType.Divide, tokens[3].Type);
        Assert.Equal(TokenType.Modulo, tokens[4].Type);
        Assert.Equal(TokenType.Power, tokens[5].Type);
    }

    [Fact]
    public void TokenizeOperators_ShouldHandleComparisonOperators()
    {
        // Arrange
        var input = "== != < <= > >= === !==";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(9, tokens.Count); // 8 operators + EOF
        Assert.Equal(TokenType.Equal, tokens[0].Type);
        Assert.Equal(TokenType.NotEqual, tokens[1].Type);
        Assert.Equal(TokenType.LessThan, tokens[2].Type);
        Assert.Equal(TokenType.LessThanOrEqual, tokens[3].Type);
        Assert.Equal(TokenType.GreaterThan, tokens[4].Type);
        Assert.Equal(TokenType.GreaterThanOrEqual, tokens[5].Type);
        Assert.Equal(TokenType.StrictEqual, tokens[6].Type);
        Assert.Equal(TokenType.StrictNotEqual, tokens[7].Type);
    }

    [Fact]
    public void TokenizeOperators_ShouldHandleLogicalOperators()
    {
        // Arrange
        var input = "&& || !";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(4, tokens.Count); // 3 operators + EOF
        Assert.Equal(TokenType.And, tokens[0].Type);
        Assert.Equal(TokenType.Or, tokens[1].Type);
        Assert.Equal(TokenType.Not, tokens[2].Type);
    }

    [Fact]
    public void TokenizeOperators_ShouldHandleBitwiseOperators()
    {
        // Arrange
        var input = "& | ^ ~ << >> >>>";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(8, tokens.Count); // 7 operators + EOF
        Assert.Equal(TokenType.BitwiseAnd, tokens[0].Type);
        Assert.Equal(TokenType.BitwiseOr, tokens[1].Type);
        Assert.Equal(TokenType.BitwiseXor, tokens[2].Type);
        Assert.Equal(TokenType.BitwiseNot, tokens[3].Type);
        Assert.Equal(TokenType.LeftShift, tokens[4].Type);
        Assert.Equal(TokenType.RightShift, tokens[5].Type);
        Assert.Equal(TokenType.UnsignedRightShift, tokens[6].Type);
    }

    [Fact]
    public void TokenizeOperators_ShouldHandleAssignmentOperators()
    {
        // Arrange
        var input = "= += -= *= /= %= ++ --";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(9, tokens.Count); // 7 operators + EOF
        Assert.Equal(TokenType.Assign, tokens[0].Type);
        Assert.Equal(TokenType.PlusAssign, tokens[1].Type);
        Assert.Equal(TokenType.MinusAssign, tokens[2].Type);
        Assert.Equal(TokenType.MultiplyAssign, tokens[3].Type);
        Assert.Equal(TokenType.DivideAssign, tokens[4].Type);
        Assert.Equal(TokenType.ModuloAssign, tokens[5].Type);
        Assert.Equal(TokenType.Increment, tokens[6].Type);
        Assert.Equal(TokenType.Decrement, tokens[7].Type);
        Assert.Equal(TokenType.EndOfFile, tokens[8].Type);
    }

    [Fact]
    public void TokenizeOperators_ShouldHandleOtherOperators()
    {
        // Arrange
        var input = "? : => . , ; ( ) { } [ ]";
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.Tokenize();

        // Assert
        Assert.Equal(13, tokens.Count); // 12 operators/punctuation + EOF
        Assert.Equal(TokenType.QuestionMark, tokens[0].Type);
        Assert.Equal(TokenType.Colon, tokens[1].Type);
        Assert.Equal(TokenType.Arrow, tokens[2].Type);
        Assert.Equal(TokenType.Dot, tokens[3].Type);
        Assert.Equal(TokenType.Comma, tokens[4].Type);
        Assert.Equal(TokenType.Semicolon, tokens[5].Type);
        Assert.Equal(TokenType.LeftParen, tokens[6].Type);
        Assert.Equal(TokenType.RightParen, tokens[7].Type);
        Assert.Equal(TokenType.LeftBrace, tokens[8].Type);
        Assert.Equal(TokenType.RightBrace, tokens[9].Type);
        Assert.Equal(TokenType.LeftBracket, tokens[10].Type);
        Assert.Equal(TokenType.RightBracket, tokens[11].Type);
        Assert.Equal(TokenType.EndOfFile, tokens[12].Type);
    }
}
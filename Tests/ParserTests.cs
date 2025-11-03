using Xunit;
using Snip.AST;
using Snip.Lexer;
using Snip.Parser;

namespace Tests;

public class ParserTests
{
    [Fact]
    public void ParseUnaryPlus_ShouldParseCorrectly()
    {
        // Arrange
        var input = "+42;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var unaryExpr = Assert.IsType<UnaryExpressionNode>(exprStmt.Expression);
        Assert.Equal("+", unaryExpr.Operator);
        Assert.True(unaryExpr.IsPrefix);
        var operand = Assert.IsType<NumberLiteralNode>(unaryExpr.Argument);
        Assert.Equal(42.0, operand.Value);
    }

    [Fact]
    public void ParseUnaryMinus_ShouldParseCorrectly()
    {
        // Arrange
        var input = "-42;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var unaryExpr = Assert.IsType<UnaryExpressionNode>(exprStmt.Expression);
        Assert.Equal("-", unaryExpr.Operator);
        Assert.True(unaryExpr.IsPrefix);
        var operand = Assert.IsType<NumberLiteralNode>(unaryExpr.Argument);
        Assert.Equal(42.0, operand.Value);
    }

    [Fact]
    public void ParseUnaryMinusWithComplexExpression_ShouldParseCorrectly()
    {
        // Arrange
        var input = "-(3 + 4);";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var unaryExpr = Assert.IsType<UnaryExpressionNode>(exprStmt.Expression);
        Assert.Equal("-", unaryExpr.Operator);
        Assert.True(unaryExpr.IsPrefix);
        var addExpr = Assert.IsType<AddExpressionNode>(unaryExpr.Argument);
        var left = Assert.IsType<NumberLiteralNode>(addExpr.Left);
        var right = Assert.IsType<NumberLiteralNode>(addExpr.Right);
        Assert.Equal(3.0, left.Value);
        Assert.Equal(4.0, right.Value);
    }

    [Fact]
    public void ParsePowerOperator_ShouldParseCorrectly()
    {
        // Arrange
        var input = "2 ** 3;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var powerExpr = Assert.IsType<PowerExpressionNode>(exprStmt.Expression);
        var left = Assert.IsType<NumberLiteralNode>(powerExpr.Left);
        var right = Assert.IsType<NumberLiteralNode>(powerExpr.Right);
        Assert.Equal(2.0, left.Value);
        Assert.Equal(3.0, right.Value);
    }

    [Fact]
    public void ParsePowerOperatorWithPrecedence_ShouldParseCorrectly()
    {
        // Arrange
        var input = "2 * 3 ** 4;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var multiplyExpr = Assert.IsType<MultiplyExpressionNode>(exprStmt.Expression);
        var left = Assert.IsType<NumberLiteralNode>(multiplyExpr.Left);
        var powerExpr = Assert.IsType<PowerExpressionNode>(multiplyExpr.Right);
        Assert.Equal(2.0, left.Value);
        var powerLeft = Assert.IsType<NumberLiteralNode>(powerExpr.Left);
        var powerRight = Assert.IsType<NumberLiteralNode>(powerExpr.Right);
        Assert.Equal(3.0, powerLeft.Value);
        Assert.Equal(4.0, powerRight.Value);
    }

    [Fact]
    public void ParseNullLiteral_ShouldParseCorrectly()
    {
        // Arrange
        var input = "null;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        Assert.IsType<NullLiteralNode>(exprStmt.Expression);
    }

    [Fact]
    public void ParseUndefinedLiteral_ShouldParseCorrectly()
    {
        // Arrange
        var input = "undefined;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        Assert.IsType<UndefinedLiteralNode>(exprStmt.Expression);
    }

    [Fact]
    public void ParseThisExpression_ShouldParseCorrectly()
    {
        // Arrange
        var input = "this;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        Assert.IsType<ThisExpressionNode>(exprStmt.Expression);
    }

    [Fact]
    public void ParseSuperExpression_ShouldParseCorrectly()
    {
        // Arrange
        var input = "super;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        Assert.IsType<SuperExpressionNode>(exprStmt.Expression);
    }

    [Fact]
    public void ParseConditionalExpression_ShouldParseCorrectly()
    {
        // Arrange
        var input = "x > 0 ? x : 0;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var condExpr = Assert.IsType<ConditionalExpressionNode>(exprStmt.Expression);

        var test = Assert.IsType<GreaterThanExpressionNode>(condExpr.Test);
        var consequent = Assert.IsType<IdentifierNode>(condExpr.Consequent);
        var alternative = Assert.IsType<NumberLiteralNode>(condExpr.Alternative);

        Assert.Equal("x", Assert.IsType<IdentifierNode>(test.Left).Name);
        Assert.Equal(0.0, Assert.IsType<NumberLiteralNode>(test.Right).Value);
        Assert.Equal("x", consequent.Name);
        Assert.Equal(0.0, alternative.Value);
    }

    [Fact]
    public void ParseFunctionDeclaration_ShouldParseCorrectly()
    {
        // Arrange
        var input = "function add(a, b) { return a + b; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var funcDecl = Assert.IsType<FunctionDeclarationNode>(program.Statements[0]);
        Assert.Equal("add", funcDecl.Name.Name);
        Assert.Equal(2, funcDecl.Parameters.Count);
        Assert.Equal("a", Assert.IsType<ParameterNode>(funcDecl.Parameters[0]).Name.Name);
        Assert.Equal("b", Assert.IsType<ParameterNode>(funcDecl.Parameters[1]).Name.Name);

        var body = funcDecl.Body;
        Assert.Single(body.Body);
        var returnStmt = Assert.IsType<ReturnStatementNode>(body.Body[0]);
        var addExpr = Assert.IsType<AddExpressionNode>(returnStmt.Value);
        Assert.Equal("a", Assert.IsType<IdentifierNode>(addExpr.Left).Name);
        Assert.Equal("b", Assert.IsType<IdentifierNode>(addExpr.Right).Name);
    }

    [Fact]
    public void ParseArrowFunction_ShouldParseCorrectly()
    {
        // Arrange
        var input = "(a, b) => a + b;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var arrowFunc = Assert.IsType<ArrowFunctionExpressionNode>(exprStmt.Expression);
        Assert.Equal(2, arrowFunc.Parameters.Count);
        Assert.Equal("a", Assert.IsType<ParameterNode>(arrowFunc.Parameters[0]).Name.Name);
        Assert.Equal("b", Assert.IsType<ParameterNode>(arrowFunc.Parameters[1]).Name.Name);

        var body = Assert.IsType<AddExpressionNode>(arrowFunc.Body);
        Assert.Equal("a", Assert.IsType<IdentifierNode>(body.Left).Name);
        Assert.Equal("b", Assert.IsType<IdentifierNode>(body.Right).Name);
    }

    [Fact]
    public void ParseArrowFunctionWithBlockBody_ShouldParseCorrectly()
    {
        // Arrange
        var input = "(a, b) => { return a + b; };";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var arrowFunc = Assert.IsType<ArrowFunctionExpressionNode>(exprStmt.Expression);
        Assert.Equal(2, arrowFunc.Parameters.Count);

        var body = Assert.IsType<BlockStatementNode>(arrowFunc.Body);
        Assert.Single(body.Body);
        var returnStmt = Assert.IsType<ReturnStatementNode>(body.Body[0]);
        var addExpr = Assert.IsType<AddExpressionNode>(returnStmt.Value);
        Assert.Equal("a", Assert.IsType<IdentifierNode>(addExpr.Left).Name);
        Assert.Equal("b", Assert.IsType<IdentifierNode>(addExpr.Right).Name);
    }

    [Fact]
    public void ParseForLoop_ShouldParseCorrectly()
    {
        // Arrange
        var input = "for (let i = 0; i < 10; i++) { console.log(i); }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var forStmt = Assert.IsType<ForStatementNode>(program.Statements[0]);

        var init = Assert.IsType<LetStatementNode>(forStmt.Initializer);
        var identPattern = Assert.IsType<IdentifierPatternNode>(init.Pattern);
        Assert.Equal("i", identPattern.Name);
        Assert.Equal(0.0, Assert.IsType<NumberLiteralNode>(init.Value).Value);

        var condition = Assert.IsType<LessThanExpressionNode>(forStmt.Condition);
        Assert.Equal("i", Assert.IsType<IdentifierNode>(condition.Left).Name);
        Assert.Equal(10.0, Assert.IsType<NumberLiteralNode>(condition.Right).Value);

        var increment = Assert.IsType<UnaryExpressionNode>(forStmt.Update);
        Assert.Equal("++", increment.Operator);
        Assert.Equal("i", Assert.IsType<IdentifierNode>(increment.Argument).Name);

        var body = forStmt.Body;
        Assert.Single(body.Body);
        var callStmt = Assert.IsType<ExpressionStatementNode>(body.Body[0]);
        var callExpr = Assert.IsType<CallExpressionNode>(callStmt.Expression);
        var memberExpr = Assert.IsType<MemberExpressionNode>(callExpr.Callee);
        var property = Assert.IsType<IdentifierNode>(memberExpr.Property);
        Assert.Equal("log", property.Name);
    }

    [Fact]
    public void ParseVarStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "var x = 42;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var varStmt = Assert.IsType<VarStatementNode>(program.Statements[0]);
        var identPattern = Assert.IsType<IdentifierPatternNode>(varStmt.Name);
        Assert.Equal("x", identPattern.Name);
        var value = Assert.IsType<NumberLiteralNode>(varStmt.Value);
        Assert.Equal(42.0, value.Value);
    }

    [Fact]
    public void ParseConstStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "const PI = 3.14;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var constStmt = Assert.IsType<ConstStatementNode>(program.Statements[0]);
        var identPattern = Assert.IsType<IdentifierPatternNode>(constStmt.Pattern);
        Assert.Equal("PI", identPattern.Name);
        var value = Assert.IsType<NumberLiteralNode>(constStmt.Value);
        Assert.Equal(3.14, value.Value);
    }

    [Fact]
    public void ParseDoWhileLoop_ShouldParseCorrectly()
    {
        // Arrange
        var input = "do { x++; } while (x < 10);";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var doWhileStmt = Assert.IsType<DoWhileStatementNode>(program.Statements[0]);

        var body = doWhileStmt.Body;
        Assert.Single(body.Body);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(body.Body[0]);
        var increment = Assert.IsType<UnaryExpressionNode>(exprStmt.Expression);
        Assert.Equal("++", increment.Operator);
        Assert.Equal("x", Assert.IsType<IdentifierNode>(increment.Argument).Name);

        var condition = Assert.IsType<LessThanExpressionNode>(doWhileStmt.Condition);
        Assert.Equal("x", Assert.IsType<IdentifierNode>(condition.Left).Name);
        Assert.Equal(10.0, Assert.IsType<NumberLiteralNode>(condition.Right).Value);
    }

    [Fact]
    public void ParseThrowStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "throw new Error(\"Something went wrong\");";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var throwStmt = Assert.IsType<ThrowStatementNode>(program.Statements[0]);
        var newExpr = Assert.IsType<NewExpressionNode>(throwStmt.Argument);
        Assert.Equal("Error", Assert.IsType<IdentifierNode>(newExpr.Callee).Name);
        Assert.Single(newExpr.Arguments);
        var arg = Assert.IsType<StringLiteralNode>(newExpr.Arguments[0]);
        Assert.Equal("Something went wrong", arg.Value);
    }
}
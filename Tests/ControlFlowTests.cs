using Xunit;
using Snip.AST;
using Snip.Lexer;
using Snip.Parser;

namespace Tests;

public class ControlFlowTests
{
    [Fact]
    public void ParseIfStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "if (x > 0) { return x; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var ifStmt = Assert.IsType<IfStatementNode>(program.Statements[0]);
        var condition = Assert.IsType<GreaterThanExpressionNode>(ifStmt.Condition);
        var left = Assert.IsType<IdentifierNode>(condition.Left);
        var right = Assert.IsType<NumberLiteralNode>(condition.Right);
        Assert.Equal("x", left.Name);
        Assert.Equal(0.0, right.Value);
        
        var consequence = Assert.IsType<BlockStatementNode>(ifStmt.Consequence);
        var returnStmt = Assert.IsType<ReturnStatementNode>(consequence.Body[0]);
        var returnValue = Assert.IsType<IdentifierNode>(returnStmt.Value);
        Assert.Equal("x", returnValue.Name);
    }

    [Fact]
    public void ParseIfElseStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "if (x > 0) { return x; } else { return 0; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var ifStmt = Assert.IsType<IfStatementNode>(program.Statements[0]);
        
        var condition = Assert.IsType<GreaterThanExpressionNode>(ifStmt.Condition);
        var consequence = Assert.IsType<BlockStatementNode>(ifStmt.Consequence);
        var alternative = Assert.IsType<BlockStatementNode>(ifStmt.Alternative);
        
        var returnStmt1 = Assert.IsType<ReturnStatementNode>(consequence.Body[0]);
        var returnStmt2 = Assert.IsType<ReturnStatementNode>(alternative.Body[0]);
        
        var returnValue1 = Assert.IsType<IdentifierNode>(returnStmt1.Value);
        var returnValue2 = Assert.IsType<NumberLiteralNode>(returnStmt2.Value);
        
        Assert.Equal("x", returnValue1.Name);
        Assert.Equal(0.0, returnValue2.Value);
    }

    [Fact]
    public void ParseNestedIfStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "if (x > 0) { if (y > 0) { return y; } else { return 0; } }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var outerIf = Assert.IsType<IfStatementNode>(program.Statements[0]);
        var outerConsequence = Assert.IsType<BlockStatementNode>(outerIf.Consequence);
        var innerIf = Assert.IsType<IfStatementNode>(outerConsequence.Body[0]);
        
        var condition = Assert.IsType<GreaterThanExpressionNode>(innerIf.Condition);
        var left = Assert.IsType<IdentifierNode>(condition.Left);
        var right = Assert.IsType<NumberLiteralNode>(condition.Right);
        Assert.Equal("y", left.Name);
        Assert.Equal(0.0, right.Value);
    }

    [Fact]
    public void ParseReturnStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "return 42;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var returnStmt = Assert.IsType<ReturnStatementNode>(program.Statements[0]);
        var value = Assert.IsType<NumberLiteralNode>(returnStmt.Value);
        Assert.Equal(42.0, value.Value);
    }

    [Fact]
    public void ParseReturnStatementWithoutValue_ShouldParseCorrectly()
    {
        // Arrange
        var input = "return;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var returnStmt = Assert.IsType<ReturnStatementNode>(program.Statements[0]);
        Assert.Null(returnStmt.Value);
    }

    [Fact]
    public void ParseContinueStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "continue;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var continueStmt = Assert.IsType<ContinueStatementNode>(program.Statements[0]);
    }

    [Fact]
    public void ParseBreakStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "break;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var breakStmt = Assert.IsType<BreakStatementNode>(program.Statements[0]);
    }

    [Fact]
    public void ParseSwitchStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "switch (x) { case 1: return 1; case 2: return 2; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var switchStmt = Assert.IsType<SwitchStatementNode>(program.Statements[0]);
        var expression = Assert.IsType<IdentifierNode>(switchStmt.Expression);
        Assert.Equal("x", expression.Name);
        
        Assert.Equal(2, switchStmt.Cases.Count);
        
        var case1 = Assert.IsType<CaseNode>(switchStmt.Cases[0]);
        var case1Test = Assert.IsType<NumberLiteralNode>(case1.Test);
        Assert.Equal(1.0, case1Test.Value);
        var case1Consequent = Assert.IsType<ReturnStatementNode>(case1.Consequent[0]);
        var case1Value = Assert.IsType<NumberLiteralNode>(case1Consequent.Value);
        Assert.Equal(1.0, case1Value.Value);
        
        var case2 = Assert.IsType<CaseNode>(switchStmt.Cases[1]);
        var case2Test = Assert.IsType<NumberLiteralNode>(case2.Test);
        Assert.Equal(2.0, case2Test.Value);
        var case2Consequent = Assert.IsType<ReturnStatementNode>(case2.Consequent[0]);
        var case2Value = Assert.IsType<NumberLiteralNode>(case2Consequent.Value);
        Assert.Equal(2.0, case2Value.Value);
    }

    [Fact]
    public void ParseSwitchStatementWithDefault_ShouldParseCorrectly()
    {
        // Arrange
        var input = "switch (x) { case 1: return 1; default: return 0; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var switchStmt = Assert.IsType<SwitchStatementNode>(program.Statements[0]);
        Assert.Single(switchStmt.Cases);
        
        var case1 = Assert.IsType<CaseNode>(switchStmt.Cases[0]);
        var case1Test = Assert.IsType<NumberLiteralNode>(case1.Test);
        Assert.Equal(1.0, case1Test.Value);
        
        var defaultCase = Assert.IsType<CaseNode>(switchStmt.DefaultCase);
        var defaultConsequent = Assert.IsType<ReturnStatementNode>(defaultCase.Consequent[0]);
        var defaultValue = Assert.IsType<NumberLiteralNode>(defaultConsequent.Value);
        Assert.Equal(0.0, defaultValue.Value);
    }

    [Fact]
    public void ParseBlockStatement_ShouldParseCorrectly()
    {
        // Arrange
        var input = "{ let x = 1; let y = 2; x + y; }";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var blockStmt = Assert.IsType<BlockStatementNode>(program.Statements[0]);
        Assert.Equal(3, blockStmt.Body.Count);
        
        var letX = Assert.IsType<LetStatementNode>(blockStmt.Body[0]);
        var letY = Assert.IsType<LetStatementNode>(blockStmt.Body[1]);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(blockStmt.Body[2]);
        
        var identPatternX = Assert.IsType<IdentifierPatternNode>(letX.Pattern);
        Assert.Equal("x", identPatternX.Name);
        var identPatternY = Assert.IsType<IdentifierPatternNode>(letY.Pattern);
        Assert.Equal("y", identPatternY.Name);
        
        var addExpr = Assert.IsType<AddExpressionNode>(exprStmt.Expression);
        var left = Assert.IsType<IdentifierNode>(addExpr.Left);
        var right = Assert.IsType<IdentifierNode>(addExpr.Right);
        Assert.Equal("x", left.Name);
        Assert.Equal("y", right.Name);
    }
}
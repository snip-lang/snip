using Xunit;
using Snip.AST;
using Snip.Lexer;
using Snip.Parser;

namespace Tests;

public class AssignmentOperatorTests
{
    [Fact]
    public void ParseSimpleAssignment_ShouldParseCorrectly()
    {
        // Arrange
        var input = "x = 5;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var assignExpr = Assert.IsType<AssignmentExpressionNode>(exprStmt.Expression);
        
        Assert.Equal("=", assignExpr.Operator);
        Assert.Equal("x", Assert.IsType<IdentifierNode>(assignExpr.Left).Name);
        Assert.Equal(5.0, Assert.IsType<NumberLiteralNode>(assignExpr.Right).Value);
    }

    [Fact]
    public void ParseCompoundAssignment_ShouldParseCorrectly()
    {
        // Arrange
        var input = "x += 10;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var assignExpr = Assert.IsType<AssignmentExpressionNode>(exprStmt.Expression);
        
        Assert.Equal("+=", assignExpr.Operator);
        Assert.Equal("x", Assert.IsType<IdentifierNode>(assignExpr.Left).Name);
        Assert.Equal(10.0, Assert.IsType<NumberLiteralNode>(assignExpr.Right).Value);
    }

    [Fact]
    public void ParseAllAssignmentOperators_ShouldParseCorrectly()
    {
        // Test all assignment operators
        var operators = new[] { "=", "+=", "-=", "*=", "/=", "%=" };
        
        foreach (var op in operators)
        {
            var input = $"x {op} 5;";
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);

            // Act
            var program = parser.Parse();

            // Assert
            Assert.Single(program.Statements);
            var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
            var assignExpr = Assert.IsType<AssignmentExpressionNode>(exprStmt.Expression);
            
            Assert.Equal(op, assignExpr.Operator);
            Assert.Equal("x", Assert.IsType<IdentifierNode>(assignExpr.Left).Name);
            Assert.Equal(5.0, Assert.IsType<NumberLiteralNode>(assignExpr.Right).Value);
        }
    }

    [Fact]
    public void ParseChainedAssignment_ShouldParseCorrectly()
    {
        // Test right-associative chaining: a = b = c = 10
        var input = "a = b = c = 10;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var assignExpr = Assert.IsType<AssignmentExpressionNode>(exprStmt.Expression);
        
        // Should be: a = (b = (c = 10))
        Assert.Equal("=", assignExpr.Operator);
        var innerAssign1 = Assert.IsType<AssignmentExpressionNode>(assignExpr.Right);
        Assert.Equal("=", innerAssign1.Operator);
        Assert.Equal("b", Assert.IsType<IdentifierNode>(innerAssign1.Left).Name);
        var innerAssign2 = Assert.IsType<AssignmentExpressionNode>(innerAssign1.Right);
        Assert.Equal("=", innerAssign2.Operator);
        Assert.Equal("c", Assert.IsType<IdentifierNode>(innerAssign2.Left).Name);
        Assert.Equal(10.0, Assert.IsType<NumberLiteralNode>(innerAssign2.Right).Value);
    }

    [Fact]
    public void ParseAssignmentWithComplexExpression_ShouldParseCorrectly()
    {
        // Test assignment with complex right-hand expression
        var input = "x = (y + z) * 2;";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        // Act
        var program = parser.Parse();

        // Assert
        Assert.Single(program.Statements);
        var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
        var assignExpr = Assert.IsType<AssignmentExpressionNode>(exprStmt.Expression);
        
        Assert.Equal("=", assignExpr.Operator);
        Assert.Equal("x", Assert.IsType<IdentifierNode>(assignExpr.Left).Name);
        
        // Right side should be: (y + z) * 2
        var multiplyExpr = Assert.IsType<MultiplyExpressionNode>(assignExpr.Right);
        var addExpr = Assert.IsType<AddExpressionNode>(multiplyExpr.Left);
        Assert.Equal("y", Assert.IsType<IdentifierNode>(addExpr.Left).Name);
        Assert.Equal("z", Assert.IsType<IdentifierNode>(addExpr.Right).Name);
        Assert.Equal(2.0, Assert.IsType<NumberLiteralNode>(multiplyExpr.Right).Value);
    }
}
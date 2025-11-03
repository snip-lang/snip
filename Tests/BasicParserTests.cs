using Xunit;
using Snip.AST;
using Snip.Lexer;
using Snip.Parser;

namespace Tests;

public class BasicParserTests
{
   [Fact]
   public void ParseIntegerLiteral_ShouldParseCorrectly()
   {
      // Arrange
      var input = "42;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var numberLiteral = Assert.IsType<NumberLiteralNode>(exprStmt.Expression);
      Assert.Equal(42.0, numberLiteral.Value);
   }

   [Fact]
   public void ParseStringLiteral_ShouldParseCorrectly()
   {
      // Arrange
      var input = "\"hello world\";";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var stringLiteral = Assert.IsType<StringLiteralNode>(exprStmt.Expression);
      Assert.Equal("hello world", stringLiteral.Value);
   }

   [Fact]
   public void ParseBooleanLiteral_ShouldParseCorrectly()
   {
      // Arrange
      var input = "true;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var boolLiteral = Assert.IsType<BooleanLiteralNode>(exprStmt.Expression);
      Assert.True(boolLiteral.Value);
   }

   [Fact]
   public void ParseIdentifierExpression_ShouldParseCorrectly()
   {
      // Arrange
      var input = "variable;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var identifier = Assert.IsType<IdentifierNode>(exprStmt.Expression);
      Assert.Equal("variable", identifier.Name);
   }

   [Fact]
   public void ParseLetStatement_ShouldParseCorrectly()
   {
      // Arrange
      var input = "let x = 42;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var letStmt = Assert.IsType<LetStatementNode>(program.Statements[0]);
      var identPattern = Assert.IsType<IdentifierPatternNode>(letStmt.Pattern);
      Assert.Equal("x", identPattern.Name);
      var value = Assert.IsType<NumberLiteralNode>(letStmt.Value);
      Assert.Equal(42L, value.Value);
   }

   [Fact]
   public void ParseSimpleAddition_ShouldParseCorrectly()
   {
      // Arrange
      var input = "1 + 2;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var addExpr = Assert.IsType<AddExpressionNode>(exprStmt.Expression);
      var left = Assert.IsType<NumberLiteralNode>(addExpr.Left);
      var right = Assert.IsType<NumberLiteralNode>(addExpr.Right);
      Assert.Equal(1L, left.Value);
      Assert.Equal(2L, right.Value);
   }

   [Fact]
   public void ParseParenthesizedExpression_ShouldParseCorrectly()
   {
      // Arrange
      var input = "(1 + 2) * 3;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var mulExpr = Assert.IsType<MultiplyExpressionNode>(exprStmt.Expression);
      var left = Assert.IsType<AddExpressionNode>(mulExpr.Left);
      var right = Assert.IsType<NumberLiteralNode>(mulExpr.Right);
      Assert.Equal(3L, right.Value);
      
      var addLeft = Assert.IsType<NumberLiteralNode>(left.Left);
      var addRight = Assert.IsType<NumberLiteralNode>(left.Right);
      Assert.Equal(1L, addLeft.Value);
      Assert.Equal(2L, addRight.Value);
   }

   [Fact]
   public void ParseEmptyArray_ShouldParseCorrectly()
   {
      // Arrange
      var input = "[];";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var arrayExpr = Assert.IsType<ArrayExpressionNode>(exprStmt.Expression);
      Assert.Empty(arrayExpr.Elements);
   }

   [Fact]
   public void ParseArrayWithElements_ShouldParseCorrectly()
   {
      // Arrange
      var input = "[1, 2, \"three\"];";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var arrayExpr = Assert.IsType<ArrayExpressionNode>(exprStmt.Expression);
      Assert.Equal(3, arrayExpr.Elements.Count);
      
      var elem1 = Assert.IsType<NumberLiteralNode>(arrayExpr.Elements[0]);
      var elem2 = Assert.IsType<NumberLiteralNode>(arrayExpr.Elements[1]);
      var elem3 = Assert.IsType<StringLiteralNode>(arrayExpr.Elements[2]);
      Assert.Equal(1L, elem1.Value);
      Assert.Equal(2L, elem2.Value);
      Assert.Equal("three", elem3.Value);
   }

   [Fact]
   public void ParseEmptyObject_ShouldParseCorrectly()
   {
      // Arrange
      var input = "{};";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var objectExpr = Assert.IsType<ObjectExpressionNode>(exprStmt.Expression);
      Assert.Empty(objectExpr.Properties);
   }

   [Fact]
   public void ParseObjectWithProperties_ShouldParseCorrectly()
   {
      // Arrange
      var input = "{\"name\": \"John\", \"age\": 30};";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var objectExpr = Assert.IsType<ObjectExpressionNode>(exprStmt.Expression);
      Assert.Equal(2, objectExpr.Properties.Count);
      
      var prop1 = objectExpr.Properties[0];
      var prop2 = objectExpr.Properties[1];
      Assert.Equal("name", prop1.Key);
      Assert.Equal("age", prop2.Key);
      
      var value1 = Assert.IsType<StringLiteralNode>(prop1.Value);
      var value2 = Assert.IsType<NumberLiteralNode>(prop2.Value);
      Assert.Equal("John", value1.Value);
      Assert.Equal(30L, value2.Value);
   }

   [Fact]
   public void ParseMultipleStatements_ShouldParseCorrectly()
   {
      // Arrange
      var input = "let x = 1; let y = 2; x + y;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Equal(3, program.Statements.Count);
      
      var letX = Assert.IsType<LetStatementNode>(program.Statements[0]);
      var letY = Assert.IsType<LetStatementNode>(program.Statements[1]);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[2]);
      
      var identPatternX = Assert.IsType<IdentifierPatternNode>(letX.Pattern);
      Assert.Equal("x", identPatternX.Name);
      var identPatternY = Assert.IsType<IdentifierPatternNode>(letY.Pattern);
      Assert.Equal("y", identPatternY.Name);
      
      var xValue = Assert.IsType<NumberLiteralNode>(letX.Value);
      var yValue = Assert.IsType<NumberLiteralNode>(letY.Value);
      Assert.Equal(1L, xValue.Value);
      Assert.Equal(2L, yValue.Value);
      
      var addExpr = Assert.IsType<AddExpressionNode>(exprStmt.Expression);
      var left = Assert.IsType<IdentifierNode>(addExpr.Left);
      var right = Assert.IsType<IdentifierNode>(addExpr.Right);
      Assert.Equal("x", left.Name);
      Assert.Equal("y", right.Name);
   }
}
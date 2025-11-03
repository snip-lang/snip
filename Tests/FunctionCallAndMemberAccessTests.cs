using Xunit;
using Snip.AST;
using Snip.Lexer;
using Snip.Parser;

namespace Tests;

public class FunctionCallAndMemberAccessTests
{
   [Fact]
   public void ParseSimpleFunctionCall_ShouldParseCorrectly()
   {
      // Arrange
      var input = "func();";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var callExpr = Assert.IsType<CallExpressionNode>(exprStmt.Expression);
      var callee = Assert.IsType<IdentifierNode>(callExpr.Callee);
      Assert.Equal("func", callee.Name);
      Assert.Empty(callExpr.Arguments);
   }

   [Fact]
   public void ParseFunctionCallWithArguments_ShouldParseCorrectly()
   {
      // Arrange
      var input = "add(1, 2);";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var callExpr = Assert.IsType<CallExpressionNode>(exprStmt.Expression);
      var callee = Assert.IsType<IdentifierNode>(callExpr.Callee);
      Assert.Equal("add", callee.Name);
      Assert.Equal(2, callExpr.Arguments.Count);
      
      var arg1 = Assert.IsType<NumberLiteralNode>(callExpr.Arguments[0]);
      var arg2 = Assert.IsType<NumberLiteralNode>(callExpr.Arguments[1]);
      Assert.Equal(1.0, arg1.Value);
      Assert.Equal(2.0, arg2.Value);
   }

   [Fact]
   public void ParseChainedFunctionCall_ShouldParseCorrectly()
   {
      // Arrange
      var input = "obj.method();";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var callExpr = Assert.IsType<CallExpressionNode>(exprStmt.Expression);
      var memberExpr = Assert.IsType<MemberExpressionNode>(callExpr.Callee);
      var obj = Assert.IsType<IdentifierNode>(memberExpr.Object);
      var property = Assert.IsType<IdentifierNode>(memberExpr.Property);
      Assert.Equal("obj", obj.Name);
      Assert.Equal("method", property.Name);
      Assert.False(memberExpr.Computed);
      Assert.Empty(callExpr.Arguments);
   }

   [Fact]
   public void ParseSimpleMemberAccess_ShouldParseCorrectly()
   {
      // Arrange
      var input = "obj.prop;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var memberExpr = Assert.IsType<MemberExpressionNode>(exprStmt.Expression);
      var obj = Assert.IsType<IdentifierNode>(memberExpr.Object);
      var property = Assert.IsType<IdentifierNode>(memberExpr.Property);
      Assert.Equal("obj", obj.Name);
      Assert.Equal("prop", property.Name);
      Assert.False(memberExpr.Computed);
   }

   [Fact]
   public void ParseChainedMemberAccess_ShouldParseCorrectly()
   {
      // Arrange
      var input = "a.b.c;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var outerMember = Assert.IsType<MemberExpressionNode>(exprStmt.Expression);
      var innerMember = Assert.IsType<MemberExpressionNode>(outerMember.Object);
      
      var a = Assert.IsType<IdentifierNode>(innerMember.Object);
      var b = Assert.IsType<IdentifierNode>(innerMember.Property);
      var c = Assert.IsType<IdentifierNode>(outerMember.Property);
      
      Assert.Equal("a", a.Name);
      Assert.Equal("b", b.Name);
      Assert.Equal("c", c.Name);
   }

   [Fact]
   public void ParseFunctionCallOnMemberAccess_ShouldParseCorrectly()
   {
      // Arrange
      var input = "obj.method(1, 2);";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var callExpr = Assert.IsType<CallExpressionNode>(exprStmt.Expression);
      var memberExpr = Assert.IsType<MemberExpressionNode>(callExpr.Callee);
      var obj = Assert.IsType<IdentifierNode>(memberExpr.Object);
      var method = Assert.IsType<IdentifierNode>(memberExpr.Property);
      
      Assert.Equal("obj", obj.Name);
      Assert.Equal("method", method.Name);
      Assert.Equal(2, callExpr.Arguments.Count);
      
      var arg1 = Assert.IsType<NumberLiteralNode>(callExpr.Arguments[0]);
      var arg2 = Assert.IsType<NumberLiteralNode>(callExpr.Arguments[1]);
      Assert.Equal(1.0, arg1.Value);
      Assert.Equal(2.0, arg2.Value);
   }

   [Fact]
   public void ParseNewExpression_ShouldParseCorrectly()
   {
      // Arrange
      var input = "new MyClass();";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var newExpr = Assert.IsType<NewExpressionNode>(exprStmt.Expression);
      var callee = Assert.IsType<IdentifierNode>(newExpr.Callee);
      Assert.Equal("MyClass", callee.Name);
      Assert.Empty(newExpr.Arguments);
   }

   [Fact]
   public void ParseNewExpressionWithArguments_ShouldParseCorrectly()
   {
      // Arrange
      var input = "new Person(\"John\", 30);";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var newExpr = Assert.IsType<NewExpressionNode>(exprStmt.Expression);
      var callee = Assert.IsType<IdentifierNode>(newExpr.Callee);
      Assert.Equal("Person", callee.Name);
      Assert.Equal(2, newExpr.Arguments.Count);
      
      var arg1 = Assert.IsType<StringLiteralNode>(newExpr.Arguments[0]);
      var arg2 = Assert.IsType<NumberLiteralNode>(newExpr.Arguments[1]);
      Assert.Equal("John", arg1.Value);
      Assert.Equal(30.0, arg2.Value);
   }

   [Fact]
   public void ParseComplexExpression_ShouldParseCorrectly()
   {
      // Arrange
      var input = "new MyClass().method().prop;";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      
      // Should be: ((new MyClass()).method()).prop
      var outerMember = Assert.IsType<MemberExpressionNode>(exprStmt.Expression);
      var prop = Assert.IsType<IdentifierNode>(outerMember.Property);
      Assert.Equal("prop", prop.Name);
      
      var innerCall = Assert.IsType<CallExpressionNode>(outerMember.Object);
      var innerMember = Assert.IsType<MemberExpressionNode>(innerCall.Callee);
      var method = Assert.IsType<IdentifierNode>(innerMember.Property);
      Assert.Equal("method", method.Name);
      Assert.Empty(innerCall.Arguments);
      
      var newExpr = Assert.IsType<NewExpressionNode>(innerMember.Object);
      var constructor = Assert.IsType<IdentifierNode>(newExpr.Callee);
      Assert.Equal("MyClass", constructor.Name);
      Assert.Empty(newExpr.Arguments);
   }

   [Fact]
   public void ParseFunctionCallWithComplexArguments_ShouldParseCorrectly()
   {
      // Arrange
      var input = "func(1 + 2, obj.prop, new Array());";
      var lexer = new Lexer(input);
      var parser = new Parser(lexer);

      // Act
      var program = parser.Parse();

      // Assert
      Assert.Single(program.Statements);
      var exprStmt = Assert.IsType<ExpressionStatementNode>(program.Statements[0]);
      var callExpr = Assert.IsType<CallExpressionNode>(exprStmt.Expression);
      var callee = Assert.IsType<IdentifierNode>(callExpr.Callee);
      Assert.Equal("func", callee.Name);
      Assert.Equal(3, callExpr.Arguments.Count);
      
      // First argument: 1 + 2
      var arg1 = Assert.IsType<AddExpressionNode>(callExpr.Arguments[0]);
      var left1 = Assert.IsType<NumberLiteralNode>(arg1.Left);
      var right1 = Assert.IsType<NumberLiteralNode>(arg1.Right);
      Assert.Equal(1.0, left1.Value);
      Assert.Equal(2.0, right1.Value);
      
      // Second argument: obj.prop
      var arg2 = Assert.IsType<MemberExpressionNode>(callExpr.Arguments[1]);
      var obj = Assert.IsType<IdentifierNode>(arg2.Object);
      var prop = Assert.IsType<IdentifierNode>(arg2.Property);
      Assert.Equal("obj", obj.Name);
      Assert.Equal("prop", prop.Name);
      
      // Third argument: new Array()
      var arg3 = Assert.IsType<NewExpressionNode>(callExpr.Arguments[2]);
      var arrayCtor = Assert.IsType<IdentifierNode>(arg3.Callee);
      Assert.Equal("Array", arrayCtor.Name);
      Assert.Empty(arg3.Arguments);
   }
}
using System.Diagnostics;
using Snip.Lexer;
using Snip.AST;
namespace Snip.Parser;

public class Parser
{
   private int _ptr = 0;
   private List<Token> _token { get; set; }

   // Precedence levels (higher number = higher precedence)
   private static readonly Dictionary<TokenType, int> _precedence = new() 
   {
      { TokenType.Plus, 1 },
      { TokenType.Minus, 1 },
      { TokenType.Multiply, 2 },
      { TokenType.Divide, 2 },
      { TokenType.Modulo, 2 },
      { TokenType.Power, 3 },
   };

   public Parser(Lexer.Lexer lexer)
   {
      _token = lexer.Tokenize();
   }

   private Token? _peek(int lookahead = 0)
   {
      if (_ptr + lookahead < _token.Count) 
         return _token[_ptr + lookahead];
      return null;
   }

   public Token? _next()
   {
      var token = _peek();
      _ptr++;
      return token;
   }

   private int GetPrecedence(TokenType tokenType)
   {
      return _precedence.TryGetValue(tokenType, out var precedence) ? precedence : 0;
   }

   // Main Pratt parser entry point
   private ExpressionNode ParseExpression(int precedence = 0)
   {
      var left = ParsePrefixExpression();
      
      while (true)
      {
         var currentToken = _peek();
         if (currentToken == null) break;
         
         var currentPrecedence = GetPrecedence(currentToken.Type);
         if (currentPrecedence <= precedence) break;
         
         left = ParseInfixExpression(left, currentToken.Type);
      }
      
      return left;
   }

   private ExpressionNode ParsePrefixExpression()
   {
      var token = _peek();
      if (token == null)
         throw new ParsingError("Expected expression");
         
      return token.Type switch
      {
         TokenType.Number => ParseNumberExpression(),
         TokenType.String => ParseStringExpression(),
         TokenType.Boolean => ParseBooleanExpression(),
         TokenType.Identifier => ParseIdentifierExpression(),
         TokenType.LeftParen => ParseParenthesizedExpression(),
         TokenType.LeftBrace => ParseObjectExpression(),
         TokenType.LeftBracket => ParseArrayExpression(),
         TokenType.Plus => ParseUnaryExpression(),
         TokenType.Minus => ParseUnaryExpression(),
         _ => throw new ParsingError($"Unexpected token: {token.Type}")
      };
   }

   private ExpressionNode ParseInfixExpression(ExpressionNode left, TokenType operatorType)
   {
      _next(); // Consume the operator
      
      var precedence = GetPrecedence(operatorType);
      var right = ParseExpression(precedence);
      
      return operatorType switch
      {
         TokenType.Plus => new AddExpressionNode(left, right),
         TokenType.Minus => new SubtractExpressionNode(left, right),
         TokenType.Multiply => new MultiplyExpressionNode(left, right),
         TokenType.Divide => new DivideExpressionNode(left, right),
         TokenType.Modulo => new ModuloExpressionNode(left, right),
         TokenType.Power => new PowerExpressionNode(left, right),
         _ => throw new ParsingError($"Unexpected infix operator: {operatorType}")
      };
   }

   private ExpressionNode ParseUnaryExpression()
   {
      var token = _peek();
      if (token?.Type != TokenType.Plus && token?.Type != TokenType.Minus)
         throw new ParsingError("Expected unary operator");
      
      _next(); // Consume the operator
      var argument = ParseExpression(GetPrecedence(TokenType.Plus)); // Unary operators have high precedence
      
      return new UnaryExpressionNode
      {
         Operator = token.Value,
         Argument = argument,
         IsPrefix = true
      };
   }

   private LetStatementNode ParseLetStatement()
   {
      // let name = expr
      var node = new LetStatementNode();
      var tok = _peek();
      if (tok == null || tok.Type != TokenType.Let)
         throw new ParsingError("Expected let");

      _next();
      tok = _peek();
      if (tok is not { Type: TokenType.Identifier })
         throw new ParsingError("Expected identifier");
      node.Name = new IdentifierNode(tok.Value);

      _next();
      tok = _peek();
      if (tok is not { Type: TokenType.Assign })
         throw new ParsingError("Expected =");

      _next();
      node.Value = ParseExpression();

      tok = _peek();
      if (tok is not { Type:  TokenType.Semicolon}) throw new ParsingError("Expected semicolon in assignment");
      _next();
      return node;
   }

   private ExpressionNode ParseParenthesizedExpression()
   {
      _next(); // Consume '('
      var expression = ParseExpression();
      if (_peek()?.Type != TokenType.RightParen)
         throw new ParsingError("Expected ')'");
      _next(); // Consume ')'
      return expression;
   }

   private ExpressionNode ParseArrayExpression()
   {
      if (_peek()?.Type != TokenType.LeftBracket) throw new ParsingError("Expected ','");
      _next();
      var elements = new List<ExpressionNode>();
      
      while (_peek() is not { Type: TokenType.RightBracket })
      {
         var element = ParseExpression();
         elements.Add(element);
         
         if (_peek()?.Type == TokenType.Comma)
         {
            _next();
         }
      }
      
      _next(); 
      return new ArrayExpressionNode(elements);
   }

   private ExpressionNode ParseObjectExpression()
   {
      _next(); // Consume '{'
      var properties = new List<PropertyNode>();
      
      while (_peek() is not { Type: TokenType.RightBrace })
      {
         var property = ParseProperty();
         properties.Add(property);
         
         // Handle comma separator
         if (_peek()?.Type == TokenType.Comma)
         {
            _next(); // Consume comma
         }
      }
      
      _next(); // Consume '}'
      return new ObjectExpressionNode(properties);
   }

   private PropertyNode ParseProperty()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.String })
         throw new ParsingError("Expected string");
      _next();
      var key = tok.Value;
      
      // Handle colon separator
      if (_peek()?.Type == TokenType.Colon)
      {
         _next(); // Consume ':'
      }
      
      var value = ParseExpression();
      return new PropertyNode(key, value);
   }

   private IdentifierNode ParseIdentifierExpression()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.Identifier })
         throw new ParsingError("Expected identifier");
      _next();
      return new IdentifierNode(tok.Value);
   }

   private IntegerLiteralNode ParseNumberExpression()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.Number })
         throw new ParsingError("Expected number");
      _next();
      return new IntegerLiteralNode(tok.Value);
   }

   private StringLiteralNode ParseStringExpression()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.String })
         throw new ParsingError("Expected string");
      _next();
      return new StringLiteralNode(tok.Value);
   }

   private BooleanLiteralNode ParseBooleanExpression()
   {
      var tok = _peek();
      if (tok is not { Type: TokenType.Boolean })
         throw new ParsingError("Expected boolean");
      _next();
      return new BooleanLiteralNode(tok.Value == "true");
   }

   private StatementNode ParseStatement(Token tok)
   {
      return tok.Type switch
      {
         TokenType.Let => ParseLetStatement(),
         TokenType.EndOfFile => new EOF(),
         _ => throw new Exception($"Unexpected token: {tok.Type}")
      };
   }

   public ProgramNode parse()
   {
      Console.WriteLine("Parsing program");
      var program = new ProgramNode();
      while (_peek() != null)
      {
         var tok = _peek();
         if (tok == null)
         {
            return program;
         }
         if (tok.Type == TokenType.EndOfFile) break;
         var node = this.ParseStatement(tok);
         program.Statements.Add(node);
      }
      Console.WriteLine("Parsing program finished");
      return program;
   }

   public void PrintParseTree(ProgramNode program, int indent = 0)
   {
      PrintNode(program, indent);
   }

   private void PrintNode(AstNode node, int indent = 0)
   {
      if (node == null) return;

      string indentStr = new string(' ', indent * 2);
      string prefix = indent > 0 ? "├─ " : "";
      
      Console.WriteLine($"{indentStr}{prefix}{node.NodeType}{GetNodeDetails(node)}");

      switch (node)
      {
         case ProgramNode programNode:
            foreach (var statement in programNode.Statements)
            {
               PrintNode(statement, indent + 1);
            }
            break;

         case LetStatementNode letNode:
            if (letNode.Name != null)
               PrintNode(letNode.Name, indent + 1);
            if (letNode.Value != null)
               PrintNode(letNode.Value, indent + 1);
            break;

         case AddExpressionNode addNode:
            PrintNode(addNode.Left, indent + 1);
            PrintNode(addNode.Right, indent + 1);
            break;

         case SubtractExpressionNode subNode:
            PrintNode(subNode.Left, indent + 1);
            PrintNode(subNode.Right, indent + 1);
            break;

         case MultiplyExpressionNode mulNode:
            PrintNode(mulNode.Left, indent + 1);
            PrintNode(mulNode.Right, indent + 1);
            break;

         case DivideExpressionNode divNode:
            PrintNode(divNode.Left, indent + 1);
            PrintNode(divNode.Right, indent + 1);
            break;

         case ModuloExpressionNode modNode:
            PrintNode(modNode.Left, indent + 1);
            PrintNode(modNode.Right, indent + 1);
            break;

         case PowerExpressionNode powNode:
            PrintNode(powNode.Left, indent + 1);
            PrintNode(powNode.Right, indent + 1);
            break;

         case UnaryExpressionNode unaryNode:
            PrintNode(unaryNode.Argument, indent + 1);
            break;

         case ArrayExpressionNode arrayNode:
            if (arrayNode.Elements != null)
            {
               foreach (var element in arrayNode.Elements)
               {
                  PrintNode(element, indent + 1);
               }
            }
            break;

         case ObjectExpressionNode objNode:
            foreach (var property in objNode.Properties)
            {
               PrintNode(property, indent + 1);
            }
            break;

         case PropertyNode propNode:
            PrintNode(propNode.Value, indent + 1);
            break;

         case CallExpressionNode callNode:
            PrintNode(callNode.Callee, indent + 1);
            foreach (var arg in callNode.Arguments)
            {
               PrintNode(arg, indent + 1);
            }
            break;

         case MemberExpressionNode memberNode:
            PrintNode(memberNode.Object, indent + 1);
            PrintNode(memberNode.Property, indent + 1);
            break;

         case ConditionalExpressionNode condNode:
            PrintNode(condNode.Test, indent + 1);
            PrintNode(condNode.Consequent, indent + 1);
            PrintNode(condNode.Alternative, indent + 1);
            break;

         case AssignmentExpressionNode assignNode:
            PrintNode(assignNode.Left, indent + 1);
            PrintNode(assignNode.Right, indent + 1);
            break;

         case BinaryExpressionNode binaryNode:
            PrintNode(binaryNode.Left, indent + 1);
            PrintNode(binaryNode.Right, indent + 1);
            break;

         case FunctionExpressionNode funcNode:
            if (funcNode.Name != null)
               PrintNode(funcNode.Name, indent + 1);
            foreach (var param in funcNode.Parameters)
            {
               PrintNode(param, indent + 1);
            }
            PrintNode(funcNode.Body, indent + 1);
            break;

         case ArrowFunctionExpressionNode arrowNode:
            foreach (var param in arrowNode.Parameters)
            {
               PrintNode(param, indent + 1);
            }
            PrintNode(arrowNode.Body, indent + 1);
            break;

         case BlockStatementNode blockNode:
            foreach (var statement in blockNode.Statements)
            {
               PrintNode(statement, indent + 1);
            }
            break;

         case IfStatementNode ifNode:
            PrintNode(ifNode.Condition, indent + 1);
            PrintNode(ifNode.Consequence, indent + 1);
            if (ifNode.Alternative != null)
               PrintNode(ifNode.Alternative, indent + 1);
            break;

         case WhileStatementNode whileNode:
            PrintNode(whileNode.Condition, indent + 1);
            PrintNode(whileNode.Body, indent + 1);
            break;

         case ForStatementNode forNode:
            if (forNode.Initializer != null)
               PrintNode(forNode.Initializer, indent + 1);
            if (forNode.Condition != null)
               PrintNode(forNode.Condition, indent + 1);
            if (forNode.Update != null)
               PrintNode(forNode.Update, indent + 1);
            PrintNode(forNode.Body, indent + 1);
            break;

         case ReturnStatementNode returnNode:
            if (returnNode.Argument != null)
               PrintNode(returnNode.Argument, indent + 1);
            break;

         case ExpressionStatementNode exprStmtNode:
            PrintNode(exprStmtNode.Expression, indent + 1);
            break;

         case FunctionDeclarationNode funcDeclNode:
            PrintNode(funcDeclNode.Name, indent + 1);
            foreach (var param in funcDeclNode.Parameters)
            {
               PrintNode(param, indent + 1);
            }
            PrintNode(funcDeclNode.Body, indent + 1);
            break;

         case ParameterNode paramNode:
            PrintNode(paramNode.Name, indent + 1);
            if (paramNode.DefaultValue != null)
               PrintNode(paramNode.DefaultValue, indent + 1);
            break;

         case ClassDeclarationNode classNode:
            PrintNode(classNode.Name, indent + 1);
            if (classNode.SuperClass != null)
               PrintNode(classNode.SuperClass, indent + 1);
            foreach (var member in classNode.Members)
            {
               PrintNode(member, indent + 1);
            }
            break;

         case ClassMemberNode memberDeclNode:
            if (memberDeclNode.Value != null)
               PrintNode(memberDeclNode.Value, indent + 1);
            break;

         case SwitchStatementNode switchNode:
            PrintNode(switchNode.Expression, indent + 1);
            foreach (var caseNode in switchNode.Cases)
            {
               PrintNode(caseNode, indent + 1);
            }
            if (switchNode.DefaultCase != null)
               PrintNode(switchNode.DefaultCase, indent + 1);
            break;

         case CaseNode caseNode:
            if (caseNode.Test != null)
               PrintNode(caseNode.Test, indent + 1);
            foreach (var stmt in caseNode.Consequent)
            {
               PrintNode(stmt, indent + 1);
            }
            break;

         case TryStatementNode tryNode:
            PrintNode(tryNode.Block, indent + 1);
            if (tryNode.Handler != null)
               PrintNode(tryNode.Handler, indent + 1);
            if (tryNode.Finalizer != null)
               PrintNode(tryNode.Finalizer, indent + 1);
            break;

         case CatchClauseNode catchNode:
            if (catchNode.Parameter != null)
               PrintNode(catchNode.Parameter, indent + 1);
            PrintNode(catchNode.Body, indent + 1);
            break;

         case TemplateLiteralNode templateNode:
            foreach (var quasi in templateNode.Quasis)
            {
               PrintNode(quasi, indent + 1);
            }
            foreach (var expr in templateNode.Expressions)
            {
               PrintNode(expr, indent + 1);
            }
            break;

         case ImportDeclarationNode importNode:
            foreach (var spec in importNode.Specifiers)
            {
               PrintNode(spec, indent + 1);
            }
            PrintNode(importNode.Source, indent + 1);
            break;

         case ImportSpecifierNode importSpecNode:
            PrintNode(importSpecNode.Local, indent + 1);
            if (importSpecNode.Imported != null)
               PrintNode(importSpecNode.Imported, indent + 1);
            break;

         case ExportDeclarationNode exportNode:
            PrintNode(exportNode.Declaration, indent + 1);
            break;

         case InterfaceDeclarationNode interfaceNode:
            PrintNode(interfaceNode.Name, indent + 1);
            foreach (var prop in interfaceNode.Properties)
            {
               PrintNode(prop, indent + 1);
            }
            foreach (var extend in interfaceNode.Extends)
            {
               PrintNode(extend, indent + 1);
            }
            break;
      }
   }

   private string GetNodeDetails(AstNode node)
   {
      return node switch
      {
         IntegerLiteralNode intNode => $" ({intNode.Value})",
         FloatLiteralNode floatNode => $" ({floatNode.Value})",
         StringLiteralNode strNode => $" (\"{strNode.Value}\")",
         BooleanLiteralNode boolNode => $" ({boolNode.Value})",
         IdentifierNode idNode => $" ({idNode.Name})",
         UnaryExpressionNode unaryNode => $" ({unaryNode.Operator})",
         BinaryExpressionNode binaryNode => $" ({binaryNode.Operator})",
         AssignmentExpressionNode assignNode => $" ({assignNode.Operator})",
         MemberExpressionNode memberNode => $" (computed: {memberNode.Computed})",
         PropertyNode propNode => $" (key: \"{propNode.Key}\", computed: {propNode.Computed}, shorthand: {propNode.Shorthand})",
         TemplateElementNode templateElementNode => $" (value: \"{templateElementNode.Value}\", tail: {templateElementNode.Tail})",
         ClassMemberNode classMemberNode => $" (name: {classMemberNode.Name}, visibility: {classMemberNode.Visibility}, static: {classMemberNode.IsStatic}, readonly: {classMemberNode.IsReadonly})",
         ParameterNode paramNode => $" (name: {paramNode.Name.Name}, type: {paramNode.TypeAnnotation ?? "none"})",
         FunctionDeclarationNode funcDeclNode => $" (name: {funcDeclNode.Name.Name}, async: {funcDeclNode.IsAsync}, returnType: {funcDeclNode.ReturnType ?? "none"})",
         ArrowFunctionExpressionNode arrowNode => $" (async: {arrowNode.IsAsync}, returnType: {arrowNode.ReturnType ?? "none"})",
         FunctionExpressionNode funcExprNode => $" (name: {funcExprNode.Name?.Name ?? "anonymous"}, async: {funcExprNode.IsAsync}, returnType: {funcExprNode.ReturnType ?? "none"})",
         _ => ""
      };
   }
}
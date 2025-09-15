using System.ComponentModel.DataAnnotations;

namespace Snip.AST;

public abstract class AstNode
{
    public abstract string NodeType { get; }
    public int Line { get; set; }
    public int Column { get; set; }
}

public abstract class StatementNode : AstNode { }
public abstract class ExpressionNode : AstNode { }
public abstract class DeclarationNode : StatementNode { }

public class ProgramNode : AstNode
{
    public List<StatementNode> Statements { get; set; } = new();
    public override string NodeType => "Program";
}

public class LetStatementNode : DeclarationNode
{
    public IdentifierNode? Name { get; set; }
    public ExpressionNode? Value { get; set; }
    public string? TypeAnnotation { get; set; }
    public override string NodeType => "LetStatement";
}

public class ConstStatementNode : DeclarationNode
{
    public IdentifierNode Name { get; set; }
    public ExpressionNode Value { get; set; }
    public string? TypeAnnotation { get; set; }
    public override string NodeType => "ConstStatement";
}

public class VarStatementNode : DeclarationNode
{
    public IdentifierNode Name { get; set; }
    public ExpressionNode? Value { get; set; }
    public string? TypeAnnotation { get; set; }
    public override string NodeType => "VarStatement";
}

public class IfStatementNode : StatementNode
{
    public ExpressionNode Condition { get; set; }
    public BlockStatementNode Consequence { get; set; }
    public BlockStatementNode? Alternative { get; set; }
    public override string NodeType => "IfStatement";
}

public class WhileStatementNode : StatementNode
{
    public required ExpressionNode Condition { get; set; }
    public BlockStatementNode Body { get; set; }
    public override string NodeType => "WhileStatement";
}

public class ForStatementNode : StatementNode
{
    public StatementNode? Initializer { get; set; }
    public ExpressionNode? Condition { get; set; }
    public ExpressionNode? Update { get; set; }
    public BlockStatementNode Body { get; set; }
    public override string NodeType => "ForStatement";
}

public class DoWhileStatementNode : StatementNode
{
    public BlockStatementNode Body { get; set; }
    public ExpressionNode Condition { get; set; }
    public override string NodeType => "DoWhileStatement";
}

public class SwitchStatementNode : StatementNode
{
    public ExpressionNode Expression { get; set; }
    public List<CaseNode> Cases { get; set; } = new();
    public CaseNode? DefaultCase { get; set; }
    public override string NodeType => "SwitchStatement";
}

public class CaseNode : StatementNode
{
    public ExpressionNode? Test { get; set; } // null for default case
    public List<StatementNode> Consequent { get; set; } = new();
    public override string NodeType => "Case";
}

public class FunctionDeclarationNode : DeclarationNode
{
    public IdentifierNode Name { get; set; }
    public List<ParameterNode> Parameters { get; set; } = new();
    public BlockStatementNode Body { get; set; }
    public string? ReturnType { get; set; }
    public bool IsAsync { get; set; }
    public override string NodeType => "FunctionDeclaration";
}

public class ParameterNode : AstNode
{
    public IdentifierNode Name { get; set; }
    public string? TypeAnnotation { get; set; }
    public ExpressionNode? DefaultValue { get; set; }
    public override string NodeType => "Parameter";
}

public class ClassDeclarationNode : DeclarationNode
{
    public IdentifierNode Name { get; set; }
    public IdentifierNode? SuperClass { get; set; }
    public List<ClassMemberNode> Members { get; set; } = new();
    public override string NodeType => "ClassDeclaration";
}

public class ClassMemberNode : AstNode
{
    public string Name { get; set; }
    public string Visibility { get; set; } = "public"; // public, private, protected
    public bool IsStatic { get; set; }
    public bool IsReadonly { get; set; }
    public string? TypeAnnotation { get; set; }
    public ExpressionNode? Value { get; set; }
    public override string NodeType => "ClassMember";
}

public class ReturnStatementNode : StatementNode
{
    public ExpressionNode? Argument { get; set; }
    public override string NodeType => "ReturnStatement";
}

public class BreakStatementNode : StatementNode
{
    public IdentifierNode? Label { get; set; }
    public override string NodeType => "BreakStatement";
}

public class ContinueStatementNode : StatementNode
{
    public IdentifierNode? Label { get; set; }
    public override string NodeType => "ContinueStatement";
}

public class ThrowStatementNode : StatementNode
{
    public ExpressionNode Argument { get; set; }
    public override string NodeType => "ThrowStatement";
}

public class TryStatementNode : StatementNode
{
    public BlockStatementNode Block { get; set; }
    public CatchClauseNode? Handler { get; set; }
    public BlockStatementNode? Finalizer { get; set; }
    public override string NodeType => "TryStatement";
}

public class CatchClauseNode : AstNode
{
    public IdentifierNode? Parameter { get; set; }
    public BlockStatementNode Body { get; set; }
    public override string NodeType => "CatchClause";
}

public class BlockStatementNode : StatementNode
{
    public List<StatementNode> Statements { get; set; } = new();
    public override string NodeType => "BlockStatement";
}

public class ExpressionStatementNode : StatementNode
{
    public ExpressionNode Expression { get; set; }
    public override string NodeType => "ExpressionStatement";
}

public class EOF : StatementNode
{
    public override string NodeType => "EOF";
}


public class IntegerLiteralNode(string value) : ExpressionNode
{
    public long Value { get; set; } = long.Parse(value);
    public override string NodeType => "IntegerLiteral";
}

public class FloatLiteralNode(double value) : ExpressionNode
{
    public double Value { get; set; } = value;
    public override string NodeType => "FloatLiteral";
}

public class StringLiteralNode(string value) : ExpressionNode
{
    public string Value { get; set; } = value;
    public override string NodeType => "StringLiteral";
}

public class BooleanLiteralNode(bool value) : ExpressionNode
{
    public bool Value { get; set; } = value;
    public override string NodeType => "BooleanLiteral";
}

public class NullLiteralNode : ExpressionNode
{
    public override string NodeType => "NullLiteral";
}

public class UndefinedLiteralNode : ExpressionNode
{
    public override string NodeType => "UndefinedLiteral";
}

public class IdentifierNode(string name) : ExpressionNode
{
    public string Name = name;
    public override string NodeType => "Identifier";
}

public class ThisExpressionNode : ExpressionNode
{
    public override string NodeType => "ThisExpression";
}

public class SuperExpressionNode : ExpressionNode
{
    public override string NodeType => "SuperExpression";
}

public class BinaryExpressionNode : ExpressionNode
{
    public ExpressionNode Left { get; set; }
    public string Operator { get; set; }
    public ExpressionNode Right { get; set; }
    public override string NodeType => "BinaryExpression";
}

public class UnaryExpressionNode : ExpressionNode
{
    public string Operator { get; set; }
    public ExpressionNode Argument { get; set; }
    public bool IsPrefix { get; set; } = true;
    public override string NodeType => "UnaryExpression";
}

public class PowerExpressionNode(ExpressionNode left, ExpressionNode right) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = left;
    public ExpressionNode Right { get; set; } = right;
    public override string NodeType => "PowerExpression";
}

public class AssignmentExpressionNode : ExpressionNode
{
    public ExpressionNode Left { get; set; }
    public string Operator { get; set; }
    public ExpressionNode Right { get; set; }
    public override string NodeType => "AssignmentExpression";
}

public class ConditionalExpressionNode : ExpressionNode
{
    public ExpressionNode Test { get; set; }
    public ExpressionNode Consequent { get; set; }
    public ExpressionNode Alternative { get; set; }
    public override string NodeType => "ConditionalExpression";
}

public class AddExpressionNode(ExpressionNode left, ExpressionNode right) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = left;
    public ExpressionNode Right { get; set; } = right;
    public override string NodeType => "AddExpression";
}

public class SubtractExpressionNode (ExpressionNode l, ExpressionNode r) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = l;
    public ExpressionNode Right { get; set; } = r;
    public override string NodeType => "SubtractExpression";
}

public class MultiplyExpressionNode(ExpressionNode l, ExpressionNode r) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = l;
    public ExpressionNode Right { get; set; } = r;
    public override string NodeType => "MultiplyExpression";
}


public class DivideExpressionNode (ExpressionNode l, ExpressionNode r) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = l;
    public ExpressionNode Right { get; set; } = r;
    public override string NodeType => "DivideExpression";
}

public class ModuloExpressionNode(ExpressionNode l, ExpressionNode r) : ExpressionNode
{
    public ExpressionNode Left { get; set; } = l;
    public ExpressionNode Right { get; set; } = r;
    public override string NodeType => "ModuloExpression";
}


public class FunctionExpressionNode : ExpressionNode
{
    public IdentifierNode? Name { get; set; }
    public List<ParameterNode> Parameters { get; set; } = new();
    public BlockStatementNode Body { get; set; }
    public string? ReturnType { get; set; }
    public bool IsAsync { get; set; }
    public override string NodeType => "FunctionExpression";
}

public class ArrowFunctionExpressionNode : ExpressionNode
{
    public List<ParameterNode> Parameters { get; set; } = new();
    public AstNode Body { get; set; } // Can be BlockStatement or Expression
    public string? ReturnType { get; set; }
    public bool IsAsync { get; set; }
    public override string NodeType => "ArrowFunctionExpression";
}

public class CallExpressionNode : ExpressionNode
{
    public ExpressionNode Callee { get; set; }
    public List<ExpressionNode> Arguments { get; set; } = new();
    public override string NodeType => "CallExpression";
}

public class NewExpressionNode : ExpressionNode
{
    public ExpressionNode Callee { get; set; }
    public List<ExpressionNode> Arguments { get; set; } = new();
    public override string NodeType => "NewExpression";
}

public class MemberExpressionNode(ExpressionNode obj, ExpressionNode p) : ExpressionNode
{
    public ExpressionNode Object { get; set; } = obj;
    public ExpressionNode Property { get; set; } = p;
    public bool Computed { get; set; }
    public override string NodeType => "MemberExpression";
}

public class ArrayExpressionNode (List<ExpressionNode> els) : ExpressionNode
{
    public List<ExpressionNode>? Elements { get; set; } = els;
    public override string NodeType => "ArrayExpression";
}

public class ObjectExpressionNode(List<PropertyNode> properties) : ExpressionNode
{
    public List<PropertyNode> Properties { get; set; } = properties;
    public override string NodeType => "ObjectExpression";
}

public class PropertyNode(string k, ExpressionNode v) : AstNode
{
    public string Key { get; set; } = k;
    public ExpressionNode Value { get; set; } = v;
    public bool Computed { get; set; }
    public bool Shorthand { get; set; }
    public override string NodeType => "Property";
}

public class TemplateLiteralNode : ExpressionNode
{
    public List<TemplateElementNode> Quasis { get; set; } = new();
    public List<ExpressionNode> Expressions { get; set; } = new();
    public override string NodeType => "TemplateLiteral";
}

public class TemplateElementNode : AstNode
{
    public string Value { get; set; }
    public bool Tail { get; set; }
    public override string NodeType => "TemplateElement";
}

public class SpreadElementNode : AstNode
{
    public ExpressionNode Argument { get; set; }
    public override string NodeType => "SpreadElement";
}

public class RestElementNode : AstNode
{
    public ExpressionNode Argument { get; set; }
    public override string NodeType => "RestElement";
}

public class TypeAnnotationNode : AstNode
{
    public string Type { get; set; }
    public override string NodeType => "TypeAnnotation";
}

public class InterfaceDeclarationNode : DeclarationNode
{
    public IdentifierNode Name { get; set; }
    public List<PropertyNode> Properties { get; set; } = new();
    public List<IdentifierNode> Extends { get; set; } = new();
    public override string NodeType => "InterfaceDeclaration";
}

public class ImportDeclarationNode : StatementNode
{
    public List<ImportSpecifierNode> Specifiers { get; set; } = new();
    public StringLiteralNode Source { get; set; }
    public override string NodeType => "ImportDeclaration";
}

public class ImportSpecifierNode : AstNode
{
    public IdentifierNode? Imported { get; set; }
    public IdentifierNode Local { get; set; }
    public override string NodeType => "ImportSpecifier";
}

public class ExportDeclarationNode : StatementNode
{
    public DeclarationNode Declaration { get; set; }
    public override string NodeType => "ExportDeclaration";
}

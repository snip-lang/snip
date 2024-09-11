#ifndef AST_H
#define AST_H

#include "globals.h"
#include "parser.h"
#include <unordered_map>
#include <vector>
#include <memory>

class ASTNode {
public:
  ASTNode();
  ASTNode(ParserTokenChunk &);
  void set_tok(ParserTokenChunk &);
  ParserTokenChunk get_tok() const;
  void set_children(std::vector<ASTNode>);
  std::vector<ASTNode> get_children() const;
  void add_child(ASTNode);
  void set_type(ParserToken &);
  ParserToken get_type() const;
  void set_value(std::variant<int, std::string, double>);
  std::variant<int, std::string, double> get_value() const;

private:
  std::vector<ASTNode> children;
  ASTNode* next_sibling;
  ASTNode* parent;
  ParserToken type;
  std::variant<int, std::string, double> value;
};


class ExprAST : public ASTNode {
private:
public:
  ExprAST();
};

class LiteralAST : public ExprAST {};

class StringAST : public LiteralAST {
public:
  StringAST(std::string value);

private:
  std::string value;
};

class DoubleAST : public LiteralAST {
private:
  std::string value;

public:
  DoubleAST(std::string value);
};

class CharAST : public LiteralAST {
public:
  CharAST(std::string value);

private:
  std::string value;
};

class IntAST : public LiteralAST {
private:
  std::string value;

public:
  IntAST(std::string value);
};

class VarDeclAST : public ASTNode {
public:
  VarDeclAST();
  void set_type(std::unique_ptr<ParserTokenChunk>& type);
  void set_ident_name(std::unique_ptr<ParserTokenChunk>& name);
  void set_value(std::unique_ptr<ExprAST> value);

private:
  std::unique_ptr<Token> type;
  std::string ident_name; 
  std::unique_ptr<ExprAST> value;
};

class BinaryExprAST : public ExprAST {
private:
  std::unique_ptr<ExprAST> LHS;
  std::unique_ptr<Token> op;
  std::unique_ptr<ExprAST> RHS;

public:
  BinaryExprAST();
};

class UnaryExprAST : public ExprAST {
public:
  UnaryExprAST();
};

class CallExprAST : public ExprAST {
public:
  CallExprAST();
};

class NumberExprAST : public ExprAST {
public:
  NumberExprAST();
};


#endif

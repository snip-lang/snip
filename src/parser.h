#include "globals.h"
#include <cstddef>
#include <memory>

#ifndef PARSER_H
#define PARSER_H

// struct to hold the const structs used during parsing
struct StmtPTCs {
  ParserTokenChunk semicolon = {ParserToken::SEMICOLON, ""};
  ParserTokenChunk if_stmt = {ParserToken::IFSTMT, ""};
  ParserTokenChunk expr = {ParserToken::EXPR, ""};
  ParserTokenChunk while_stmt = {ParserToken::WHILESTMT, ""};
  ParserTokenChunk fn_decl = {ParserToken::FNDECL, ""};
  ParserTokenChunk var_decl = {ParserToken::VARDECL, ""};
  ParserTokenChunk assign = {ParserToken::ASSIGN, ""};
  ParserTokenChunk assignstmt = {ParserToken::ASSIGNSTMT, ""};
  ParserTokenChunk add = {ParserToken::ADD, ""};
  ParserTokenChunk subtract = {ParserToken::SUBTRACT, ""};
  ParserTokenChunk multiply = {ParserToken::MULTIPLY, ""};
  ParserTokenChunk divide = {ParserToken::DIVIDE, ""};
  ParserTokenChunk left_paren = {ParserToken::LEFTPARENTHESIS, ""};
  ParserTokenChunk right_paren = {ParserToken::RIGHTPARENTHESIS, ""};
  ParserTokenChunk left_brace = {ParserToken::LEFTBRACE, ""};
  ParserTokenChunk right_brace = {ParserToken::RIGHTBRACE, ""};
  ParserTokenChunk stmt = {ParserToken::STMT, ""};
  ParserTokenChunk stmts = {ParserToken::STMTS, ""};
};

class PTNode {
public:
  PTNode(ParserTokenChunk);
  ~PTNode();
  void add_child(ParserTokenChunk &);
  void add_child(PTNode *);
  void add_sibling(ParserTokenChunk &);
  void add_sibling(PTNode *);
  void print(const int);

private:
  std::unique_ptr<ParserTokenChunk> val = nullptr;
  PTNode *first_child = nullptr;
  PTNode *last_child = nullptr;
  PTNode *next_sibling = nullptr;
  PTNode *prev_sibling = nullptr;
};

class ExprNode : public PTNode {};
class IntegerNode : public PTNode {};
class DoubleNode : public PTNode {};
class IfStmt : public PTNode {};
class WhileStmt : public PTNode {};

class Parser {
public:
  Parser(std::unique_ptr<TokenChunk[]> &);
  Parser();
  // ~Parser();
  TokenChunk peek() const noexcept;
  TokenChunk peek(int k) const;
  TokenChunk get() const noexcept;
  void next();
  void parse(std::unique_ptr<PTNode> &head);
  PTNode *parse_stmt();
  PTNode *parse_stmts();
  void shunting_yard();

private:
  std::unique_ptr<TokenChunk[]> token_stream;
  PTNode *parse_expr();
  PTNode *parse_if_stmt();
  PTNode *parse_while_stmt();
  PTNode *parse_var_decl();
  PTNode *parse_fn_decl();
  PTNode *parse_assignment();
  int _ptr = 0;
  PTNode *head = nullptr;
  // make a list of const nodes that can be used to initialize to when parsing,
  // instead of making new parserChunks
  StmtPTCs ptcs;
};

#endif // !PARSER_H

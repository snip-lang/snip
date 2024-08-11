#include "globals.h"
#include <cstddef>
#include <memory>

#ifndef PARSER_H
#define PARSER_H

// struct to hold the const structs used during parsing
struct StmtPTCs {
  ParserTokenChunk if_stmt = {ParserToken::IFSTMT, ""};
  ParserTokenChunk while_stmt = {ParserToken::WHILESTMT, ""};
  ParserTokenChunk fn_decl = {ParserToken::FNDECL, ""};
};

class PTNode {
public:
  PTNode(ParserTokenChunk &);
  ~PTNode();
  void add_child(ParserTokenChunk &);
  void add_child(PTNode *);
  void add_sibling(ParserTokenChunk &);
  void add_sibling(PTNode *);
  void print(const int);

private:
  ParserTokenChunk *val = nullptr;
  PTNode *first_child = nullptr;
  PTNode *last_child = nullptr;
  PTNode *next_sibling = nullptr;
  PTNode *prev_sibling = nullptr;
};

class ExprNode : public PTNode {};
class IntegerNode : public PTNode {};
class DoubleNode : public PTNode {};

class Parser {
public:
  Parser(std::unique_ptr<TokenChunk[]>);
  Parser();
  ~Parser();
  TokenChunk peek() const noexcept;
  TokenChunk get() const noexcept;
  void next();
  void parse(std::unique_ptr<PTNode> &head);
  void parse_stmt();
  void add_node();
  void shunting_yard();

private:
  std::unique_ptr<TokenChunk[]> token_stream;
  PTNode *parse_expr();
  int ptr = 0;
  PTNode *head = nullptr;
  // make a list of const nodes that can be used to initialize to when parsing,
  // instead of making new parserChunks
  StmtPTCs ptcs;
};

#endif // !PARSER_H

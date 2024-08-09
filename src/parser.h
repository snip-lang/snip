#include "globals.h"
#include <memory>

#ifndef PARSER_H
#define PARSER_H

class PTNode {
public:
  PTNode(ParserTokenChunk &);
  ~PTNode();
  void add_child(ParserTokenChunk &);
  void add_sibling(ParserTokenChunk &);
  void print(const int);

private:
  ParserTokenChunk *val = nullptr;
  PTNode *first_child = nullptr;
  PTNode *last_child = nullptr;
  PTNode *next_sibling = nullptr;
  PTNode *prev_sibling = nullptr;
};

class PT {
public:
  PT();
  ~PT();

private:
  PTNode *root = nullptr;
  PTNode *children = nullptr;
};

class Parser {
public:
  Parser(std::unique_ptr<TokenChunk[]>);
  Parser();
  ~Parser();
  TokenChunk peek() const noexcept;
  TokenChunk get() const noexcept;
  void next();
  void parse();

private:
  std::unique_ptr<TokenChunk[]> token_stream;
  int ptr = 0;
};

#endif // !PARSER_H

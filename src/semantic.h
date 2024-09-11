#ifndef SEMANTIC_ANALYZER_H
#define SEMANTIC_ANALYZER_H

#include "ast.h"
#include "globals.h"
#include "parser.h"
#include <unordered_map>
#include <vector>

class SemanticAnalyzer {
public:
  SemanticAnalyzer(std::unique_ptr<PTNode> &root);

private:
  ASTNode *root;
};

class SymbolTable {
public:
  SymbolTable();
  ParserTokenChunk *get_tok() const;
  void insert_tok(ParserTokenChunk);

private:
  // std::unordered_map<std::string, ParserTokenChunk> table;
};

#endif

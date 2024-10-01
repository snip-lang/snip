#ifndef SEMANTIC_ANALYZER_H
#define SEMANTIC_ANALYZER_H

#include "ast.h"
#include "globals.h"
#include "parser.h"
#include <memory>
#include <unordered_map>
#include <variant>
#include <vector>

class SemanticAnalyzer {
public:
  SemanticAnalyzer(std::unique_ptr<PTNode> &root);

private:
  ASTNode *root;
};

struct SymbolTableEntry {
  ParserToken type;
  std::variant<char, int, std::string> ident_value;
};

class SymbolTable {
public:
  SymbolTable();
  ParserTokenChunk *get_tok() const;
  int insert_tok(ParserTokenChunk *, PTNode *);
  int insert_tok(PTNode *, PTNode *);

private:
  std::unordered_map<std::string, SymbolTableEntry> table;
  // how to  i in sert an element into this the table?
  int scope{0};
};

class SymbolTableS {
public:
  SymbolTableS();
  ~SymbolTableS();
  int insert(PTNode *, PTNode *);
  void enter_scope();
  void exit_scope();
  int get_scope() const;
  SymbolTable *get_top_table();
  SymbolTable *get_current_scope();

private:
  std::vector<SymbolTable *> tables;
  int scope = 0;
};

#endif

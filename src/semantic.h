#ifndef SEMANTIC_ANALYZER_H
#define SEMANTIC_ANALYZER_H

#include "globals.h"
#include "parser.h"
#include <memory>
#include <unordered_map>
#include <variant>
#include <vector>

class SemanticAnalyzer {
public:
  SemanticAnalyzer(std::unique_ptr<PTNode> &root);
  void analyze();
};

struct SymbolTableEntry {
  ParserToken type;
	std::variant<int, std::string, double, bool, char> ident_value;
};

class SymbolTable {
public:
  SymbolTable();
  int insert_tok(ParserTokenChunk *, PTNode *);
  ParserTokenChunk *get_tok(const std::string&) ;
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
  ParserTokenChunk *get_tok(std::string) ;
  SymbolTable *get_top_table();
  SymbolTable *get_current_scope();

private:
  std::vector<SymbolTable *> tables;
  int scope = 0;
};

#endif

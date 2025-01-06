#include "./semantic.h"
#include "ast.h"
#include "globals.h"
#include "helper.h"
#include "parser.h"
#include <iostream>
#include <memory>
#include <stdexcept>
#include <unordered_map>
#include <utility>
#include <variant>

extern bool is_type(const Token tok);
extern Token parser_token_to_token(const ParserToken &pt);
extern void get_variant_value_and_assign_to(ParserTokenChunk &,
                                            SymbolTableEntryValue &);
extern void get_variant_value_and_assign_to(SymbolTableEntryValue &,
                                            SymbolTableEntryValue &);

SemanticAnalyzer::SemanticAnalyzer(std::unique_ptr<PTNode> &root) {
  if (root == nullptr) {
    throw std::runtime_error("no tokens to analyze");
  }

  PTNode *root_node = root.get();
  if (root_node->get_type() != "START") {
    throw std::runtime_error("root node of parsed tokens is not of type START");
  }
  PTNode *child = root_node->get_first_child();
  while (child and child->get_type() == "STMT") {
    auto child_node = child->get_first_child();
    if (child_node->get_type() == "IFSTMT") {
    } else if (child_node->get_type() == "WHILESTMT") {
    } else if (child_node->get_type() == "VARDECL") {
    } else if (child_node->get_type() == "FNDECL") {
    } else {
      throw std::runtime_error("unexpected token type");
    }
    child = child->get_next_sibling();
  }
}

/**
 * Walk through the parsed_tokens, then build the AST
 */
void SemanticAnalyzer::analyze() {}

ExprNode *parse_expr(PTNode *expr_node) {
  ExprNode *ast_expr_node = nullptr;

  return ast_expr_node;
}

SymbolTable::SymbolTable() { this->scope = 0; }

int SymbolTableS::get_scope() const { return this->scope; }

SymbolTableS::~SymbolTableS() {
  for (auto it = this->tables.begin(); it != this->tables.end(); ++it) {
    delete *it;
  }
}

SymbolTable *SymbolTableS::get_current_scope() {
  return this->tables.at(this->tables.size() - 1);
}

void SymbolTableS::enter_scope() {
  this->scope++;
  SymbolTable *table = new SymbolTable();
  this->tables.push_back(table);
}

void SymbolTableS::exit_scope() {
  if (this->tables.size() == 0) {
    throw std::runtime_error(
        "semantic: exit_scope() called when no scopes present");
  }
  SymbolTable *table = this->tables.at(tables.size() - 1);
  delete table;
  this->tables.pop_back();
  this->scope--;
}

SymbolTable *SymbolTableS::get_top_table() {
  return this->tables.at(this->tables.size() - 1);
}

/**
 * Insert a parserToken into the topmost symtable
 *
 * @param node
 * @return int
 */
int SymbolTableS::insert(PTNode *node, PTNode *type) {
  SymbolTable *top_table = this->get_top_table();
  top_table->insert_tok(node, type);
  return 0;
}

/** aux method to insert a identifier into the sym_table
 *
 * @param node ParserTokenChunk*
 */
int SymbolTable::insert_tok(ParserTokenChunk *tok, PTNode *ident_type) {
  std::string ident_name = std::get<std::string>(tok->value);
  SymbolTableEntry entry = {ParserToken(), '\0'};
  switch (ident_type->get_val()->type) {
  case ParserToken::CHAR:
  case ParserToken::INT:
    entry = {ident_type->get_val()->type, '\0'};
    break;
  case ParserToken::STRING:
    entry = {ident_type->get_val()->type, ""};
    break;
  case ParserToken::BOOL:
    entry = {ident_type->get_val()->type, 0};
    break;
  }
  this->table.insert(std::make_pair(ident_name, entry));
  return 0;
}

/** get a identifier from the sym_table
 *
 * @param ident_name std::string
 * @return ParserTokenChunk*
 */
ParserTokenChunk *SymbolTable::get_tok(const std::string &ident_name) {
  auto entry_it = table.find(ident_name);
  if (entry_it == this->table.end()) {
    return nullptr;
  }
  ParserTokenChunk *chunk = new ParserTokenChunk;
  chunk->type = entry_it->second.type;
  get_variant_value_and_assign_to(entry_it->second.ident_value, chunk->value);
  return chunk;
}

ParserTokenChunk *SymbolTableS::get_tok(std::string ident_name) {
  ParserTokenChunk *ident = nullptr;
  for (SymbolTable *table : this->tables) {
    ParserTokenChunk *ident = table->get_tok(ident_name);
    if (ident != nullptr) {
      break;
    }
  }
  return ident;
}

/** insert a identifier into the sym_table
 *
 * @param node PTNode*
 */
int SymbolTable::insert_tok(PTNode *node, PTNode *ident_type) {
  ParserTokenChunk *ident = (node->get_val().get());
  this->insert_tok(ident, ident_type);
  return 0;
}

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

typedef std::variant<char, int, std::string> ident_value;

extern bool is_type(const Token tok);
extern Token parser_token_to_token(const ParserToken &pt);

SemanticAnalyzer::SemanticAnalyzer(std::unique_ptr<PTNode> &root) {
  if (root == nullptr) {
    throw std::runtime_error("tokens not receieved in semantic analyzer");
  }
  PTNode *root_node = root.get();

  if (root_node->get_type() != "START") {
    throw std::runtime_error("root node of parsed tokens is not of type START");
  }
  PTNode *child = root_node->get_first_child();
  while (child != nullptr) {
    if (child->get_type() == "STMT") {
      PTNode *stmt_type = child->get_first_child();
      if (stmt_type->get_type() == "VARDECL") {
        VarDeclAST *var_decl = new VarDeclAST();
        PTNode *type_ast_node = stmt_type->get_first_child();
        // DEBUG:
        std::cout << "type: "
                  << token_to_string(type_ast_node->get_val().get()->type)
                  << std::endl;
        PTNode *ident_ast_node = type_ast_node->get_next_sibling();
        std::cout << "ident: "
                  << token_to_string(ident_ast_node->get_val().get()->type)
                  << std::endl;
        PTNode *expr_parsed_node =
            ident_ast_node->get_next_sibling()->get_next_sibling();
        // for each expr_node
        while (expr_parsed_node != nullptr) {
          ParserTokenChunk *expr = expr_parsed_node->get_val().get();

          std::cout << "expr: "
                    << token_to_string(expr_parsed_node->get_val().get()->type)
                    << std::endl;
          expr_parsed_node = expr_parsed_node->get_next_sibling();
        }
        std::cout << "\n";
      }
    } else {
      throw std::runtime_error(
          "(semantic): unexpected node type in top level of parsed tokens");
    }
    child = child->get_next_sibling();
  }
}

// START
//   STMT
//     VARDECL
//       INTK
//       IDENTIFIER
//       ASSIGN
//       EXPR
//         EXPR
//           EXPR
//             IDENTIFIER
//             IDENTIFIER
//             SUBTRACT -
//           LEFTPARENTHESIS
//           RIGHTPARENTHESIS
//         EXPR
//           IDENTIFIER
//           EXPR
//             IDENTIFIER
//             EXPR
//             MULTIPLY *
//           ADD +
//       SEMICOLON
// END

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

/* aux method to insert a identifier into the sym_table
 *
 * @param node ParserTokenChunk*
 */
int SymbolTable::insert_tok(ParserTokenChunk *tok, PTNode *ident_type) {
  std::string ident_name = std::get<std::string>(tok->value);
  if (ident_type->get_val()->type == ParserToken::CHAR) {
    SymbolTableEntry entry = {ParserToken::CHAR, '\0'};
    this->table.insert(std::make_pair(ident_name, entry));
  } else if (ident_type->get_val()->type == ParserToken::INT) {
    SymbolTableEntry entry = {ParserToken::INT, 0};
    this->table.insert(std::make_pair(ident_name, entry));
  } else if (ident_type->get_val()->type == ParserToken::STRING) {
    SymbolTableEntry entry = {ParserToken::STRING, ""};
    this->table.insert(std::make_pair(ident_name, entry));
  } else if (ident_type->get_val()->type == ParserToken::BOOL) {
    SymbolTableEntry entry = {ParserToken::STRING, 0};
    this->table.insert(std::make_pair(ident_name, entry));
  }
  return 0;
}

/* insert a identifier into the sym_table
 *
 * @param node PTNode*
 */
int SymbolTable::insert_tok(PTNode *node, PTNode *ident_type) {
  ParserTokenChunk *ident = (node->get_val().get());
  this->insert_tok(ident, ident_type);
  return 0;
}

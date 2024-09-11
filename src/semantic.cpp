#include "./semantic.h"
#include "ast.h"
#include "globals.h"
#include "parser.h"
#include <memory>
#include <stdexcept>
#include <unordered_map>

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
        // VarDeclAST * var_decl = new VarDeclAST();
        PTNode *decl_child = stmt_type->get_first_child();
     // TODO: 
      }
    } else {
      throw std::runtime_error(
          "(semantic): unexpected node type in top level of parsed tokens");
    }
    child = child->get_next_sibling();
  }
}

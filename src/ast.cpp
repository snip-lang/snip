#include "ast.h"
#include <cassert>

extern bool is_type(const Token tok);
extern Token parser_token_to_token(ParserToken& tok);

ASTNode::ASTNode() {}
ASTNode::ASTNode(ParserTokenChunk& tok) {
  this->children = std::vector<ASTNode>();
  this->type = tok.type;
  this->value = tok.value;
}

void ASTNode::set_tok(ParserTokenChunk& tok) {
  this->type = tok.type;
  this->value = tok.value;
}

ParserTokenChunk ASTNode::get_tok() const {
  return ParserTokenChunk({ this->type, this->value });
}

std::vector<ASTNode> ASTNode::get_children() const {
  return this->children;
}

void ASTNode::add_child(ASTNode child) {
  this->children.push_back(child);
}

void ASTNode::set_type(ParserToken& type) {
  this->type = type;
}
ParserToken ASTNode::get_type() const {
  return this->type;
}
void ASTNode::set_value(std::variant<int, std::string, double>value) {
  this->value = value;
}
std::variant<int, std::string, double> ASTNode::get_value() const {
  return this->value;
}

VarDeclAST::VarDeclAST(){}

void VarDeclAST::set_ident_name(std::unique_ptr<ParserTokenChunk>& name) {
  assert(name.get()->type == ParserToken::IDENTIFIER);
  this->ident_name = std::get<std::string>(name.get()->value);
}

void VarDeclAST::set_type(std::unique_ptr<ParserTokenChunk>& name) {
  assert(is_type(parser_token_to_token(name.get()->type)));
  this->type = std::make_unique<Token>(parser_token_to_token( name.get()->type ));
}

#include "parser.h"
#include "globals.h"
#include "helper.h"
#include <iostream>
#include <memory>
#include <type_traits>

template <typename T, typename U> bool tokencmp(T t1, U t2) {
  static_assert(std::is_enum<T>::value && std::is_enum<U>::value,
                "tokencmp requires enum types");
  return static_cast<int>(t1) == static_cast<int>(t2);
}

Parser::Parser(std::unique_ptr<TokenChunk[]> token_s) {
  token_stream = std::move(token_s);
}

void PTNode::print(const int spaces) {
  for (int i = 0; i < spaces; i++) {
    std::cout << " ";
  }
  std::cout << token_to_string(this->val->type) << std::endl;
  if (this->first_child) {
    this->first_child->print(spaces + 2);
  }
  if (this->next_sibling) {
    this->next_sibling->print(spaces);
  }
}

void PTNode::add_child(ParserTokenChunk &tok) {
  if (this->last_child != nullptr) {
    this->last_child->add_sibling(tok);
    this->last_child = this->last_child->next_sibling;
  } else {
    this->first_child = new PTNode(tok);
    this->first_child->val->value = tok.value;
    this->first_child->val->type = tok.type;
    this->last_child = this->first_child;
  }
}

void PTNode::add_sibling(ParserTokenChunk &tok) {
  if (this->next_sibling != nullptr) {
    this->next_sibling->add_sibling(tok);
  } else {
    this->next_sibling = new PTNode(tok);
    ;
    this->next_sibling->prev_sibling = this;
  }
}

PTNode::~PTNode() { // pointless
  delete first_child;
  delete next_sibling;
}

PTNode::PTNode(ParserTokenChunk &tok) { this->val = &tok; }

void Parser::next() { this->ptr++; }

TokenChunk Parser::peek() const noexcept { return this->token_stream[ptr + 1]; }

TokenChunk Parser::get() const noexcept { return this->token_stream[ptr]; }

void Parser::parse() {
  if (this->token_stream[ptr].type == Token::START) {
    this->next();
    while (this->token_stream[ptr].type != Token::END) {
      if (tokencmp(this->get().type, Token::IF)) {
      }
    }
  }
}

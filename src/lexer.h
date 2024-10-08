#include "globals.h"
#include <cstddef>
#include <memory>
#include <string>
#include <vector>

#ifndef LEXER

struct token_node {
  TokenChunk tok;
  token_node *next = nullptr;
};

class Lexer {
public:
  Lexer() = default;
  Lexer(std::string);
  void read_next() noexcept;
  void read_next(int) noexcept;
  TokenChunk get_token();
  void tokenize(std::unique_ptr<TokenChunk[]> &token_stack);
  void process_literal_token(TokenChunk &, token_node *&);
  void process_token(token_node *&);

private:
  std::string input = "";
  int ptr{0};
  int next_ptr{1};
  int current_token_ptr{0};
  char current_char{0};
  int line_number{1};
  int len{0};
  int num_tokens{0};
  std::string current_token;
};

#endif

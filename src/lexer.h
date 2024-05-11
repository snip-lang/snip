#include <cstddef>
#include <memory>
#include <string>
#include <vector>
#include "globals.h"

#ifndef LEXER

// TODO: 
class Lexer {
public:
  Lexer() = default;
  Lexer(std::string);
  Token get_token();
  void tokenize( std::unique_ptr<Token[]>& token_stack);

private:
  std::string input;
  int ptr{0};
  int next_ptr{1}; 
  int current_token_ptr{0};
  char current_char{0} ;
  int line_number{1}; 
  int len{0};
  int num_tokens{0};
  std::string current_token; 
};

#endif

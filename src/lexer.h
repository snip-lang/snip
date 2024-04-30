#include <string>
#include <vector>
#ifndef LEXER 


enum class Token {
  IF,
  ELSE,
  ELIF,
  FUNCTION,
  CHARK, 
  INTK,
  STRINGK,
  BOOLK,
  WHILE,
  LITERAL,
  IDENTIFIER,
};



class Lexer {
public:
  Lexer() = default;
  Lexer(std::vector<std::string>); 
  Lexer(Lexer &&) = default;
  Lexer(const Lexer &) = default;
  Lexer &operator=(Lexer &&) = default;
  Lexer &operator=(const Lexer &) = default;
  ~Lexer() = default;

private:
  std::vector<Token> tokens; 
};

#endif 


#include <cstddef>
#include <memory>
#include <string>
#include <vector>
#ifndef LEXER

enum class Token {
  IF,
  ELSE,
  ELIF,
  INT, 
  DOUBLE,
  FUNCTION,
  STRING,
  CHARK,
  INTK,
  STRINGK,
  BOOLK,
  WHILE,
  RIGHTPARENTHESIS,
  LEFTPARENTHESIS,
  RIGHTBRACKET,
  LEFTBRACKET,
  RIGHTBRACE,
  LEFTBRACE,
  PERIOD,
  COMMA,
  EQUAL,
  NOTEQUAL,
  ASSIGN,
  GREATERTHAN,
  LESSERTHAN,
  LESSTHANEQUAL,
  GREATERTHANEQUAL,
  AND,
  OR,
  NOT,
  IDENTIFIER,
  INVALID, 
  NONE
};


// TODO 
union TokenValue {
  int num; 
  std::string val; 
  char literal; 
};
// TODO 
struct TokenChunk {
  Token tok; 
  TokenValue value;
};



class Lexer {
public:
  Lexer() = default;
  Lexer(std::string_view);
  Lexer(Lexer &&) = default;
  Lexer &operator=(Lexer &&) = default;
  ~Lexer() = default;
  void read_next();
  Token get_token();
  void tokenize();

private:
  std::string_view input;
  int ptr{0};
  int next_ptr{1}; 
  char cur_char{0} ;
  int line_number{0}; // to be used in the future 
  int len{0};
  int num_tokens{0};
  std::string char_stack; 
  std::unique_ptr<Token[]> token_stack;
};

#endif

#include <cstddef>
#include <memory>
#include <string>
#include <vector>
#ifndef LEXER

enum class Token {
  UNDEFINED,
  SEMICOLON,
  IF,
  ELSE,
  ELIF,
  INT, 
  DOUBLE,
  DOUBLEK,
  FUNCTION,
  STRING,
  CHARK,
  CHAR,
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
  NONE,
  MULTIPLY, 
  ADD, 
  SUBTRACT, 
  DIVIDE, 
  END, 
};


// TODO: 
union TokenValue {
  int num; 
  std::string val; 
  char literal; 
};
// TODO:
struct TokenChunk {
  Token tok; 
  TokenValue value;
};



class Lexer {
public:
  Lexer() = default;
  Lexer(std::string);
  Lexer(Lexer &&) = default;
  void read_next() noexcept;
  void read_next(int) noexcept;
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

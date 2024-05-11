#include <string>
enum class Token {
  UNDEFINED,
  SEMICOLON,
  IF,
  ELSE,
  ELIF,
  INT, 
  DOUBLE,
  DOUBLEK,
  TRUEK,
  FALSEK, 
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




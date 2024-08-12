#ifndef GLOBAL_H
#define GLOBAL_H

#include <string>
#include <variant>

enum class Token {
  UNDEFINED = 0,
  SEMICOLON = 1,
  IF = 2,
  ELSE = 3,
  ELIF = 4,
  INT = 5,
  DOUBLE = 6,
  DOUBLEK = 7,
  TRUEK = 8,
  FALSEK = 9,
  FUNCTION = 10,
  STRING = 11,
  CHARK = 12,
  CHAR = 13,
  INTK = 14,
  STRINGK = 15,
  BOOLK = 16,
  WHILE = 17,
  RIGHTPARENTHESIS = 18,
  LEFTPARENTHESIS = 19,
  RIGHTBRACKET = 20,
  LEFTBRACKET = 21,
  RIGHTBRACE = 22,
  LEFTBRACE = 23,
  PERIOD = 24,
  COMMA = 25,
  EQUAL = 26,
  NOTEQUAL = 27,
  ASSIGN = 28,
  GREATERTHAN = 29,
  LESSERTHAN = 30,
  LESSTHANEQUAL = 31,
  GREATERTHANEQUAL = 32,
  AND = 33,
  OR = 34,
  NOT = 35,
  IDENTIFIER = 36,
  INVALID = 37,
  NONE = 38,
  MULTIPLY = 39,
  DIVIDE = 40,
  ADD = 41,
  SUBTRACT = 42,
  START = 43,
  END = 44,
};

enum OperatorPrecedence {
  ASSIGN,
  OR,
  AND,
  EQUALITY,
  COMPARISON,
  ADDSUB,
  MULTIDIV,
  EXPONENT,
  NOT,
  PAREN,
  NONE,
};

enum class ParserToken {
  UNDEFINED = 0,
  SEMICOLON = 1,
  IF = 2,
  ELSE = 3,
  ELIF = 4,
  INT = 5,
  DOUBLE = 6,
  DOUBLEK = 7,
  TRUEK = 8,
  FALSEK = 9,
  FUNCTION = 10,
  STRING = 11,
  CHARK = 12,
  CHAR = 13,
  INTK = 14,
  STRINGK = 15,
  BOOLK = 16,
  WHILE = 17,
  RIGHTPARENTHESIS = 18,
  LEFTPARENTHESIS = 19,
  RIGHTBRACKET = 20,
  LEFTBRACKET = 21,
  RIGHTBRACE = 22,
  LEFTBRACE = 23,
  PERIOD = 24,
  COMMA = 25,
  EQUAL = 26,
  NOTEQUAL = 27,
  ASSIGN = 28,
  GREATERTHAN = 29,
  LESSERTHAN = 30,
  LESSTHANEQUAL = 31,
  GREATERTHANEQUAL = 32,
  AND = 33,
  OR = 34,
  NOT = 35,
  IDENTIFIER = 36,
  INVALID = 37,
  NONE = 38,
  MULTIPLY = 39,
  DIVIDE = 40,
  ADD = 41,
  SUBTRACT = 42,
  START = 43,
  END = 44,
  STMT = 45,
  EXPR = 46,
  BINOP = 47,
  FNDECL = 48,
  IFSTMT = 49,
  WHILESTMT = 50,
  VARDECL = 51,
  DIGIT = 52,
  TYPE = 53,
};

union TokenValue {
  int num;
  std::string val;
  char literal;
};

struct TokenChunk {
  Token type = Token::UNDEFINED;
  std::variant<int, std::string, double> value;
};

struct ParserTokenChunk {
  ParserToken type = ParserToken::UNDEFINED;
  std::variant<int, std::string, double> value;
};

#endif // !GLOBAL_H

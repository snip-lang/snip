#include "lexer.h"
#include <fstream>
#include <iostream>
#include <memory>
#include <string>
#include <string_view>
#include <vector>

std::string token_to_string(Token tok) {
  switch (tok) {
  case Token::UNDEFINED:
    return "UNDEFINED";
  case Token::IF:
    return "IF";
  case Token::ELSE:
    return "ELSE";
  case Token::ELIF:
    return "ELIF";
  case Token::INT:
    return "INT";
  case Token::DOUBLE:
    return "DOUBLE";
  case Token::DOUBLEK:
    return "DOUBLEK";
  case Token::FUNCTION:
    return "FUNCTION";
  case Token::STRING:
    return "STRING";
  case Token::CHARK:
    return "CHARK";
  case Token::INTK:
    return "INTK";
  case Token::STRINGK:
    return "STRINGK";
  case Token::BOOLK:
    return "BOOLK";
  case Token::WHILE:
    return "WHILE";
  case Token::RIGHTPARENTHESIS:
    return "RIGHTPARENTHESIS";
  case Token::LEFTPARENTHESIS:
    return "LEFTPARENTHESIS";
  case Token::RIGHTBRACKET:
    return "RIGHTBRACKET";
  case Token::LEFTBRACKET:
    return "LEFTBRACKET";
  case Token::RIGHTBRACE:
    return "RIGHTBRACE";
  case Token::LEFTBRACE:
    return "LEFTBRACE";
  case Token::PERIOD:
    return "PERIOD";
  case Token::COMMA:
    return "COMMA";
  case Token::EQUAL:
    return "EQUAL";
  case Token::NOTEQUAL:
    return "NOTEQUAL";
    case Token::ASSIGN:
      return "ASSIGN"; 
    case Token::GREATERTHAN:
    return "GREATERTHAN";
  case Token::LESSERTHAN:
    return "LESSERTHAN";
  case Token::LESSTHANEQUAL:
    return "LESSTHANEQUAL";
  case Token::GREATERTHANEQUAL:
    return "GREATERTHANEQUAL";
  case Token::AND:
    return "AND";
  case Token::OR:
    return "OR";
  case Token::NOT:
    return "NOT";
  case Token::IDENTIFIER:
    return "IDENTIFIER";
  case Token::INVALID:
    return "INVALID";
  case Token::NONE:
    return "NONE";
  default:
    return "no token, somehow";
  }
}

std::string readFile(const std::string &filename) {
  std::ifstream file(filename);
  if (!file.is_open()) {
    std::cerr << "Unable to open source file" << std::endl;
  }
  std::string source((std::istreambuf_iterator<char>(file)),
                     std::istreambuf_iterator<char>());
  file.close();
  return source;
}

int main(int argc, char *argv[]) {
  std::string filename = "input.snip"; // default filename
  if (argc == 1) {
    filename = argv[0];
  }
  std::string lines = readFile(filename);
  Lexer lex(lines);
  std::unique_ptr<Token[]> token_stack = nullptr; 
  lex.tokenize(token_stack);
  int i{0};

  while (token_stack[i] != Token::END) {
    std::cout << token_to_string(token_stack[i]) << std::endl;
  }

  return 0;
}

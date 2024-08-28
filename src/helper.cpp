#include "globals.h"
#include "parser.h"
#include <cstring>
#include <fstream>
#include <iostream>
#include <memory>
#include <sstream>
#include <string>

const std::string token_to_string(const Token &tok) {
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
  case Token::FN:
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
  case Token::ADD:
    return "ADD";
  case Token::SUBTRACT:
    return "SUBTRACT";
  case Token::MULTIPLY:
    return "MULTIPLY";
  case Token::DIVIDE:
    return "DIVIDE";
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
  case Token::CHAR:
    return "CHAR";
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
  case Token::SEMICOLON:
    return "SEMICOLON";
  case Token::COLON:
    return "COLON";
  case Token::AND:
    return "AND";
  case Token::OR:
    return "OR";
  case Token::NOT:
    return "NOT";
  case Token::IDENTIFIER:
    return "IDENTIFIER";
  case Token::TRUEK:
    return "TRUE";
  case Token::FALSEK:
    return "FALSE";
  case Token::INVALID:
    return "INVALID";
  case Token::END:
    return "END";
  case Token::NONE:
    return "NONE";
  case Token::START:
    return "START";
  case Token::FACTOR:
    return "FACTOR";
  case Token::EXCLAIM:
    return "EXCLAIM";
  default:
    return "no token, somehow";
  }
}

const std::string token_to_string(const ParserToken &tok) {
  switch (tok) {
  case ParserToken::UNDEFINED:
    return "UNDEFINED";
  case ParserToken::FNCALL:
    return "FNCALL";
  case ParserToken::EXCLAIM:
    return "EXCLAIM";
  case ParserToken::IF:
    return "IF";
  case ParserToken::ELSE:
    return "ELSE";
  case ParserToken::ELIF:
    return "ELIF";
  case ParserToken::INT:
    return "INT";
  case ParserToken::FACTOR:
    return "FACTOR";
  case ParserToken::DOUBLE:
    return "DOUBLE";
  case ParserToken::DOUBLEK:
    return "DOUBLEK";
  case ParserToken::FN:
    return "FUNCTION";
  case ParserToken::STRING:
    return "STRING";
  case ParserToken::CHARK:
    return "CHARK";
  case ParserToken::INTK:
    return "INTK";
  case ParserToken::STRINGK:
    return "STRINGK";
  case ParserToken::BOOLK:
    return "BOOLK";
  case ParserToken::WHILE:
    return "WHILE";
  case ParserToken::RIGHTPARENTHESIS:
    return "RIGHTPARENTHESIS";
  case ParserToken::LEFTPARENTHESIS:
    return "LEFTPARENTHESIS";
  case ParserToken::RIGHTBRACKET:
    return "RIGHTBRACKET";
  case ParserToken::LEFTBRACKET:
    return "LEFTBRACKET";
  case ParserToken::RIGHTBRACE:
    return "RIGHTBRACE";
  case ParserToken::CHAR:
    return "CHAR";
  case ParserToken::LEFTBRACE:
    return "LEFTBRACE";
  case ParserToken::PERIOD:
    return "PERIOD";
  case ParserToken::COMMA:
    return "COMMA";
  case ParserToken::EQUAL:
    return "EQUAL";
  case ParserToken::NOTEQUAL:
    return "NOTEQUAL";
  case ParserToken::ASSIGN:
    return "ASSIGN";
  case ParserToken::GREATERTHAN:
    return "GREATERTHAN";
  case ParserToken::LESSERTHAN:
    return "LESSERTHAN";
  case ParserToken::LESSTHANEQUAL:
    return "LESSTHANEQUAL";
  case ParserToken::GREATERTHANEQUAL:
    return "GREATERTHANEQUAL";
  case ParserToken::SEMICOLON:
    return "SEMICOLON";
  case ParserToken::COLON:
    return "COLON";
  case ParserToken::AND:
    return "AND";
  case ParserToken::OR:
    return "OR";
  case ParserToken::NOT:
    return "NOT";
  case ParserToken::IDENTIFIER:
    return "IDENTIFIER";
  case ParserToken::TRUEK:
    return "TRUE";
  case ParserToken::FALSEK:
    return "FALSE";
  case ParserToken::INVALID:
    return "INVALID";
  case ParserToken::END:
    exit(0);
  case ParserToken::NONE:
    return "NONE";
  case ParserToken::START:
    return "START";
  case ParserToken::IFSTMT:
    return "IFSTMT";
  case ParserToken::STMT:
    return "STMT";
  case ParserToken::ASSIGNSTMT:
    return "ASSIGNSTMT";
  case ParserToken::STMTS:
    return "STMTS";
  case ParserToken::EXPR:
    return "EXPR";
  case ParserToken::ADD:
    return "ADD";
  case ParserToken::SUBTRACT:
    return "SUBTRACT";
  case ParserToken::MULTIPLY:
    return "MULTIPLY";
  case ParserToken::DIVIDE:
    return "DIVIDE";
  case ParserToken::WHILESTMT:
    return "WHILESTMT";
  case ParserToken::DIGIT:
    return "DIGIT";
  case ParserToken::VARDECL:
    return "VARDECL";
  case ParserToken::TYPE:
    return "TYPE";
  case ParserToken::FNDECL:
    return "FNDECL";
  case ParserToken::BINOP:
    return "BINOP";
  case ParserToken::FORMAL:
    return "FORMAL";
  case ParserToken::VARIABLE:
    return "VARIABLE";
  default:
    return "no token, somehow";
  }
}

void print_lexed_tokens(std::unique_ptr<TokenChunk[]> &token_stack) {
  int i{0};
  while (token_stack[i].type != Token::END) {
    std::string s = token_to_string(token_stack[i].type);
    if (token_stack[i].type == Token::DOUBLE) {
      std::cout << token_to_string(token_stack[i].type) << " "
                << std::get<std::string>(token_stack[i].value) << std::endl;
    } else if (token_stack[i].type == Token::INT) {
      std::cout << token_to_string(token_stack[i].type) << " "
                << std::get<int>(token_stack[i].value) << std::endl;
    } else
      std::cout << token_to_string(token_stack[i].type) << " "
                << std::get<std::string>(token_stack[i].value) << std::endl;
    i++;
  }
  std::cout << token_to_string(token_stack[i].type) << std::endl;
}

void print_parsed_tokens(std::unique_ptr<PTNode> &token_stack) {
  token_stack->print(0);
}

std::string
print_lexed_tokens_test(std::unique_ptr<TokenChunk[]> &token_stack) {
  std::stringstream ss;
  int i{0};
  while (token_stack[i].type != Token::END) {
    std::string s = token_to_string(token_stack[i].type);
    ss << token_to_string(token_stack[i].type) << std::endl;
    i++;
  }
  return ss.str();
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

void writeFile(const std::string &filename, const std::string &output) {
  std::ofstream file(filename);
  if (!file.is_open()) {
    std::cerr << "Unable to open output file" << std::endl;
  }
  file << output;
  file.close();
}

bool get_flags(const int &argc, char *argv[], std::string &filename,
               std::string &parseString, std::string &test_string) {
  if (argc == 1)
    return 0;
  if (argc == 2) {
    filename = argv[1];
    return 0;
  } else if (argc == 3) {
    if (std::strcmp(argv[1], "-e")) {
      parseString = argv[2];
    }
  }
  char *currentFlag;
  bool isTest{false};
  for (int count{0}; count < argc; count++) {
    if (count % 2) {
      currentFlag = argv[count];
    } else {
      if (strcmp(currentFlag, "-e") == 0) {
        parseString = argv[count];
      } else if (strcmp(currentFlag, "-t") == 0) {
        isTest = true;
        test_string = argv[count];
      } else {
        filename = argv[count];
      }
    }
  }
  return isTest;
}

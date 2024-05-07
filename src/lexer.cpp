#include <cctype>
#include <cstdlib>
#include <iostream>
#include <string>
#include <string_view>
#include <vector>

#include "./lexer.h"

enum class State : short {
  START,
  IF_1,
  IF_2,
  ELSE_1,
  ELSE_2,
  ELSE_3,
  ELSE_4,
  ELIF_3,
  ELIF_4,
  INT_1,
  INT_2,
  INT_3,
  DOUBLE_1,
  DOUBLE_2,
  DOUBLE_3,
  DOUBLE_4,
  DOUBLE_5,
  BOOL_1,
  BOOL_2,
  BOOL_3,
  BOOL_4,
  WHILE_1,
  WHILE_2,
  WHILE_3,
  WHILE_4,
  WHILE_5,
  FUNCTION_1,
  FUNCTION_2,
  STRING_1,
  STRING_2,
  STRING_3,
  CHAR_1,
  CHAR_2,
  CHAR_3,
  CHAR_4,
  AND_1,
  AND_2,
  AND_3,
  OR_1,
  OR_2,
  GE_1,
  GE_2,
  GT_1,
  LE_1,
  LE_2,
  LT_1,
  EQUAL_1,
  EQUAL_2,
  NOTEQUAL_1,
  NOTEQUAL_2,
  ASSIGN_1,
  NOT_1,
  NOT_2,
  NOT_3,
  LEFTBRACE_1,
  RIGHTBRACE_1,
  LEFTPARANTHESIS_1,
  RIGHTPARANTHESIS_1,
  PERIOD_1,
  COMMA_1,
};

bool isSpace(char c) noexcept {
  switch (c) {
  case ' ':
  case '\t':
  case '\n':
    return true;
  default:
    return false;
    break;
  }
}

bool isComment(char c) noexcept { return (c == '#'); }

// make a linked list to store all the parsed tokens the first time to avoid
// reallocating for each insertion
struct token_node {
  Token tok;
  std::string value;
  token_node *next;
};

// NOTE: the tokens are inserted in reverse order
void insert_into_linked_list(token_node *&head, Token tok, std::string value) {
  token_node *new_node = new token_node();
  new_node->value = value;
  new_node->tok = tok;
  token_node *temp = head;
  new_node->next = temp;
  head = new_node;
}

// take in a string_view and use that to lex the file
Lexer::Lexer(std::string_view input) {
  if (input.empty()) {
    std::exit(0);
  } else {
    this->input = input;
    this->len = input.length();
    this->line_number = 0;
    // reserve the char stack in advance
    this->char_stack.reserve(1000);
  }
}

void Lexer::read_next() {
  // to be used with finite automaton that quits on space
  if (this->ptr >= this->len) {
    this->char_stack[this->ptr] = '\0';
    get_token();
  }
  this->cur_char = this->input[this->ptr];

  if (this->cur_char == ' ') {
    // check if there is a token on the char stack, then make a new token
    // NOTE: this condition requires that the first element is reset to ' '
    // when a new token is added
    if (this->char_stack[0] != ' ') {
      this->char_stack[this->ptr] = this->cur_char;
      this->ptr++;
    }
  } else {
    // get the token type value on the char_stack
    this->char_stack[this->ptr] = '\0';
    get_token();
  }
}

Token Lexer::get_token() {
  // only called when char_stack has a length of atleast one
  int i = 0;
  int char_stack_len = this->char_stack.length();
  while (i < char_stack_len) {
    while (std::isdigit(this->char_stack[i])) {
      i++;
    }
    if (this->char_stack[i] == '\0') {
      return Token::INT;
    } else if (this->char_stack[i] == '.') {
      while (std::isdigit(this->char_stack[i])) {
        i++;
      }
      if (this->char_stack[i] == '\0') {
        return Token::DOUBLE;
      }
    }
    if (i != 0) {
      // neither float nor string, but starts with a digit
      return Token::INVALID;
    }
    Token retToken;
    State currentState;
    switch (this->char_stack[i]) {
    default:

      break;
    }
  }

  void lexan() {
    int ind = 0;
    std::string line;
    std::string currentToken;
    Token retToken;
    while (1) {
      if (isSpace(line[ind]) || isComment(line[ind])) {
        // check if we need to register a token since a whitespace or a comment
        // has been encountered
        if (currentToken != "") {
          // check for different "regexes", then push them into the token list
          if (currentToken == "if") {
            retToken = Token::IF;
          } else if (currentToken == "else") {
            retToken = Token::ELSE;
          } else if (currentToken == "elif") {
            retToken = Token::ELIF;
          } else if (currentToken == "fn") {
            retToken = Token::FUNCTION;
          } else if (currentToken == "char") {
            retToken = Token::CHARK;
          } else if (currentToken == "string") {
            retToken = Token::STRINGK;
          } else if (currentToken == "bool") {
            retToken = Token::BOOLK;
          } else if (currentToken == "while") {
            retToken = Token::WHILE;
          } else {
            // check for literal
            int length = currentToken.length();
            if (length == 1) {
              switch (currentToken[0]) {
              case '(':
                retToken = Token::RIGHTPARENTHESIS;
              case ')':
                retToken = Token::LEFTPARENTHESIS;
              case '[':
                retToken = Token::LEFTBRACKET;
              case ']':
                retToken = Token::RIGHTBRACKET;
              case '{':
                retToken = Token::LEFTBRACE;
              case '}':
                retToken = Token::RIGHTBRACE;
              case '.':
                retToken = Token::PERIOD;
              case ',':
                retToken = Token::COMMA;
              default:
                break;
              }
            }
            if (length >= 2) {
              // run loop to check for regexes
              // EQUALITY = r'\=='
              // ASSIGN = r'\='
              // INEQUALITY = r'\!='
              // LESSTHANEQ = r'<='
              // GREATERTHANEQ = r'>='
              // LESSTHAN = r'<'
              // GREATERTHAN = r">"
              // DOUBLE = r'[-|+]?[0-9]+\.(E[-|+]?[0-9]+)?'
              // INTEGER = r'[-|+]?[0-9]+'
            }
          }
        }
      }
    }
  }

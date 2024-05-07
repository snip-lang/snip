#include <cstdlib>
#include <iostream>
#include <string>
#include <string_view>
#include <vector>

#include "./lexer.h"

bool isAlpha(char c) noexcept {
  return ((c > 96 && c < 123) || (c > 64 && c < 90));
}

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

bool isNum(char c) noexcept { return (c > 47 && c < 58); }

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
  // to be used wit finite automaton that quits on space
  if (this->ptr >= this->len) {
    this->char_stack[this->ptr] = '\0';
    get_token();
  }
  this->cur_char = this->input[this->ptr];

  if (this->cur_char == ' ') {
    // check if there is a token on the char stack, then make a new token
    // NOTE: this condition requires that the first element is reset to ' '
    // when a new token is added
    if (char_stack[0] != ' ') {
      this->char_stack[ptr] = this->cur_char;
    }
  } else {
    // get the token type value on the char_stack
    get_token();
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

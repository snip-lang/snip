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
  BOOLK_1,
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
  STRINGK_1,
  STRING_2,
  STRING_3,
  CHAR_1,
  CHAR_2,
  CHAR_3,
  CHAR_4,
  AND_1,
  ANDK_1,
  ANDK_2,
  ANDK_3,
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
  NOTK_1,
  NOTK_2,
  NOTK_3,
  NOT_1,
  LEFTBRACE_1,
  RIGHTBRACE_1,
  LEFTPARANTHESIS_1,
  RIGHTPARANTHESIS_1,
  PERIOD_1,
  COMMA_1,
  IDENTIFIER,
  INVALID,
};

State get_initial_state(char c) {
  switch (c) {
  case '_':
  case '-':
    return State::IDENTIFIER;
    break;
  case 'i':
    return State::IF_1;
    break;
  case 'e':
    return State::ELSE_1;
    break;
  case 'w':
    return State::WHILE_1;
  case 's':
    return State::STRINGK_1;
  case 'b':
    return State::BOOLK_1;
  case 'f':
    return State::FUNCTION_1;
  case 'c':
    return State::CHAR_1;
  case '&':
    return State::AND_1;
  case '{':
    return State::LEFTBRACE_1;
    break;
  case '}':
    return State::RIGHTBRACE_1;
    break;
  case '(':
    return State::LEFTPARANTHESIS_1;
    break;
  case ')':
    return State::RIGHTPARANTHESIS_1;
    break;
  case '.':
    return State::PERIOD_1;
    break;
  case ',':
    return State::COMMA_1;
    break;
  case '=':
    return State::EQUAL_1;
  case '!':
    return State::NOT_1;
  default:
    if (isalpha(c)) return State::IDENTIFIER; 
    return State::INVALID;
  }
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
  //
  // i++;
  // while (i < char_stack_len &&
  //        (std::isalnum(this->char_stack[i]) || this->char_stack[i] == '-' ||
  //         this->char_stack[i] == '_')) {
  //   i++;
  // }
  // if (this->char_stack[i] == '\0') {
  //   return Token::IDENTIFIER;
  // } else {
  //   return Token::INVALID;
  // }
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
  }
}

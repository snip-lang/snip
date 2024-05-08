#include <cctype>
#include <cstdlib>
#include <filesystem>
#include <iostream>
#include <memory>
#include <string>
#include <string_view>
#include <vector>

#include "./lexer.h"

enum class State : short {
  START,
  END,
  IF_1,
  IF_2,
  ELSE_1,
  ELSE_2,
  ELSE_3,
  ELSE_4,
  ELIF_3,
  ELIF_4,
  INTK_1,
  INTK_2,
  INTK_3,
  DOUBLEK_1,
  DOUBLEK_2,
  DOUBLEK_3,
  DOUBLEK_4,
  DOUBLEK_5,
  DOUBLEK_6,
  BOOLK_1,
  BOOLK_2,
  BOOLK_3,
  BOOLK_4,
  WHILE_1,
  WHILE_2,
  WHILE_3,
  WHILE_4,
  WHILE_5,
  FUNCTION_1,
  FUNCTION_2,
  STRINGK_1,
  STRINGK_2,
  STRINGK_3,
  CHAR,
  CHARK_1,
  CHARK_2,
  CHARK_3,
  CHARK_4,
  AND_1,
  AND_2,
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
    return State::CHARK_1;
  case '&':
    return State::AND_1;
  case '=':
    return State::EQUAL_1;
  case '!':
    return State::NOT_1;
  default:
    if (isalpha(c))
      return State::IDENTIFIER;
    else
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
  token_node *next = nullptr;
};

// NOTE: the tokens are inserted in reverse order
int insert_into_linked_list(token_node *&head, Token tok, int num_tokens) {
  token_node *new_node = new token_node();
  new_node->tok = tok;
  token_node *temp = head;
  new_node->next = temp;
  head = new_node;
  return num_tokens++;
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

// calls read_next to update ptr values
void Lexer::tokenize(std::unique_ptr<Token[]> &token_stack) {
  token_node *head = nullptr;
  int i = 0;
  while (i < this->len) {
    if (this->next_ptr >= this->len) { // reached EOF
      this->char_stack[this->next_ptr] = '\0';
      const Token token = get_token();
      insert_into_linked_list(head, token, this->num_tokens);
      this->char_stack[0] = ' '; // reset
    } else {
      this->cur_char = this->input[this->ptr];
      if (this->cur_char == ' ' || this->cur_char == '\t' ||
          this->cur_char == '\n') {
        // when a whitespace is encountered, check if there is a token on the
        // char stack, then make a new token NOTE: this condition requires that
        // the first element is reset to ' ' when a new token is added
        if (this->char_stack[0] != ' ') {
          this->char_stack[this->next_ptr] = '\0';
          Token returnToken = get_token();
          insert_into_linked_list(head, returnToken, this->num_tokens);
          i++;
          this->read_next();
          this->char_stack[0] = ' '; // reset
        } else {
          i++; // consecutive whitespace, encountered, ignore whitespace
          this->read_next();
        }
      } else {
        // check if any literals are on the next element, and if so, get_token,
        // also insert the literal's token into the list
        // if the next element is not a literal, just keep going
        Token returnToken;
        switch (this->input[this->next_ptr]) {
        case '{':
          this->char_stack[this->next_ptr] = '\0';
          returnToken = get_token();
          this->char_stack[0] = ' '; // reset
          insert_into_linked_list(head, returnToken, this->num_tokens);
          returnToken = Token::LEFTBRACE;
          insert_into_linked_list(head, returnToken, this->num_tokens);
          i += 2;
          read_next();
          read_next();
          break;
        case '}':
          this->char_stack[this->next_ptr] = '\0';
          returnToken = get_token();
          this->char_stack[0] = ' '; // reset
          insert_into_linked_list(head, returnToken, this->num_tokens);
          returnToken = Token::RIGHTBRACE;
          insert_into_linked_list(head, returnToken, this->num_tokens);
          i += 2;
          read_next();
          read_next();
          break;
        case '(':
          this->char_stack[this->next_ptr] = '\0';
          returnToken = get_token();
          this->char_stack[0] = ' '; // reset
          insert_into_linked_list(head, returnToken, this->num_tokens);
          returnToken = Token::LEFTPARENTHESIS;
          insert_into_linked_list(head, returnToken, this->num_tokens);
          i += 2;
          read_next();
          read_next();
          break;
        case ')':
          this->char_stack[this->next_ptr] = '\0';
          returnToken = get_token();
          this->char_stack[0] = ' '; // reset
          insert_into_linked_list(head, returnToken, this->num_tokens);
          returnToken = Token::RIGHTPARENTHESIS;
          insert_into_linked_list(head, returnToken, this->num_tokens);
          i += 2;
          read_next();
          read_next();
          break;
        case '.':
          this->char_stack[this->next_ptr] = '\0';
          insert_into_linked_list(head, returnToken, this->num_tokens);
          returnToken = Token::PERIOD;
          insert_into_linked_list(head, returnToken, this->num_tokens);
          i += 2;
          read_next();
          read_next();
          break;
        case ',':
          this->char_stack[this->next_ptr] = '\0';
          insert_into_linked_list(head, returnToken, this->num_tokens);
          returnToken = Token::COMMA;
          insert_into_linked_list(head, returnToken, this->num_tokens);
          i += 2;
          read_next();
          read_next();
          break;
        default:
          i++;
          this->read_next();
          break;
        }
      }
    }
  }
  token_stack = std::make_unique<Token[]>(this->num_tokens+1 );
  //  move the tokens from head to the token_stack
  token_node *temp = head;
  token_stack[this->num_tokens] = Token::END;
  for (int i = this->num_tokens-1; i >= 0; i--) {
    token_stack[i] = (*temp).tok;
    temp = temp->next;
    delete temp;
  }
  head = nullptr;
}

void Lexer::read_next() {
  std::cout << this->input[this->ptr] << std::endl;
  // NOTE: should not be called once EOF is reached
  if (this->ptr >= this->len) {
    this->char_stack[this->ptr] = '\0';
  }
  this->cur_char = this->input[this->ptr];
  this->ptr++;
  this->next_ptr++;
}

Token Lexer::get_token() {
  // NOTE: only called when char_stack has a length of atleast one
  int i = 0;
  const int char_stack_len = this->char_stack.length();
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
  State currentState{get_initial_state(this->char_stack[i])};
  i++; // increment before while because all literals are lexed before this
       // statement
  // so the token is atleast two chars long
  while (i < char_stack_len) {
    switch (currentState) {
    // ([a-z][A-z][0-9]|_)*
    case State::IDENTIFIER:
      retToken = Token::IDENTIFIER;
      while (i < char_stack_len) {
        if (!(isalnum(this->char_stack[i]) || this->char_stack[i] == '_')) {
          retToken = Token::INVALID;
        }
      }
      break;
    // &&
    case State::ANDK_1:
      if (this->char_stack[i] == '&') {
        currentState = State::OR_2;
        i++;
      } else
        retToken = Token::INVALID;
      break;
    // || 
    case State::OR_1:
      if (this->char_stack[i] == '|') {
        currentState = State::OR_2;
        i++;
      } else
        retToken = Token::INVALID;
      break;
    case State::OR_2:
      if (i == char_stack_len) {
        currentState = State::END;
        retToken = Token::IF;
        i--;
      } else {
        currentState = State::IDENTIFIER;
        // i++ handled in identifier case
      }
      break;
    case State::IF_1:
      if (this->char_stack[i] == 'f') {
        currentState = State::IF_2;
        i++;
      } else if (this->char_stack[i] == 'n') {
        currentState = State::INTK_2;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::IF_2:
      if (i == char_stack_len) {
        currentState = State::END;
        retToken = Token::IF;
        i--;
      } else {
        currentState = State::IDENTIFIER;
        // i++ handled in identifier case
      }
      break;
    case State::INTK_2:
      if (this->char_stack[i] == 't') {
        currentState = State::INTK_2;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::INTK_3:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::IF;
        i--;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_1:
      if (this->char_stack[i] == 'o') {
        currentState = State::DOUBLEK_2;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_3:
      if (this->char_stack[i] == 'b') {
        currentState = State::DOUBLEK_4;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_4:
      if (this->char_stack[i] == 'l') {
        currentState = State::DOUBLEK_5;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_5:
      if (this->char_stack[i] == 'e') {
        currentState = State::DOUBLEK_6;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_6:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::DOUBLEK;
        i--;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_1:
      if (this->char_stack[i] == 'o') {
        currentState = State::BOOLK_2;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_2:
      if (this->char_stack[i] == 'o') {
        currentState = State::BOOLK_3;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_3:
      if (this->char_stack[i] == 'l') {
        currentState = State::BOOLK_4;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_4:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::DOUBLEK;
        i--;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_1:
      if (this->char_stack[i] == 'h') {
        currentState = State::WHILE_2;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_2:
      if (this->char_stack[i] == 'i') {
        currentState = State::WHILE_3;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_3:
      if (this->char_stack[i] == 'l') {
        currentState = State::WHILE_4;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_4:
      if (this->char_stack[i] == 'e') {
        currentState = State::WHILE_5;
        i++;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_5:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::WHILE;
        i--;
      } else {
        currentState = State::IDENTIFIER;
      }
    case State::FUNCTION_1:
      if (this->char_stack[i] == 'n') {
        currentState = State::FUNCTION_2;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::FUNCTION_2:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::FUNCTION;
        i--;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::STRINGK_1:
      if (this->char_stack[i] == 't') {
        currentState = State::STRINGK_2;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::STRINGK_2:
      if (this->char_stack[i] == 'r') {
        currentState = State::STRINGK_3;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::STRINGK_3:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::STRINGK;
        i--;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::AND_1:
      if (this->char_stack[i] == '&') {
        currentState = State::AND_2;
      } else {
        currentState = State::IDENTIFIER;
      }
      break;
    case State::AND_2:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::AND;
        i--;
      } else {
        currentState = State::INVALID;
      }
      break;
    case State::EQUAL_1:
      if (this->char_stack[i] == '=') {
        currentState = State::EQUAL_2;
      } else if (this->char_stack[i] == '\0') {
        i--;
        retToken = Token::ASSIGN;
        currentState = State::END;
      } else {
        i--;
        retToken = Token::UNDEFINED;
        currentState = State::END;
      }
      break;
    case State::NOT_1:
      if (this->char_stack[i] == '\0') {
        currentState = State::END;
        retToken = Token::NOT;
      } else if (this->char_stack[i] == '=') {
        currentState = State::NOTEQUAL_2;
        i++;
      } else {
        currentState = State::END;
        retToken = Token::UNDEFINED;
        i--;
      }
      break;
    case State::END:
      std::cout << "Token Found: " << std::endl;
      if (retToken == Token::UNDEFINED) {
        std::cerr << "Undefined behavior" << std::endl;
        std::exit(-10);
      }
      return retToken;
      break;
    default:
      break;
    }
  }
  std::cout << "Did not finish lexing " << std::endl;
  std::cout << "Aborting" << std::endl;
  std::exit(-11);
}

#include <cctype>
#include <cstdlib>
#include <iostream>
#include <memory>
#include <string>
#include <string_view>
#include <vector>

#include "./lexer.h"

enum class State : short {
  START,
  TRUEK_1,
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
  EXCLAIMATION_1,
};

State get_initial_state(char c) {
  switch (c) {
  case '_':
    return State::IDENTIFIER;
    break;
  case 'i':
    return State::IF_1;
    break;
  case 't':
    return State::TRUEK_1;
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

// the tokens are inserted in reverse order
int insert_into_linked_list(token_node *&head, TokenChunk tok,
                            int &num_tokens) {
  token_node *new_node = new token_node();
  new_node->tok = tok;
  token_node *temp = head;
  new_node->next = temp;
  head = new_node;
  return num_tokens++;
}

// TODO: take in a string_view and use that to lex the file
Lexer::Lexer(std::string input) {
  if (input.empty()) {
    std::exit(0);
  } else {
    this->input = input;
    this->len = input.length();
    // reserve the char stack in advance
    this->current_token.reserve(1000);
  }
}

void Lexer::process_token(token_node *&head) {
  if (this->current_token_ptr != 0) {
    this->current_token[this->current_token_ptr] = '\0';
    TokenChunk retToken = get_token();
    this->current_token[0] = '\0';
    this->current_token_ptr = 0;
    insert_into_linked_list(head, retToken, this->num_tokens);
  }
}
void Lexer::process_literal_token(TokenChunk &retToken, token_node *&head) {
  if (this->current_token_ptr != 0) {
    this->current_token[this->current_token_ptr] = '\0';
    TokenChunk tok = this->get_token();
    this->current_token[0] = '\0';
    this->current_token_ptr = 0;
    insert_into_linked_list(head, tok, this->num_tokens);
  }
  insert_into_linked_list(head, retToken, this->num_tokens);
  read_next();
}

// calls read_next to update ptr values
void Lexer::tokenize(std::unique_ptr<TokenChunk[]> &token_stack) {
  token_node *head = nullptr;
  int i = 0;
  while (i < this->len) {
    if (this->next_ptr >= this->len) { // reached EOF
      this->current_token[this->current_token_ptr] = '\0';
      if (this->current_token[0] != '\0') {
        const TokenChunk token = get_token();
        insert_into_linked_list(head, token, this->num_tokens);
      }
      this->current_token[0] = '\0';
      this->current_token_ptr = 0;
      i++;
    }

    // if the end of the file is not reached, then the cases are:
    // 1. you might have hit a blank space, in which case,
    //  1. get_token() if there is a token in this->char_stack
    //  2. do nothing if there isn't, increment the ptrs and i
    // 2. any other type of whitespace, do nothing, increment the ptrs and i
    // 3. you might hit a literal with the next ptr, so check if
    // this->input[this->next_ptr] is a literal, if it is:
    //  1. get_token() if there is a token in this->char_stack
    //          then insert the literal into the linked list
    //          IMPORTANT: increment the ptrs twice(since the literal was
    //          detected using this-> next_ptr, and not this->ptr)
    //  2. do nothing if there isn't, increment the ptrs twice, and insert the
    //  literal into the list
    // 4. If you don't encounter a whitespace or a literal, then keep going by
    // calling read_next()

    else {
      this->current_char = this->input[this->ptr];
      this->current_token[this->current_token_ptr] = '\0';
      if (this->current_char == ' ' || this->current_char == '\t') {
        if (this->current_token[0] != '\0') {
          this->current_token[this->current_token_ptr] = '\0';
          TokenChunk retToken = get_token();
          insert_into_linked_list(head, retToken, this->num_tokens);
          this->current_token[0] = '\0';
          this->current_token_ptr = 0;
        }
        i++;
        this->read_next();
      } else if (this->current_char == '\n') {
        if (this->current_token[0] != '\0') {
          this->current_token[this->current_token_ptr] = '\0';
          TokenChunk retToken = get_token();
          insert_into_linked_list(head, retToken, this->num_tokens);
          this->current_token[0] = '\0';
          this->current_token_ptr = 0;
        }
        i++;
        this->read_next();
      } else {
        TokenChunk retToken;
        switch (this->input[this->ptr]) {
        case '#': // comments
          while (this->input[this->ptr] != '\n') {
            i++;
            this->read_next();
          }
          break;
        case '{':
          retToken = {Token::LEFTBRACE, "{"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case '}':
          retToken = {Token::RIGHTBRACE, "}"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case '(':
          retToken = {Token::LEFTPARENTHESIS, "("};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case ')':
          retToken = {Token::RIGHTPARENTHESIS, ")"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case '.':
          retToken = {Token::PERIOD, "."};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case ',':
          retToken = {Token::COMMA, ","};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case ':':
          retToken = {Token::COLON, ":"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case '!':
          retToken = {Token::EXCLAIM, "!"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case '*':
          retToken = {Token::MULTIPLY, "*"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case '+':
          retToken = {Token::ADD, "+"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          break;
        case '-':
          retToken = {Token::SUBTRACT, "-"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case '/':
          retToken = {Token::DIVIDE, "/"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        case ';':
          retToken = {Token::SEMICOLON, ";"};
          this->process_token(head);
          this->process_literal_token(retToken, head);
          i++;
          break;
        default:
          // capture strings starting with double quotes
          if (this->current_char == '"') {
            if (this->current_token_ptr > 0) {
              this->current_token[this->current_token_ptr] = '\0';
              insert_into_linked_list(head, retToken, this->num_tokens);
            }
            read_next();
            i++;
            while (this->input[this->ptr] != '"') {
              read_next();
              i++;
            }
            if (this->input[this->ptr] == '"')
              retToken = {Token::STRING, this->current_token};
            else
              retToken = {Token::UNDEFINED, this->current_token};
            insert_into_linked_list(head, retToken, this->num_tokens);
            this->current_token[0] = '\0';
            this->current_token_ptr = 0;
            i++;
            read_next();
          } else if (this->current_char == '\'') {
            if (this->input[this->ptr + 2] == '\'') {
              insert_into_linked_list(
                  head,
                  {Token::CHAR, std::string(1, this->input[this->ptr + 1])},
                  this->num_tokens);
              read_next(3);
              i += 3;
              this->current_token[0] = '\0';
              this->current_token_ptr = 0;
            } else {
              // error out since a char either too long or too short
              throw std::runtime_error("Expected expression for char at line ");
            }
          } else [[likely]] {
            this->current_token[this->current_token_ptr] = this->current_char;
            this->current_token_ptr++;
            i++;
            this->read_next();
          }
          break;
        }
      }
    }
  }
  token_stack = std::make_unique<TokenChunk[]>(this->num_tokens + 2);
  token_stack[0] = {Token::START, ""};
  token_node *temp;
  for (int i = this->num_tokens - 1; i >= 0; i--) {
    if (head != nullptr) {
      temp = head;
      token_stack[i + 1] = (*head).tok;
      head = head->next;
      delete temp;
    }
  }
  head = nullptr;
  token_stack[this->num_tokens + 1] = {Token::END, ""};
}

// should not be called once EOF is reached
void Lexer::read_next() noexcept {
  if (this->ptr >= this->len) {
    this->current_token[this->ptr] = '\0';
  }
  this->current_char = this->input[this->ptr];
  this->ptr += 1;
  this->next_ptr += 1;
}

// overloaded method to avoid multiple function calls calling read()
void Lexer::read_next(int count) noexcept {
  if (count > 0) {
    if (this->ptr >= this->len) {
      this->current_token[this->ptr] = '\0';
    }
    this->current_char = this->input[this->ptr];
    this->ptr += count;
    this->next_ptr += count;
  }
}

// NOTE: only called when char_stack has a length of atleast one
TokenChunk Lexer::get_token() {
  int i = 0;
  int token_len = this->current_token_ptr;
  while (std::isdigit(this->current_token[i])) {
    i++;
  }
  if (this->current_token[i] == '\0') {
    return {Token::INT, std::stoi(this->current_token)};
  } else if (this->current_token[i] == '.') {
    while (std::isdigit(this->current_token[i])) {
      i++;
    }
    if (this->current_token[i] == '\0') {
      return {Token::DOUBLE, std::stod(this->current_token)};
    }
  }
  if (i != 0) {
    // neither float nor string, but starts with a digit
    return {Token::INVALID, "INVALID"};
  }
  Token retToken;
  State currentState{get_initial_state(this->current_token[i])};
  i++; // increment before while because all literals are lexed before this
  // statement so the token is atleast two chars long
  while (i <= token_len) {
    switch (currentState) {
      // ([a-z][A-z][0-9]|_)*
    case State::IDENTIFIER:
      retToken = Token::IDENTIFIER;
      while (i < token_len) {
	if (!(isalnum(this->current_token[i]) ||
	      this->current_token[i] == '_')) {
	  retToken = Token::INVALID;
	}
	i++;
      }
      currentState = State::END;
      break;
    case State::ANDK_1:
      if (this->current_token[i] == '&') {
	currentState = State::OR_2;
	i++;
      } else
	retToken = Token::INVALID;
      break;
    case State::OR_1:
      if (this->current_token[i] == '|') {
	currentState = State::OR_2;
	i++;
      } else
	retToken = Token::INVALID;
      break;
    case State::OR_2:
      if (i == token_len) {
	currentState = State::END;
	retToken = Token::OR;
	i--;
      } else {
	currentState = State::IDENTIFIER;
	// i++ handled in identifier case
      }
      break;
    case State::IF_1:
      if (this->current_token[i] == 'f') {
	currentState = State::IF_2;
	i++;
      } else if (this->current_token[i] == 'n') {
	currentState = State::INTK_2;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::IF_2:
      if (i == token_len) {
	currentState = State::END;
	retToken = Token::IF;
	i--;
      } else {
	currentState = State::IDENTIFIER;
	// i++ handled in identifier case
      }
      break;
    case State::ELSE_1:
      if (this->current_token[i] == 'l') {
	currentState = State::ELSE_2;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::ELSE_2:
      if (this->current_token[i] == 's') {
	currentState = State::ELSE_3;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::ELSE_3:
      if (this->current_token[i] == 'e') {
	currentState = State::ELSE_4;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::ELSE_4:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::ELSE;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::INTK_2:
      if (this->current_token[i] == 't') {
	currentState = State::INTK_3;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::INTK_3:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::INTK;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::TRUEK_1:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::TRUEK;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_1:
      if (this->current_token[i] == 'o') {
	currentState = State::DOUBLEK_2;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_3:
      if (this->current_token[i] == 'b') {
	currentState = State::DOUBLEK_4;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_4:
      if (this->current_token[i] == 'l') {
	currentState = State::DOUBLEK_5;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_5:
      if (this->current_token[i] == 'e') {
	currentState = State::DOUBLEK_6;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::DOUBLEK_6:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::DOUBLEK;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_1:
      if (this->current_token[i] == 'o') {
	currentState = State::BOOLK_2;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_2:
      if (this->current_token[i] == 'o') {
	currentState = State::BOOLK_3;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_3:
      if (this->current_token[i] == 'l') {
	currentState = State::BOOLK_4;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::BOOLK_4:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::BOOLK;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_1:
      if (this->current_token[i] == 'h') {
	currentState = State::WHILE_2;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_2:
      if (this->current_token[i] == 'i') {
	currentState = State::WHILE_3;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_3:
      if (this->current_token[i] == 'l') {
	currentState = State::WHILE_4;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_4:
      if (this->current_token[i] == 'e') {
	currentState = State::WHILE_5;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::WHILE_5:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::WHILE;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
    case State::FUNCTION_1:
      if (this->current_token[i] == 'n') {
	currentState = State::FUNCTION_2;
	i++;
      } else if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::FALSEK;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::FUNCTION_2:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::FN;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::STRINGK_1:
      if (this->current_token[i] == 't') {
	currentState = State::STRINGK_2;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::STRINGK_2:
      if (this->current_token[i] == 'r') {
	currentState = State::STRINGK_3;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::STRINGK_3:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::STRINGK;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::CHARK_1:
      if (this->current_token[i] == 'h') {
	currentState = State::CHARK_2;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::CHARK_2:
      if (this->current_token[i] == 'a') {
	currentState = State::CHARK_3;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::CHARK_3:
      if (this->current_token[i] == 'r') {
	currentState = State::CHARK_4;
	i++;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::CHARK_4:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::CHARK;
	i--;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::AND_1:
      if (this->current_token[i] == '&') {
	currentState = State::AND_2;
      } else {
	currentState = State::IDENTIFIER;
      }
      break;
    case State::AND_2:
      if (this->current_token[i] == '\0') {
	currentState = State::END;
	retToken = Token::AND;
	i--;
      } else {
	currentState = State::INVALID;
      }
      break;
    case State::EQUAL_1:
      if (this->current_token[i] == '=') {
	currentState = State::EQUAL_2;
      } else if (this->current_token[i] == '\0') {
	i--;
	retToken = Token::ASSIGN;
	currentState = State::END;
      } else {
	i--;
	retToken = Token::UNDEFINED;
	currentState = State::END;
	break;
	case State::EQUAL_2:
	  if (this->current_token[i] == '\0') {
	    i--;
	    currentState = State::END;
	    retToken = Token::EQUAL;
	  }
	  break;
	  case State::NOT_1:
	    if (this->current_token[i] == '\0') {
	      currentState = State::END;
	      retToken = Token::NOT;
	    } else if (this->current_token[i] == '=') {
	      currentState = State::NOTEQUAL_2;
	      i++;
	    } else {
	      currentState = State::END;
	      retToken = Token::UNDEFINED;
	      i--;
	    }
	    break;
	    case State::END:
	      if (retToken == Token::UNDEFINED) {
		std::cerr << "Undefined behavior" << std::endl;
		std::exit(-10);
	      }
	      return {retToken, this->current_token};
	      break;
	      default:
		break;
      }
    }
  }
  std::cout << "Did not finish lexing " << std::endl;
  std::cout << "Aborting" << std::endl;
  std::exit(-11);
}

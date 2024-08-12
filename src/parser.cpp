#include "parser.h"
#include "globals.h"
#include "helper.h"
#include <array>
#include <cstdlib>
#include <exception>
#include <iostream>
#include <memory>
#include <stdexcept>
#include <type_traits>

bool operator==(Token t, ParserToken pt) {
  return static_cast<int>(t) == static_cast<int>(pt);
}

bool operator==(ParserToken pt, Token t) {
  return static_cast<int>(pt) == static_cast<int>(t);
}

ParserToken token_to_parser_token(const Token &t) {
  return static_cast<ParserToken>(t);
}

ParserTokenChunk tc_to_ptc(const TokenChunk &tc) {
  ParserTokenChunk ptc;
  ptc.type = token_to_parser_token(tc.type);
  ptc.value = tc.value;
  return ptc;
}

struct OutputQueueNode {
  std::unique_ptr<PTNode> node = nullptr;
  ParserTokenChunk *ptc = nullptr;
  OutputQueueNode *next = nullptr;
};

struct OutputQueue {
  OutputQueueNode *head = nullptr;
  OutputQueueNode *tail = nullptr;
};
/*
 * Adds nodes to tail
 */
void add_sym_to_output_queue(std::unique_ptr<OutputQueue> &queue,
                             ParserTokenChunk &sym) {
  OutputQueueNode *new_node = new OutputQueueNode;
  new_node->node = std::make_unique<PTNode>(sym);
  if (queue->tail == nullptr) {
    queue->tail = new_node;
    queue->head = new_node;
  } else {
    queue->tail->next = new_node;
    queue->tail = queue->tail->next;
  }
}

/*
 * Remove nodes from head
 */
std::unique_ptr<ParserTokenChunk>
remove_tail_from_queue(std::unique_ptr<OutputQueue> &queue) {
  if (queue->head == nullptr) {
    return nullptr;
  }
  std::unique_ptr<ParserTokenChunk> ptc =
      std::make_unique<ParserTokenChunk>(*queue->head->ptc);
  OutputQueueNode *temp = queue->head;
  queue->head = temp->next;
  if (queue->head == nullptr) {
    queue->tail = nullptr;
  }
  delete temp;
  return ptc;
}

struct OperatorStackNode {
  ParserTokenChunk *ptc = nullptr;
  OperatorStackNode *next = nullptr;
};

struct OperatorStack {
  OperatorStackNode *head = nullptr;
};

void add_op_to_stack(std::unique_ptr<OperatorStack> &stack,
                     ParserTokenChunk &op) {
  if (stack->head->ptc == nullptr) {
    stack->head = new OperatorStackNode;
    stack->head->ptc = &op;
  } else {
    OperatorStackNode *new_node = new OperatorStackNode;
    new_node->ptc = &op;
    stack->head->next = new_node;
    stack->head = new_node;
  }
}

std::unique_ptr<ParserTokenChunk>
pop_from_stack(std::unique_ptr<OperatorStack> &stack) {
  if (stack->head == nullptr) {
    return nullptr;
  }
  std::unique_ptr<ParserTokenChunk> ptc =
      std::make_unique<ParserTokenChunk>(*stack->head->ptc);
  OperatorStackNode *temp = stack->head;
  stack->head = stack->head->next;
  delete temp;
  return ptc;
}

Token parser_token_to_token(const ParserToken &pt) {
  return static_cast<Token>(pt);
}

TokenChunk ptc_to_tc(const ParserTokenChunk &ptc) {
  TokenChunk tc;
  tc.type = parser_token_to_token(ptc.type);
  tc.value = ptc.value;
  return tc;
}

Parser::Parser(std::unique_ptr<TokenChunk[]> &token_s) {
  token_stream = std::move(token_s);
  ParserTokenChunk ptc;
}

void PTNode::print(const int spaces) {
  for (int i = 0; i < spaces; i++) {
    std::cout << " ";
  }
  std::cout << token_to_string(this->val->type) << std::endl;
  if (this->first_child) {
    this->first_child->print(spaces + 2);
  }
  if (this->next_sibling) {
    this->next_sibling->print(spaces);
  }
}

void PTNode::add_child(ParserTokenChunk &tok) {
  if (this->last_child != nullptr) {
    this->last_child->add_sibling(tok);
    this->last_child = this->last_child->next_sibling;
  } else {
    this->first_child = new PTNode(tok);
    this->first_child->val->value = tok.value;
    this->first_child->val->type = tok.type;
    this->last_child = this->first_child;
  }
}

void PTNode::add_sibling(ParserTokenChunk &tok) {
  if (this->next_sibling != nullptr) {
    this->next_sibling->add_sibling(tok);
  } else {
    this->next_sibling = new PTNode(tok);
    ;
    this->next_sibling->prev_sibling = this;
  }
}

void PTNode::add_sibling(PTNode *sibling) {
  if (this->next_sibling != nullptr) {
    this->next_sibling->add_sibling(sibling);
  } else {
    this->next_sibling = sibling;
    sibling->prev_sibling = this;
  }
}

void PTNode::add_child(PTNode *child) {
  if (this->last_child != nullptr) {
    this->last_child->add_sibling(child);
    this->last_child = this->last_child->next_sibling;
  } else {
    this->first_child = child;
    this->first_child = child;
    this->last_child = this->first_child;
  }
}

PTNode::~PTNode() { // NOTE: pointless
  delete this->first_child;
  delete this->next_sibling;
}

PTNode::PTNode(ParserTokenChunk tok) {
  this->val = std::make_unique<ParserTokenChunk>();
  this->val->type = tok.type;
  this->val->value = tok.value;
}

OperatorPrecedence token_type_to_precedence(const ParserToken &tok) {
  switch (tok) {
  case ParserToken::MULTIPLY:
  case ParserToken::DIVIDE:
    return OperatorPrecedence::MULTIDIV;
  case ParserToken::ADD:
  case ParserToken::SUBTRACT:
    return OperatorPrecedence::ADDSUB;
  case ParserToken::GREATERTHANEQUAL:
  case ParserToken::LESSTHANEQUAL:
  case ParserToken::GREATERTHAN:
  case ParserToken::LESSERTHAN:
    return OperatorPrecedence::COMPARISON;
  case ParserToken::NOT:
    return OperatorPrecedence::NOT;
  case ParserToken::ASSIGN:
    return OperatorPrecedence::ASSIGN;
  case ParserToken::AND:
    return OperatorPrecedence::AND;
  case ParserToken::OR:
    return OperatorPrecedence::OR;
  default:
    return OperatorPrecedence::NONE;
  }
}

void Parser::next() { this->_ptr++; }

TokenChunk Parser::peek() const noexcept {
  return this->token_stream[_ptr + 1];
}
TokenChunk Parser::peek(int k) const { return this->token_stream[_ptr + k]; }

TokenChunk Parser::get() const noexcept { return this->token_stream[_ptr]; }

void Parser::parse_stmt() {} // TODO:

PTNode *Parser::parse_expr() { // TODO:
  PTNode *expression = nullptr;
  if (this->get().type == Token::LEFTPARENTHESIS) {
    this->next();
    expression->add_child(this->ptcs.left_paren);
    PTNode *temp = nullptr;
    auto current = this->get();
    ParserTokenChunk ptc = tc_to_ptc(current);
    std::unique_ptr<OutputQueue> output_q = std::make_unique<OutputQueue>();
    auto op_stack = std::make_unique<OperatorStack>();
    while (this->get().type != Token::RIGHTPARENTHESIS) {
      switch (current.type) {
      case Token::RIGHTPARENTHESIS:
        temp = this->parse_expr();
        expression->add_child(temp);
        break;
      case Token::LEFTPARENTHESIS:
        temp = this->parse_expr();
        expression->add_child(temp);
        break;
      case Token::INT:
      case Token::DOUBLE:
        add_sym_to_output_queue(output_q, ptc);
        break;
      default:
        // TODO: add fn calls, and comma support for factors in fn calls
        if (op_stack->head == nullptr) {
          add_op_to_stack(op_stack, ptc);
          break;
        }
        while (token_type_to_precedence(op_stack->head->ptc->type) >=
               token_type_to_precedence(ptc.type)) {
          add_sym_to_output_queue(output_q, *pop_from_stack(op_stack));
        }
        add_op_to_stack(op_stack, ptc);
        break;
      }
      this->next();
    }
    while (op_stack->head != nullptr) {
      add_sym_to_output_queue(output_q, *pop_from_stack(op_stack));
    }
    // (1+2+3)
    // 1 2 3 + + +
    while (output_q->head) {
    }
  }
  expression->add_child(this->ptcs.right_paren);
  return expression;
}

/*
 * Implement the shunting yard algorithm
 */
void Parser::shunting_yard() {
  auto output_q = std::make_unique<OutputQueue>();
  auto o_stack = std::make_unique<OperatorStack>();
  while (this->get().type != Token::END) {
    ParserTokenChunk current = tc_to_ptc(this->get());
    if (current.type == Token::INT || current.type == Token::DOUBLE) {
      add_sym_to_output_queue(output_q, current);
    } else if (current.type == Token::ADD || current.type == Token::SUBTRACT ||
               current.type == Token::MULTIPLY ||
               current.type == Token::DIVIDE) {
      add_op_to_stack(o_stack, current);
    } else if (current.type == Token::LEFTPARENTHESIS) {
      add_op_to_stack(o_stack, current);
    } else if (current.type == Token::RIGHTPARENTHESIS) {
      while (!(o_stack->head->ptc->type == Token::LEFTPARENTHESIS)) {
        add_sym_to_output_queue(output_q, *pop_from_stack(o_stack));
      }
      pop_from_stack(o_stack);
    }
  }
}

PTNode *expr_to_tree() { return nullptr; } // TODO:

PTNode *Parser::parse_if_stmt() {
  PTNode *if_stmt = new PTNode(this->ptcs.if_stmt);
  this->next();
  this->parse_expr(); // TODO:
  this->next();
  if (this->get().type == Token::LEFTBRACE) {
    this->parse_stmt();
    if (this->get().type == Token::RIGHTBRACE) {
    }
  }
  return if_stmt;
}

PTNode *Parser::parse_while_stmt() {
  PTNode *while_stmt = new PTNode(this->ptcs.while_stmt);
  this->next();
  this->parse_expr();
  this->next();
  return while_stmt;
}

PTNode *Parser::parse_var_decl() {
  PTNode *var_decl = new PTNode(this->ptcs.var_decl);
  if (!(this->get().type == Token::INTK || this->get().type == Token::CHARK ||
        this->get().type == Token::DOUBLEK ||
        this->get().type == Token::STRINGK)) {
    throw std::runtime_error("parse_var_decl() expects type keyword");
  }
  this->next();
  ParserTokenChunk type_k;
  type_k.type = token_to_parser_token(this->get().type);
  type_k.value = this->get().value;
  PTNode *type = new PTNode(type_k);
  this->next();

  ParserTokenChunk ident_ptc;
  ident_ptc.type = token_to_parser_token(this->get().type);
  ident_ptc.value = this->get().value;
  PTNode *ident = new PTNode(ident_ptc);
  this->next();

  PTNode *assign = new PTNode(this->ptcs.assign);
  this->next();

  PTNode *expr = this->parse_expr();
  this->next();

  var_decl->add_child(type);
  var_decl->add_child(ident);
  var_decl->add_child(assign);
  var_decl->add_child(expr);
  return var_decl;
}

void Parser::parse(std::unique_ptr<PTNode> &head) {
  if (this->token_stream[_ptr].type == Token::START) {
    ParserTokenChunk start;
    start.type = ParserToken::START;
    this->head = new PTNode(start);
    this->next();
    while (this->token_stream[_ptr].type != Token::END) {
      if ((this->get().type == Token::IF)) {
        this->parse_if_stmt();
      } else if (this->get().type == Token::WHILE) {
        this->parse_while_stmt();
      } else if (this->get().type == Token::INTK ||
                 this->get().type == Token::CHARK ||
                 this->get().type == Token::DOUBLEK ||
                 this->get().type == Token::STRINGK) {
        if (this->peek().type == Token::IDENTIFIER &&
            this->peek(2).type == Token::ASSIGN) {
          this->parse_var_decl();
        }
      }
    }
    ParserTokenChunk end;
    end.type = ParserToken::END;
    this->head->add_sibling(end);
  }
}

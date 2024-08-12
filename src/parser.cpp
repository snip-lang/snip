#include "parser.h"
#include "error.h"
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

void add_node_to_output_queue(std::unique_ptr<OutputQueue> &queue,
                              PTNode *node) {
  OutputQueueNode *new_node = new OutputQueueNode;
  new_node->node = std::unique_ptr<PTNode>(node);
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
std::unique_ptr<PTNode>
pop_from_output_queue(std::unique_ptr<OutputQueue> &queue) {
  if (queue->head == nullptr) {
    return nullptr;
  }
  std::unique_ptr<PTNode> node = std::move(queue->head->node);
  OutputQueueNode *temp = queue->head;
  queue->head = temp->next;
  if (queue->head == nullptr) {
    queue->tail = nullptr;
  }
  delete temp;
  return node;
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

PTNode *Parser::parse_expr() { // TODO: parse output queue
  PTNode *expression = new PTNode(this->ptcs.expr);
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
      case Token::LEFTPARENTHESIS:
        temp = this->parse_expr();
        expression->add_child(temp);
        break;
      case Token::INT:
      case Token::DOUBLE:
      case Token::IDENTIFIER:
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
          // pop from stack, create a node with its operands, and add to output
          // queue
          auto op = *pop_from_stack(op_stack);
          auto right = pop_from_output_queue(output_q);
          auto left = pop_from_output_queue(output_q);
          if (left == nullptr || right == nullptr) {
            throw std::runtime_error(
                "binop in parse_expr() expects left and right");
          }
          PTNode *op_node = new PTNode(op);
          PTNode *left_node = left.release();
          PTNode *right_node = right.release();
          op_node->add_child(left_node);
          op_node->add_child(right_node);

          add_node_to_output_queue(output_q, op_node);
        }
        add_op_to_stack(op_stack, ptc);
        break;
      }
      this->next();
    }
    while (op_stack->head != nullptr) {
      add_sym_to_output_queue(output_q, *pop_from_stack(op_stack));
    }
    while (output_q->head != nullptr) {
      auto node = pop_from_output_queue(output_q);
      expression->add_child(node.release());
    }
  }
  expression->add_child(this->ptcs.right_paren);
  return expression;
}

PTNode *Parser::parse_if_stmt() {
  PTNode *if_stmt = new PTNode(this->ptcs.if_stmt);
  this->next();
  PTNode *cond = this->parse_expr(); // TODO:
  if (cond == nullptr) {
    throw std::runtime_error("parse_if_stmt() expects condition");
  }
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

  ParserTokenChunk type_k = {token_to_parser_token(this->get().type),
                             this->get().value};
  var_decl->add_child(new PTNode(type_k));
  this->next();

  ParserTokenChunk ident_ptc = {token_to_parser_token(this->get().type),
                                this->get().value};
  var_decl->add_child(new PTNode(ident_ptc));
  this->next();

  if (this->get().type == Token::ASSIGN) {
    var_decl->add_child(new PTNode(this->ptcs.assign));
    this->next();

    var_decl->add_child(this->parse_expr());
    this->next();
  }

  var_decl->add_child(new PTNode(this->ptcs.semicolon));
  this->next();
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
        if (this->peek().type == Token::IDENTIFIER) {
          this->parse_var_decl();
        }
      }
    }
    ParserTokenChunk end;
    end.type = ParserToken::END;
    this->head->add_sibling(end);
  }
}

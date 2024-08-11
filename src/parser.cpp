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

PTNode::~PTNode() { // pointless
  delete this->first_child;
  delete this->next_sibling;
  if (!(this->val->type == ParserToken::IFSTMT ||
        this->val->type == ParserToken::WHILESTMT ||
        this->val->type == ParserToken::FNDECL)) {
    delete val;
  }
}

PTNode::PTNode(ParserTokenChunk &tok) {
  this->val = new ParserTokenChunk;
  this->val->type = tok.type;
  this->val->value = tok.value;
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
    while (this->get().type != Token::RIGHTPARENTHESIS) {
      this->next();
      if (this->get().type == Token::LEFTPARENTHESIS) {
        PTNode *temp = this->parse_expr();
        expression->add_child(temp);
      }
    }
    return expression;
  }
  return expression;
}

struct OutputQueueNode {
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
void add_output_sym_to_queue(OutputQueue *&queue, ParserTokenChunk &sym) {
  if (queue->tail == nullptr) {
    OutputQueueNode *new_node = new OutputQueueNode;
    new_node->ptc = &sym;
    queue->head = new_node;
    queue->tail = new_node;
  } else {
    OutputQueueNode *new_node = new OutputQueueNode;
    new_node->ptc = &sym;
    queue->tail->next = new_node;
    queue->tail = new_node;
  }
}

/*
 * Remove nodes from head
 */
ParserTokenChunk *remove_tail_from_queue(OutputQueue *&queue) {
  if (queue->head == nullptr) {
    return nullptr;
  }
  OutputQueueNode *temp = queue->head;
  queue->head = temp->next;
  if (queue->head == nullptr) {
    queue->tail = nullptr;
  }
  return temp->ptc;
}

struct OperatorStackNode {
  ParserTokenChunk *ptc = nullptr;
  OperatorStackNode *next = nullptr;
};

struct OperatorStack {
  OperatorStackNode *head = nullptr;
};

void add_op_to_stack(OperatorStack *&stack, ParserTokenChunk &op) {
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

ParserTokenChunk *remove_op_from_stack(OperatorStack *&stack) {
  if (stack->head == nullptr) {
    return nullptr;
  }
  OperatorStackNode *temp = stack->head;
  stack->head = temp->next;
  return stack->head->ptc;
}

/*
 * Implement the shunting yard algorithm
 */
void Parser::shunting_yard() {
  OutputQueue *output_q = new OutputQueue;
  OperatorStack *o_stack = new OperatorStack;
  while (this->get().type != Token::END) {
    ParserTokenChunk current = tc_to_ptc(this->get());
    if (current.type == Token::INT || current.type == Token::DOUBLE) {
      add_output_sym_to_queue(output_q, current);
    } else if (current.type == Token::ADD || current.type == Token::SUBTRACT ||
               current.type == Token::MULTIPLY ||
               current.type == Token::DIVIDE) {
      add_op_to_stack(o_stack, current);
    } else if (current.type == Token::LEFTPARENTHESIS) {
      add_op_to_stack(o_stack, current);
    } else if (current.type == Token::RIGHTPARENTHESIS) {
      while (!(o_stack->head->ptc->type == Token::LEFTPARENTHESIS)) {
        add_output_sym_to_queue(output_q, *remove_op_from_stack(o_stack));
      }
      remove_op_from_stack(o_stack);
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
  // TYPE IDENT ASSIGN EXPR
  PTNode *var_decl = new PTNode(this->ptcs.var_decl);
  this->next();
  if (!(this->get().type == Token::INTK || this->get().type == Token::CHARK ||
        this->get().type == Token::DOUBLEK ||
        this->get().type == Token::STRINGK)) {
    throw std::runtime_error("parse_var_decl() expectes type keyword");
  }
  ParserTokenChunk *type_k = new ParserTokenChunk;
  type_k->type = token_to_parser_token(this->get().type);
  type_k->value = this->get().value;
  PTNode *type = new PTNode(*type_k);
  this->next();

  ParserTokenChunk *ident_ptc = new ParserTokenChunk;
  ident_ptc->type = token_to_parser_token(this->get().type);
  ident_ptc->value = this->get().value;
  PTNode *ident = new PTNode(*ident_ptc);
  this->next();

  delete type_k;
  delete ident_ptc;
  var_decl->add_child(type);
  var_decl->add_child(ident);
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

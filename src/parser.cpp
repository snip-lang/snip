#include "parser.h"
#include "error.h"
#include "globals.h"
#include "helper.h"
#include <array>
#include <cassert>
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
  // TODO: add different types of PTNode like expr, number
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
  delete first_child;
  delete next_sibling;
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

PTNode *Parser::parse_stmts() {
  PTNode *stmts = new PTNode(this->ptcs.stmts);
  if (this->get().type != Token::LEFTBRACE) {
    return nullptr; // TODO: consider allowing singular stmts without braces
  }
  stmts->add_child(new PTNode(this->ptcs.left_brace));
  this->next();
  while (this->get().type != Token::RIGHTBRACE) {
    PTNode *stmt = this->parse_stmt();
    stmts->add_child(stmt);
  }
  this->next();
  stmts->add_child(new PTNode(this->ptcs.right_brace));
  return stmts;
}

PTNode *Parser::parse_stmt() {
  PTNode *stmt{new PTNode(this->ptcs.if_stmt)};
  if (this->get().type != Token::LEFTBRACE) {
    return nullptr;
  }
  this->get();
  switch (this->get().type) {
  case Token::IF:
    stmt->add_child(this->parse_if_stmt());
    break;
  case Token::WHILE:
    stmt->add_child(this->parse_while_stmt());
  case Token::INTK:
  case Token::CHARK:
  case Token::DOUBLEK:
  case Token::STRINGK:
    if (this->peek().type == Token::IDENTIFIER) {
      stmt->add_child(this->parse_var_decl());
    }
    break;
  case Token::IDENTIFIER:
    if (this->peek().type == Token::ASSIGN) {
      stmt->add_child(this->parse_assignment());
    }
    // TODO: think of other cases
    break;
  default:
    throw std::runtime_error("Stmt type not recognized");
    break;
  }
  stmt->add_child(this->ptcs.semicolon);
  return stmt;
}

PTNode *Parser::parse_assignment() {
  PTNode *assignment = new PTNode(this->ptcs.assignstmt);
  ParserTokenChunk ident = {ParserToken::IDENTIFIER, this->get().value};
  assignment->add_child(new PTNode(ident));
  this->next();
  PTNode *assign = new PTNode(this->ptcs.assign);
  assignment->add_child(assign);
  this->next();
  PTNode *expr = this->parse_expr();
  if (expr == nullptr) {
    std::runtime_error("parse_assignment() expects an expression");
  }
  if (this->get().type != Token::SEMICOLON) {
    Error("Assignment statement expects a semicolon", 0, Severity::ERROR,
          __FILE__, __LINE__);
  }
  assignment->add_child(expr);
  assignment->add_child(this->ptcs.semicolon);
  return assignment;
}

PTNode *Parser::parse_expr() {
  PTNode *expr{new PTNode(this->ptcs.expr)};
  PTNode *parens = nullptr;
  PTNode *expression = nullptr;
  PTNode *temp = nullptr;
  TokenChunk current;
  ParserTokenChunk ptc;
  std::unique_ptr<OutputQueue> output_q = std::make_unique<OutputQueue>();
  auto op_stack = std::make_unique<OperatorStack>();
  if (this->get().type == Token::LEFTPARENTHESIS) {
    this->next();
    parens = new PTNode(this->ptcs.left_paren);
  }
  expression = (parens == nullptr) ? expr : parens;
  while (this->get().type != Token::RIGHTPARENTHESIS &&
         this->get().type != Token::SEMICOLON &&
         this->get().type != Token::COMMA) {
    current = this->get();
    ptc = tc_to_ptc(current);
    switch (current.type) {
    case Token::LEFTPARENTHESIS:
      temp = this->parse_expr();
      expression->add_child(temp);
      break;
    case Token::INT:
    case Token::DOUBLE:
    case Token::STRING:
    case Token::IDENTIFIER:
    case Token::ADD:
    case Token::SUBTRACT:
    case Token::MULTIPLY:
    case Token::DIVIDE:
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
        op_node->add_child(left.release());
        op_node->add_child(right.release());
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
  if (parens == nullptr) {
  } else {
    if (this->get().type == Token::RIGHTPARENTHESIS) {
      parens->add_sibling(this->ptcs.right_paren);
      expr->add_child(parens);
      this->next();
    } else
      throw std::runtime_error("Missing closing ')'");
  }

  return expr;
}

PTNode *Parser::parse_if_stmt() {
  PTNode *if_stmt = new PTNode(this->ptcs.if_stmt);
  this->next();
  PTNode *cond = this->parse_expr();
  if (cond == nullptr) {
    throw std::runtime_error("parse_if_stmt() expects condition");
  }
  PTNode *block = this->parse_stmts();
  if (block == nullptr) {
    throw std::runtime_error("parse_if_stmt() expects block");
  }
  if_stmt->add_child(cond);
  if_stmt->add_child(block);
  return if_stmt;
}

PTNode *Parser::parse_while_stmt() {
  PTNode *while_stmt = new PTNode(this->ptcs.while_stmt);
  assert(this->get().type == Token::WHILE);
  while_stmt->add_child(new PTNode(this->ptcs.while_stmt));
  this->next();
  PTNode *expr = this->parse_expr();
  if (expr == nullptr) {
    throw std::runtime_error("while() requires an expression");
  }
  while_stmt->add_child(expr);
  this->next();
  PTNode *stmts = this->parse_stmts();
  while_stmt->add_child(stmts);
  return while_stmt;
}

PTNode *Parser::parse_var_decl() {
  PTNode *var_decl = new PTNode(this->ptcs.var_decl);
  assert((this->get().type == Token::INTK || this->get().type == Token::CHARK ||
          this->get().type == Token::DOUBLEK ||
          this->get().type == Token::STRINGK));
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
  }
  if (this->get().type != Token::SEMICOLON) {
    throw std::runtime_error("Variable declaration expects a semicolon");
  }
  var_decl->add_child(new PTNode(this->ptcs.semicolon));
  this->next();
  return var_decl;
}

void Parser::parse(std::unique_ptr<PTNode> &head) {
  assert(this->get().type == Token::START);
  ParserTokenChunk start = {ParserToken::START, ""};
  this->head = new PTNode(start);
  this->next();
  while (this->token_stream[_ptr].type != Token::END) {
    switch (this->get().type) {
    case Token::IF:
      this->head->add_child(this->parse_if_stmt());
      break;
    case Token::WHILE:
      this->head->add_child(this->parse_while_stmt());
    case Token::INTK:
    case Token::CHARK:
    case Token::DOUBLEK:
    case Token::STRINGK:
      if (this->peek().type == Token::IDENTIFIER) {
        this->head->add_child(this->parse_var_decl());
      }
      break;
    default:
      break;
    }
  }
  ParserTokenChunk end = {ParserToken::END, ""};
  this->head->add_sibling(end);
  this->head->print(2);
  delete this->head;
}

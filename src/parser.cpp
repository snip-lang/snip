#include "parser.h"
#include "error.h"
#include "globals.h"
#include "helper.h"
#include <cassert>
#include <climits>
#include <iostream>
#include <memory>
#include <ostream>
#include <sstream>
#include <stack>
#include <stdexcept>

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
  if (queue->tail == nullptr) {
    queue->tail = new_node;
    queue->head = new_node;
  } else {
    queue->tail->next = new_node;
    queue->tail = queue->tail->next;
  }
}

/*
  Add nodes to tail
 */
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
  OperatorStackNode *new_node = new OperatorStackNode;
  new_node->ptc = new ParserTokenChunk;
  new_node->ptc->type = op.type;
  new_node->ptc->value = op.value;
  new_node->next = stack->head;
  stack->head = new_node;
}

std::unique_ptr<ParserTokenChunk>
pop_from_stack(std::unique_ptr<OperatorStack> &stack) {
  if (stack->head == nullptr) {
    throw std::runtime_error("pop_from_stack() called on empty stack");
    return nullptr;
  }
  std::unique_ptr<ParserTokenChunk> ptc =
      std::make_unique<ParserTokenChunk>(*stack->head->ptc);
  OperatorStackNode *temp = stack->head;
  stack->head = temp->next;
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
  if (this->val->type == ParserToken::INT) {
    std::cout << token_to_string(this->val->type) << " "
              << std::get<int>(this->val->value) << std::endl;
  } else if (this->val->type == ParserToken::DOUBLE) {
    std::cout << token_to_string(this->val->type) << " "
              << std::get<double>(this->val->value) << std::endl;
  } else {
    std::cout << token_to_string(this->val->type) << " "
              << std::get<std::string>(this->val->value) << std::endl;
  }
  if (this->first_child) {
    this->first_child->print(spaces + 2);
  }
  if (this->next_sibling) {
    this->next_sibling->print(spaces);
  }
}

PTNode *PTNode::get_first_child() {
  return (this->first_child == nullptr) ? nullptr : this->first_child;
}

PTNode *PTNode::get_next_sibling() {
  return (this->next_sibling == nullptr) ? nullptr : this->next_sibling;
}

const std::string PTNode::get_type() {
  return token_to_string(this->val->type);
}

void output_nodes_recursively(const int spaces, PTNode *node,
                              std::stringstream &ss) {
  if (node == nullptr) {
    return;
  }
  for (int i = 0; i < spaces; i++) {
    ss << " ";
  }
  ss << node->get_type() << std::endl;
  output_nodes_recursively(spaces + 2, node->get_first_child(), ss);
  output_nodes_recursively(spaces, node->get_next_sibling(), ss);
}

std::string Parser::output_tree_as_str() { return this->head->output(); }

// return the parse tree as a string
std::string PTNode::output() {
  std::stringstream ss;
  output_nodes_recursively(0, this, ss);
  return ss.str();
}

PTNode* PTNode::add_child(ParserTokenChunk &tok) {
  if (this->last_child != nullptr) {
    this->last_child->add_sibling(tok);
    this->last_child = this->last_child->next_sibling;
  } else {
    this->first_child = new PTNode(tok);
    this->first_child->val->value = tok.value;
    this->first_child->val->type = tok.type;
    this->last_child = this->first_child;
  }
  return this;
}

PTNode* PTNode::add_sibling(ParserTokenChunk &tok) {
  if (this->next_sibling != nullptr) {
    this->next_sibling->add_sibling(tok);
  } else {
    this->next_sibling = new PTNode(tok);
    ;
    this->next_sibling->prev_sibling = this;
  }
  return this;
}

void PTNode::add_sibling(PTNode *sibling) {
  if (this->next_sibling != nullptr) {
    this->next_sibling->add_sibling(sibling);
  } else {
    this->next_sibling = sibling;
    if (sibling != nullptr) {
      sibling->prev_sibling = this;
    }
  }
}

PTNode* PTNode::add_child(PTNode *child) {
  if (this->last_child != nullptr) {
    this->last_child->add_sibling(child);
    this->last_child = this->last_child->next_sibling;
  } else {
    this->first_child = child;
    this->first_child = child;
    this->last_child = this->first_child;
  }
  return this ;
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
    throw std::runtime_error("parse_stmts() expects a left brace, statements "
                             "without braces are not supported yet");
    return nullptr;
  }
  PTNode *braces = new PTNode(this->ptcs.left_brace);
  this->next();
  while (this->get().type != Token::RIGHTBRACE) {
    PTNode *stmt = this->parse_stmt();
    braces->add_child(stmt);
  }
  this->next();
  stmts->add_child(braces);
  stmts->add_child(new PTNode(this->ptcs.right_brace));
  return stmts;
}

void Parser::parse_body(PTNode *start_token) {
  while (this->get().type != Token::END) {
    PTNode *stmt = this->parse_stmt();
    if (stmt == nullptr) {
      throw std::runtime_error("stmt is somehow null");
    }
    start_token->add_child(stmt);
  }
}

bool is_type(const Token tok) {
  return (tok == Token::INTK || tok == Token::BOOLK || tok == Token::STRINGK ||
          tok == Token::CHARK);
}

// Type <identifier>
PTNode *Parser::parse_variable() {
  PTNode *variable = new PTNode(this->ptcs.variable);
  if (!is_type(this->get().type)) {
    throw std::runtime_error("parse_variable() expects a type");
  }
  ParserTokenChunk type = {token_to_parser_token(this->get().type),
                           this->get().value};
  variable->add_child(type);
  this->next();
  if (this->get().type != Token::IDENTIFIER) {
    throw std::runtime_error("parse_variable() expects an identifier");
  }
  ParserTokenChunk ident = {token_to_parser_token(this->get().type),
                            this->get().value};
  variable->add_child(ident);
  return variable;
}

// ( <variable> (, <variable>*) )
PTNode *Parser::parse_formal() {
  PTNode *formal = new PTNode(this->ptcs.formal);
  if (this->get().type != Token::LEFTPARENTHESIS) {
    throw std::runtime_error("formal expects a left parenthesis");
  }
  this->next();
  formal->add_child(this->ptcs.left_paren);
  while (this->get().type != Token::RIGHTPARENTHESIS) {
    if (this->get().type == Token::COMMA) {
      this->next();
      continue;
    }
    PTNode *variable = this->parse_variable();
    if (variable == nullptr) {
      throw std::runtime_error("formal expects a left parenthesis");
    }
    formal->add_child(variable);
    this->next();
  }
  return formal;
}

// ( <expr> (, <expr>)* )
PTNode *Parser::parse_factor() {
  assert(this->get().type != Token::LEFTPARENTHESIS);
  PTNode *factor = new PTNode(this->ptcs.factor);
  factor->add_child(this->ptcs.left_paren);
  this->next();
  while (this->get().type != Token::RIGHTPARENTHESIS) {
    if (this->get().type == Token::COMMA) {
      this->next();
      continue;
    }
    PTNode *expr = this->parse_expr();
    if (expr == nullptr) {
      throw std::runtime_error("factor expects an expr");
    }
    factor->add_child(expr);
  }
  factor->add_child(this->ptcs.right_paren);
  return factor;
}

// <fn> <identifier> : <type> (formal) <stmts>
PTNode *Parser::parse_fn_decl() {
  PTNode *fn_decl{new PTNode(this->ptcs.fn_decl)};
  fn_decl->add_child(this->ptcs.fn);
  this->next();
  ParserTokenChunk ident_ptc = {token_to_parser_token(this->get().type),
                                this->get().value};
  if (ident_ptc.type != ParserToken::IDENTIFIER) {
    throw std::runtime_error(
        "Function name in function declaration is not an identifier");
  }
  this->next();
  PTNode *ident = new PTNode(ident_ptc);
  fn_decl->add_child(ident);
  if (this->get().type != Token::COLON) {
    Error("Invalid function declaration syntax").logError();
  }
  fn_decl->add_child(this->ptcs.colon);
  this->next();
  if (!is_type(this->get().type)) {
    Error("Invalid type specifier in function declaration").logError();
  }
  ParserTokenChunk fn_type_ptc = {token_to_parser_token(this->get().type), ""};
  fn_decl->add_child(fn_type_ptc);
  this->next();
  fn_decl->add_child(this->parse_formal());
  this->next();
  PTNode *block = this->parse_stmts();
  if (block == nullptr) {
    Error("function declaration body is undefined:").logError();
  }
  fn_decl->add_child(block);
  return fn_decl;
}

// ! <identifier>  (<factor>) ;
PTNode *Parser::parse_fn_call() {
  PTNode *fn_call = new PTNode(this->ptcs.fn_call);
  fn_call->add_child(this->ptcs.exclam);
  this->next();
  ParserTokenChunk ident_ptc = {token_to_parser_token(this->get().type),
                                this->get().value};
  if (ident_ptc.type != ParserToken::IDENTIFIER) {
    throw std::runtime_error(
        "Function name in function call is not an identifier");
  }
  this->next();
  if (this->get().type != Token::LEFTPARENTHESIS) {
    throw std::runtime_error("Function call expects a left parenthesis");
  }
  PTNode *factor = this->parse_factor();
  fn_call->add_child(factor);
  this->next();
  if (this->get().type == Token::SEMICOLON) {
    fn_call->add_child(this->ptcs.semicolon);
  }
  this->next();
  return fn_call;
}

PTNode *Parser::parse_stmt() {
  PTNode *stmt{new PTNode(this->ptcs.stmt)};
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
  case Token::FN:
    stmt->add_child(this->parse_fn_decl());
    break;
  case Token::EXCLAIM:
    if (this->peek().type == Token::IDENTIFIER) {
      stmt->add_child(this->parse_fn_call());
    } else
      throw std::runtime_error("Unknown symbol: !");
    break;
  case Token::IDENTIFIER:
    if (this->peek().type == Token::ASSIGN) {
      stmt->add_child(this->parse_assignment());
    } else {
      throw std::runtime_error("parse() expects a variable declaration");
    }
    break;
  default:
    throw std::runtime_error("Stmt type not recognized");
    break;
  }
  return stmt;
}

PTNode *Parser::parse_assignment() {
  PTNode *assignment = new PTNode(this->ptcs.assignstmt);
  ParserTokenChunk ident = {ParserToken::IDENTIFIER, this->get().value};
  PTNode *assign = new PTNode(this->ptcs.assign);
  assignment->add_child(new PTNode(ident));
  this->next();
  assignment->add_child(assign);
  this->next();
  PTNode *expr = this->parse_expr();
  assert(expr != nullptr);
  assert(this->get().type == Token::SEMICOLON);
  assignment->add_child(expr);
  assignment->add_child(this->ptcs.semicolon);
  this->next();
  return assignment;
}

std::unique_ptr<ParserTokenChunk> &PTNode::get_val() { return this->val; }

bool is_operator(PTNode *node) {
  return (node->get_val()->type == ParserToken::ADD ||
          node->get_val()->type == ParserToken::SUBTRACT ||
          node->get_val()->type == ParserToken::MULTIPLY ||
          node->get_val()->type == ParserToken::DIVIDE);
}

struct RPNStack {
  PTNode *head = nullptr;
  RPNStack *next = nullptr;
};

PTNode *convert_RPN_to_tree(std::unique_ptr<OutputQueue> &postfix_queue) {
  PTNode *root = nullptr;
  std::stack<PTNode *> rpn_stack;

  // convert to rpn_ to tree format
  // 0. make a rpn_stack that holds all the nodes from postfix_queue
  // 1. keep popping nodes from postfix_queue
  // 2. if an operator is encountered, then pop the last two
  // nodes from rpn_stack
  // 3. return a new node containing the expr back into the rpn_stack

  PTNode *node = nullptr;

  do {
    node = pop_from_output_queue(postfix_queue).release();
    if (is_operator(node) || node->get_val()->type == ParserToken::BINOP) {
      PTNode *right = rpn_stack.top();
      assert(right != nullptr);
      rpn_stack.pop();
      PTNode *left = rpn_stack.top();
      assert(left != nullptr);
      rpn_stack.pop();

      PTNode *sub_expr_node = new PTNode({ParserToken::BINOP, ""});
      sub_expr_node->add_child(left);
      sub_expr_node->add_child(right);
      sub_expr_node->add_child(node);
      rpn_stack.push(sub_expr_node);
    } else {
      rpn_stack.push(node);
    }
  } while (postfix_queue != nullptr && postfix_queue->head != nullptr);
  
  
  return rpn_stack.top();
}

PTNode *Parser::parse_expr(bool is_outer_expr) {
  PTNode *expr{new PTNode(this->ptcs.expr)};
  PTNode *parens{nullptr};
  PTNode *expression{nullptr};
  PTNode *temp{nullptr};
  bool is_expr{false};
  TokenChunk current;
  ParserTokenChunk ptc;
  std::unique_ptr<OutputQueue> output_q = std::make_unique<OutputQueue>();
  auto op_stack{std::make_unique<OperatorStack>()};
  if (this->get().type == Token::LEFTPARENTHESIS) {
    this->next();
    parens = new PTNode(this->ptcs.left_paren);
  }
  expression = (parens == nullptr) ? expr : parens;
  current = this->get();
  while (!(is_outer_expr && current.type == Token::RIGHTPARENTHESIS) &&
         (current.type != Token::SEMICOLON && current.type != Token::COMMA)) {
    ptc = tc_to_ptc(current);
    switch (current.type) {
    case Token::LEFTPARENTHESIS:
      is_expr = true;
      temp = this->parse_expr(is_outer_expr = false);
      assert(temp != nullptr);
      expression->add_child(temp);
      ptc = {ParserToken::EXPR, ""};
      add_sym_to_output_queue(output_q, ptc);
      break;
    case Token::INT:
    case Token::DOUBLE:
    case Token::STRING:
    case Token::IDENTIFIER:
    case Token::CHAR:
      add_sym_to_output_queue(output_q, ptc);
      break;
    default:
      // operators
      while (op_stack->head != nullptr &&
             token_type_to_precedence(op_stack->head->ptc->type) >=
                 token_type_to_precedence(ptc.type)) {
        auto op = *pop_from_stack(op_stack);
        PTNode *op_node = new PTNode({ParserToken::BINOP, ""});
        op_node->add_child(op);
        add_node_to_output_queue(output_q, op_node);
      }
      add_op_to_stack(op_stack, ptc);
      break;
    }
    // NOTE: temp fix: look at next() behavior for parse_expr
    // this is a hack to prevent the parser from skipping tokens
    if (!is_expr) {
      this->next();
    } else
      is_expr = false;
    current = this->get();
  }
  while (op_stack->head != nullptr) {
    add_sym_to_output_queue(output_q, *pop_from_stack(op_stack));
  }
  auto expr_nodes = convert_RPN_to_tree(output_q);
  expr->add_child(expr_nodes);

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
  assert(cond != nullptr);
  PTNode *block = this->parse_stmts();
  if (block == nullptr) {
    throw std::runtime_error("parse_if_stmt() expects block");
  }
  if_stmt->add_child(cond);
  if_stmt->add_child(block);
  return if_stmt;
}

// <while> ( <expr> ) <stmts>
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

// <type> <identifier> [= <expr>] ;
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
  this->parse_body(this->head);
  ParserTokenChunk end = {ParserToken::END, ""};
  this->head->add_sibling(end);
  head.reset(this->head);
}

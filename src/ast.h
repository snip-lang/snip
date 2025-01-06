#ifndef AST_H
#define AST_H

#include "semantic.h"

class ASTRoot {
public:
  ASTRoot();
  ~ASTRoot();
  void add_child(PTNode *);
  void print(const int);
  std::string output();
  PTNode *get_first_child();
  PTNode *get_next_sibling();
private:
  PTNode *first_child = nullptr;
  PTNode *last_child = nullptr;
  PTNode *next_sibling = nullptr;
  PTNode *prev_sibling = nullptr;
};

#endif

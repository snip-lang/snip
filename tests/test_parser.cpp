#include "../src/parser.h"

void test_parser_tree() {
  ParserTokenChunk p1 = {ParserToken::IF, 0};
  ParserTokenChunk p2 = {ParserToken::AND, 0};
  ParserTokenChunk p3 = {ParserToken::OR, 0};
  ParserTokenChunk p4 = {ParserToken::ELSE, 0};
  PTNode pn1(p1);
  pn1.add_child(p2);
  pn1.add_child(p3);
  pn1.add_child(p4);
  pn1.print(0);
}

int main(int argc, char *argv[]) {
  test_parser_tree();
  return 0;
}

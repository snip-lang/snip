#include "../src/helper.h"
#include "../src/lexer.h"
#include "../src/parser.h"
#include "./test.h"
#include <iostream>
#include <memory>

void test_parser_tree() {
  ParserTokenChunk p1 = {ParserToken::IF, 0};
  ParserTokenChunk p2 = {ParserToken::AND, 0};
  ParserTokenChunk p3 = {ParserToken::OR, 0};
  ParserTokenChunk p4 = {ParserToken::ELSE, 0};
  PTNode pn1(p1);
  pn1.add_child(p2);
  pn1.add_child(p3);
  pn1.add_child(p4);
  std::string a = pn1.output();
  TestCase("ParserTree output", a, "IF\n  AND\n  OR\n  ELSE\n").checkResult();
}

std::string lex_and_parse_input(std::string input) {
  std::unique_ptr<TokenChunk[]> token_stack = nullptr;
  Lexer lex(input);
  lex.tokenize(token_stack);

  Parser parser(token_stack);
  std::unique_ptr<PTNode> parsed_tokens = nullptr;
  parser.parse(parsed_tokens);

  return parser.output_tree_as_str();
}

void test_statement_types() {
  std::string var_dec_int = "int a = 1; ";
  std::string var_dec_int_expected = "START\n"
                                     "  STMT\n"
                                     "    VARDECL\n"
                                     "      INTK\n"
                                     "      IDENTIFIER\n"
                                     "      ASSIGN\n"
                                     "      EXPR\n"
                                     "        INT\n"
                                     "      SEMICOLON\n"
                                     "END\n";
  std::string var_dec_int_rec = lex_and_parse_input(var_dec_int);
  TestCase("parse if statement (int)", var_dec_int_expected, var_dec_int_rec)
      .checkResult();
  std::string var_dec_char = "char c = 'c'; ";

  std::string var_dec_char_expected = "START\n"
                                      "  STMT\n"
                                      "    VARDECL\n"
                                      "      CHARK\n"
                                      "      IDENTIFIER\n"
                                      "      ASSIGN\n"
                                      "      EXPR\n"
                                      "        CHAR\n"
                                      "      SEMICOLON\n"
                                      "END\n";
  std::string var_dec_char_rec = lex_and_parse_input(var_dec_char);
  TestCase("parse if statement (char)", var_dec_char_expected, var_dec_char_rec)
      .checkResult();
  std::string var_dec_str = "str a = \";\"; ";
  std::string var_dec_str_expected = "START\n"
                                     "  STMT\n"
                                     "    VARDECL\n"
                                     "      STRINGK\n"
                                     "      IDENTIFIER\n"
                                     "      ASSIGN\n"
                                     "      EXPR\n"
                                     "        STRING\n"
                                     "      SEMICOLON\n"
                                     "END\n";
  std::string var_dec_str_rec = lex_and_parse_input(var_dec_str);
  TestCase("parse if statement (string)", var_dec_str_expected, var_dec_str_rec)
      .checkResult();
  std::string if_stmt = "if (a) { b = 1; }";
  std::string if_stmt_expected = "START\n"
                                 "  STMT\n"
                                 "    IF\n"
                                 "      EXPR\n"
                                 "        IDENTIFIER\n"
                                 "      STMTS\n"
                                 "        STMT\n"
                                 "          ASSIGN\n"
                                 "            IDENTIFIER\n"
                                 "            EXPR\n"
                                 "              INT\n"
                                 "END\n";
  std::string if_stmt_rec = lex_and_parse_input(if_stmt);
  TestCase("parse if statement", if_stmt_expected, if_stmt_rec).checkResult();
  std::string while_stmt = "while (a) { b = 1; } ";
  std::string exprs = "(a + (b + (1/2))) * c / d - e; ";
}

int main() {
  test_parser_tree();
  test_statement_types();
  return 0;
}

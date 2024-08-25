#include "helper.h"
#include "lexer.h"
#include "parser.h"

int main(int argc, char *argv[]) {
  std::string filename = "input.snip";
  std::string test_filename = "output.txt";
  std::string parse_string = "";
  bool isTest = get_flags(argc, argv, filename, parse_string, test_filename);
  std::unique_ptr<TokenChunk[]> token_stack = nullptr;
  std::string lines = readFile(filename);
  Lexer lex((parse_string.empty() ? lines : parse_string));
  lex.tokenize(token_stack);

  if (isTest) {
    writeFile(test_filename, print_lexed_tokens_test(token_stack));
  } else {
    // print_lexed_tokens(token_stack);
  }

  Parser parser(token_stack);
  std::unique_ptr<PTNode> parsed_tokens = nullptr;
  parser.parse(parsed_tokens);
  return 0;
}

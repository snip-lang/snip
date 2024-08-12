#include "helper.h"
#include "lexer.h"
#include "parser.h"
#include <cstring>
#include <fstream>
#include <iostream>
#include <sstream>
#include <string>
#include <string_view>
#include <tuple>
#include <vector>

std::string readFile(const std::string &filename) {
  std::ifstream file(filename);
  if (!file.is_open()) {
    std::cerr << "Unable to open source file" << std::endl;
  }
  std::string source((std::istreambuf_iterator<char>(file)),
                     std::istreambuf_iterator<char>());
  file.close();
  return source;
}

void writeFile(const std::string &filename, const std::string &output) {
  std::ofstream file(filename);
  if (!file.is_open()) {
    std::cerr << "Unable to open output file" << std::endl;
  }
  file << output;
  file.close();
}

bool get_flags(const int &argc, char *argv[], std::string &filename,
               std::string &parseString, std::string &test_string) {
  if (argc == 1)
    return 0;
  if (argc == 2) {
    filename = argv[1];
    return 0;
  } else if (argc == 3) {
    if (std::strcmp(argv[1], "-e")) {
      parseString = argv[2];
    }
  }
  int ind = 0;
  char *currentFlag;
  char *currentOption;
  bool isTest{false};
  for (int count{0}; count < argc; count++) {
    if (count % 2) {
      currentFlag = argv[count];
    } else {
      if (strcmp(currentFlag, "-e") == 0) {
        parseString = argv[count];
      } else if (strcmp(currentFlag, "-t") == 0) {
        isTest = true;
        test_string = argv[count];
      } else {
        filename = argv[count];
      }
    }
  }
  return isTest;
}

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
    print_lexed_tokens(token_stack);
  }

  Parser parser(token_stack);
  std::unique_ptr<PTNode> parsed_tokens = nullptr;
  parser.parse(parsed_tokens);
  return 0;
}

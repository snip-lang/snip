#include "lexer.h"
#include <fstream>
#include <iostream>
#include <string>
#include <string_view>
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

int main(int argc, char *argv[]) {
  std::string filename = "input.snip"; // default filename
  if (argc == 1) {
    filename = argv[0];
  }
  std::string lines = readFile(filename);
  Lexer lex(lines);
  return 0;
}

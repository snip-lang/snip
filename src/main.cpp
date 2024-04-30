#include <iostream>
#include "lexer.h"
#include <fstream>
#include <string>
#include <vector>

int main (int argc, char *argv[]) {
  std::string filename = "input.snip"; 
  if (argc == 1) {
    filename = "input.snip"; 
  }
  std::ifstream ifile(filename);   
  if (!ifile.is_open()) {
    std::cerr << "Unable to open file" << std::endl; 
    return -1; 
  }
  std::string line; 
  std::vector<std::string> lines; 
  while (std::getline(ifile, line)) {
    lines.push_back(line);
    std::cout << line << std::endl;
  }
  ifile.close(); 

  Lexer lex(lines);
  return 0;
}

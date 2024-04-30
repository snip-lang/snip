#include <iostream>
#include <string>
#include <vector>

#include "./lexer.h"

bool isAlpha(char c) noexcept {
  return ((c > 96 && c < 123) || (c > 64 && c < 90));
}

bool isSpace(char c) noexcept {
  switch (c) {
  case ' ':
  case '\t':
  case '\n':
    return true;
  default:
    return false;
    break;
  }
}

bool isComment(char c) noexcept { return (c == '#'); }

bool isNum(char c) noexcept { return (c > 47 && c < 58); }

// takes in a list of strings, stores it as a list of tokens
Lexer::Lexer(std::vector<std::string> lines) {
  for (std::string line : lines) {
    int ind = 0;
    std::string currentToken; 
    while (true) {
      if (isSpace(line[ind])) {
        if (currentToken != "") {
          // check for different "regexes", then push them into the token list
          int cnt = 0; 
        }
      }
      currentToken += line[ind];
      ind++; // strip out all whitespace
    }
  }
}

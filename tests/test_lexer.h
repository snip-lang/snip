#ifndef TEST_LEXER
#define TEST_LEXER
#include "../src/globals.h"
#include <memory>
#include <string>

class TestCase {
public:
  TestCase(const std::string &, const std::string &, const std::string &);
  TestCase(const std::string &, const std::string &,
           std::unique_ptr<Token[]> &);
  void checkResult() const;
  void runTests() ;

private:
  std::string name;
  std::string expected;
  std::string result_recieved;
};

#endif // !TEST_LEXER

#include "test.h"
#include <cstring>
#include <fstream>
#include <iostream>

TestCase::TestCase(const std::string &test_name,
                   const std::string &expected_result,
                   const std::string &result_recieved)
    : name(test_name), expected(expected_result),
      result_recieved(result_recieved) {}

void TestCase::checkResult() const {
  if (this->expected != this->result_recieved) {
    std::cout << ""
              << "============================================================="
              << "=========" << std::endl
              << "FAIL: " << this->name << std::endl
              << "Expected: " << std::endl
              << this->expected << std::endl
              << "Received: " << std::endl
              << this->result_recieved << std::endl;
  } else {
    std::cout << "PASS: " << this->name << std::endl;
  }
}

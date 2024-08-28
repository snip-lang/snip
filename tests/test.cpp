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
              << "=========" << "FAIL: " << this->name
              << "Expected: " << this->expected
              << "Received: " << this->result_recieved << std::endl;
  } else {
    std::cout << "PASS: " << this->name << std::endl;
  }
}

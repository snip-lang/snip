#include "error.h"

void Error::logError() const {
  std::cerr << "Error: " << message_ << " (code: " << code_ << ")"
            << " in file " << file_ << " at line " << line_ << std::endl;
  if (severity_ == Severity::CRITICAL) {
    std::cerr << "This is a critical error." << std::endl;
  } else if (severity_ == Severity::WARN) {
    std::cerr << "This is a warning." << std::endl;
  }
}

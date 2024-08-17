#include <iostream>
#include <string>

enum class Severity { WARN, CRITICAL, ERROR };
class Error {

public:
  Error(const std::string &message, int code = 0,
        Severity severity = Severity::ERROR, const std::string &file = "",
        int line = 0)
      : message_(message), code_(code), severity_(severity), file_(file),
        line_(line) {}

  // Override what() method to provide the error message
  const char *what() const noexcept { return message_.c_str(); }

  int code() const noexcept { return code_; }

  Severity severity() const noexcept { return severity_; }

  const std::string &file() const noexcept { return file_; }

  int line() const noexcept { return line_; }

  void logError() const;

private:
  std::string message_;
  int code_;
  Severity severity_;
  std::string file_;
  int line_;
};

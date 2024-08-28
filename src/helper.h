#include "globals.h"
#include "parser.h"
#include <memory>

const std::string token_to_string(const Token &tok);
const std::string token_to_string(const ParserToken &tok);

void print_lexed_tokens(std::unique_ptr<TokenChunk[]> &);
void print_parsed_tokens(std::unique_ptr<PTNode> &token_stack);
std::string print_lexed_tokens_test(std::unique_ptr<TokenChunk[]> &);

std::string readFile(const std::string &filename);
void writeFile(const std::string &filename, const std::string &output);
bool get_flags(const int &argc, char *argv[], std::string &filename,
               std::string &parseString, std::string &test_string);

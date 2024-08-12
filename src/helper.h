#include "globals.h"
#include <memory>

const std::string token_to_string(const Token &tok);
const std::string token_to_string(const ParserToken &tok);

void print_lexed_tokens(std::unique_ptr<TokenChunk[]> &);
std::string print_lexed_tokens_test(std::unique_ptr<TokenChunk[]> &);

#include "ast.h"
#include <cassert>
#include "globals.h"

extern bool is_type(const Token tok);
extern Token parser_token_to_token(const ParserToken& tok);
extern void get_variant_value_and_assign_to(ParserTokenChunk& tok,
																						SymbolTableEntryValue& var_to_assign);



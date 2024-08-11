# Language Specifications

## Syntax

```
<start> ::= <stmt> | <end>
<var_decl> ::= <identifier> <type> (= <expr>)? <semicolon>
<fn_decl> ::= "fn" <identifier> <lparen><rparen> <type> {<stmt>} end
<if_stmt> ::= <if> <expr> then <lbrace> <stmt> <rbrace> end
<while_stmt> ::= "while" <expr> do <lbrace> <stmt> <rbrace> end

<stmt> ::= <stmt> <stmt>
        | <if_stmt>
        | <var_decl>
        | <while_stmt>
        | <fn_decl>
        | <expr> <semicolon>

<semicolon> ::= ";"
<if> ::= "if"
<while> ::= "while"
<and> ::= "and"
<or> ::= "or"
<not> ::= "not"

<expr> ::=
        | <digit>
        | <unary_op> <expr>
        | <expr> <binary_op> <expr>
        | <array>

<array> ::=
        | <lbracket> (<expr> <comma>)* <rbracket>

<binary_op> ::=
        | "+"
        | "-"
        | "/"
        | ">"
        | "<"
        | ">="
        | "<="
        | "=="
        | "/="
        | <and>
        | <or>

<unary_op> ::=
        | "++"
        | "--"
        | "!"

<identifier> ::= <char> (<char> | <digit> | "_")*
<type> ::= "int" "char" "bool" "string"

<digit> ::= "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
<char> ::= [A-Z][a-z]
```

### Keywords

- Data Types: `int`, `char`, `string`, `bool` , `double`,

- Keywords:
  `const` `fn` `if`/`elif`/`else`
  `while` `for` `return`
  `break`
  `class` `this` `new`
  `true` `false`

- Arithmetic: `+`, `-`, `*`, `/`
- Comparison: `==`, `!=`, `<`, `>`, `<=`, `>=`
- Logical: `&&`, `||`, `!`

- Identifier regex: [a-zA-z_]

- Comments: #

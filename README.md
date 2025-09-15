
# Snip Lang

Snip is a small, interpreted programming language built with C#.

### Grammar

`snip
T -> F MUL F
T -> F DIV F
F -> (E) | NUMBER | VAR
`

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Building

1. **Clone the repository:**

   ```bash
   git clone https://github.com/chess10kp/snip.git
   cd snip
   ```

2. **Build the project:**

   ```bash
   dotnet build
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

### 1. Lexer (Lexical Analysis)

The lexer is responsible for taking the raw source code as a string and breaking it down into a series of tokens.

- **Location:** `Lexer/Lexer.cs`
- **Tasks:**
  - Define a `Token` struct or class to represent a token (e.g., with properties for `Type` and `Literal`).
  - Implement the `Lexer` class to read the source code character by character.
  - Add logic to recognize and create tokens for:
    - Keywords (e.g., `let`, `fn`, `if`, `else`, `return`)
    - Identifiers (e.g., variable names)
    - Literals (e.g., integers, strings)
    - Operators (e.g., `+`, `-`, `*`, `/`, `=`, `==`, `!=`)
    - Delimiters (e.g., `(`, `)`, `{`, `}`, `,`, `;`)

### 2. AST (Abstract Syntax Tree)

The AST is a tree representation of the source code. Each node in the tree represents a construct in the code.

- **Location:** `AST/`
- **Tasks:**
  - Define a base `AstNode` class.
  - Create concrete classes for each type of AST node, such as:
    - `ProgramNode`: The root of the AST.
    - `StatementNode`: Represents a statement (e.g., `let`, `return`).
    - `ExpressionNode`: Represents an expression (e.g., `5`, `x + 5`).
    - `IdentifierNode`, `IntegerLiteralNode`, `InfixExpressionNode`, etc.

### 3. Parser

The parser takes the tokens from the lexer and builds an AST.

- **Location:** `Parser/Parser.cs`
- **Tasks:**
  - Implement the `Parser` class to take a `Lexer` as input.
  - Write parsing functions for each type of statement and expression.
  - Use a top-down parsing approach (e.g., Pratt parsing for expressions).
  - Handle operator precedence and associativity.
  - Report parsing errors when the code is syntactically incorrect.

### 4. Evaluator

The evaluator walks the AST and evaluates the code.

- **Location:** `Evaluator/Evaluator.cs`
- **Tasks:**
  - Implement the `Evaluator` class to take an `AstNode` as input.
  - Write an `Eval` method that recursively evaluates each node of the AST.
  - Implement an `Environment` class to keep track of variable bindings.
  - Handle the evaluation of:
    - Literals
    - Infix and prefix expressions
    - `if/else` expressions
    - Function definitions and calls
    - `let` statements
    - `return` statements

### 5. REPL (Read-Eval-Print Loop)

The REPL allows you to interact with the interpreter from the command line.

- **Location:** `Program.cs`
- **Tasks:**
  - Create a loop that reads input from the user.
  - For each line of input:
    1. Create a `Lexer` and `Parser`.
    2. Parse the input to create an AST.
    3. Evaluate the AST with the `Evaluator`.
    4. Print the result to the console.

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

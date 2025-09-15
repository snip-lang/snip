# Snip Interpreter TODO List

This document outlines the tasks required to complete the Snip interpreter.

## 🎯 High-Level Goal

The goal is to create a fully functional interpreter for a subset of TypeScript using C#.

## 🚀 Current Status

-   **Lexer**: Mostly complete. It can tokenize a wide range of TypeScript syntax.
-   **Parser**: Not started.
-   **AST**: Only the base `AstNode` class exists.
-   **Evaluator**: Not started.
-   **CLI**: Basic file reading is implemented.

##  immediate Next Steps

The most critical task is to start implementing the **Parser**. The parser will take the tokens from the lexer and build an Abstract Syntax Tree (AST). Before you can do that, you need to define the structure of the AST.

1.  **Define AST Nodes**: In the `AST/` directory, create classes for the different types of nodes in the AST. Start with the basics:
    -   `ProgramNode`: The root of the AST.
    -   `LetStatementNode`: For `let` statements.
    -   `IdentifierNode`: For variable names.
    -   `ExpressionStatementNode`: For statements that are just expressions.
    -   `IntegerLiteralNode`: For integer values.

2.  **Implement the Parser**: In `Parser/Parser.cs`, start building the parser.
    -   The `Parser` class should take a `Lexer` in its constructor.
    -   Create a `ParseProgram()` method that will be the entry point for parsing.
    -   Implement methods to parse different types of statements, starting with `let` statements.

## Detailed TODO List

### 1. AST (Abstract Syntax Tree)

The AST is a tree representation of the source code.

-   [ ] **Define Base Nodes**:
    -   [ ] Create a base `StatementNode` class that inherits from `AstNode`.
    -   [ ] Create a base `ExpressionNode` class that inherits from `AstNode`.

-   [ ] **Implement Concrete Statement Nodes**:
    -   [ ] `LetStatementNode`
    -   [ ] `ReturnStatementNode`
    -   [ ] `ExpressionStatementNode`
    -   [ ] `BlockStatementNode`

-   [ ] **Implement Concrete Expression Nodes**:
    -   [ ] `IdentifierNode`
    -   [ ] `IntegerLiteralNode`
    -   [ ] `StringLiteralNode`
    -   [ ] `BooleanLiteralNode`
    -   [ ] `PrefixExpressionNode` (e.g., `!`, `-`)
    -   [ ] `InfixExpressionNode` (e.g., `+`, `-`, `*`, `/`)
    -   [ ] `IfExpressionNode`
    -   [ ] `FunctionLiteralNode`
    -   [ ] `CallExpressionNode`

### 2. Parser

The parser takes the tokens from the lexer and builds an AST.

-   [ ] **Implement Parser Class**:
    -   [ ] Add a constructor that takes a `Lexer` and initializes the current and peek tokens.
    -   [ ] Implement a `NextToken()` method to advance the tokens.

-   [ ] **Implement Statement Parsing**:
    -   [ ] `ParseProgram()`: The main loop that parses all statements.
    -   [ ] `ParseStatement()`: A helper method to determine which statement parsing method to call based on the current token.
    -   [ ] `ParseLetStatement()`
    -   [ ] `ParseReturnStatement()`
    -   [ ] `ParseExpressionStatement()`

-   [ ] **Implement Expression Parsing**:
    -   [ ] Use a Pratt parser for handling operator precedence.
    -   [ ] Register parsing functions for each token type.
    -   [ ] `ParseExpression(precedence)`: The core of the Pratt parser.
    -   [ ] Implement parsing for:
        -   [ ] Identifiers
        -   [ ] Literals (integers, strings, booleans)
        -   [ ] Prefix expressions
        -   [ ] Infix expressions
        -   [ ] Grouped expressions (`(` `)`)
        -   [ ] `if` expressions
        -   [ ] Function literals
        -   [ ] Call expressions

-   [ ] **Error Handling**:
    -   [ ] Add a list of errors to the `Parser` class.
    -   [ ] Report meaningful errors when the syntax is incorrect.

### 3. Connect Lexer and Parser

-   [ ] In `Program.cs`, after the lexer is created, create a `Parser` instance with the lexer.
-   [ ] Call the `ParseProgram()` method on the parser.
-   [ ] Print any parsing errors that occurred.

### 4. Evaluator

The evaluator walks the AST and evaluates the code.

-   [ ] **Implement Environment**:
    -   [ ] Create an `Environment` class to store variable bindings.
    -   [ ] It should support creating a new enclosed scope for functions.

-   [ ] **Implement Evaluator Class**:
    -   [ ] Create an `Eval` method that takes an `AstNode` and an `Environment`.
    -   [ ] Use a `switch` statement to handle different node types.

-   [ ] **Implement Evaluation Logic**:
    -   [ ] Evaluate literals (integers, strings, booleans).
    -   [ ] Evaluate prefix and infix expressions.
    -   [ ] Evaluate `if/else` expressions.
    -   [ ] Evaluate `let` statements (store variables in the environment).
    -   [ ] Evaluate identifiers (retrieve variables from the environment).
    -   [ ] Evaluate function literals and function calls.
    -   [ ] Handle `return` statements.

### 5. REPL (Read-Eval-Print Loop)

-   [ ] In `Program.cs`, create a REPL mode if no file is provided.
-   [ ] The REPL should:
    1.  Read a line of input.
    2.  Create a `Lexer` and `Parser`.
    3.  Parse the input.
    4.  Evaluate the resulting AST.
    5.  Print the result.
    6.  Loop.

## 🔮 Future Goals

-   [ ] Support for more data types (e.g., arrays, hashes).
-   [ ] More complete TypeScript feature support (e.g., classes, modules).
-   [ ] A standard library of built-in functions.
-   [ ] Better error reporting with line and column numbers.
-   [ ] A simple type checker.

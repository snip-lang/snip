# Snip Interpreter TODO List

## 🚀 Current Status

-   **Lexer**: ✅ Complete. Tokenizes keywords, identifiers, literals, operators, and delimiters.
-   **Parser**: ✅ Complete. Implements Pratt parser with operator precedence, handles all statements and expressions.
-   **AST**: ✅ Complete. All node types implemented including advanced features like classes, inheritance, and modern syntax.
-   **Evaluator**: 🟡 Mostly complete. Implements literals, expressions, control flow, functions, classes, objects, arrays. Missing some advanced features.
-   **CLI**: ✅ Complete. File reading and evaluation implemented. REPL mode available.

## ✅ Fully Implemented Features

**Core Language:**
- Literals (numbers, string, bool, null, undefined)
- Arithmetic, comparison, and logical operators with short-circuit evaluation (`&&`, `||`)
- Destructuring assignments for arrays and objects
- Variable declarations (`let`, `var`, `const`) with proper scoping
- Assignment and compound assignment operators
- If/else statements and ternary expressions
- Block statements with lexical scoping
- While, for, and do-while loops
- Switch statements
- Continue statements in loops
- Unary operators (`!`, `-`, `+`)
- Functions (declarations, calls, return statements) with closures
- Objects and arrays with property/index access
- Optional chaining (`?.`) for safe property access
- Classes with inheritance, constructors, methods, and `super` calls
- `this` expressions in methods
- Try/catch/finally blocks and throw statements
- Import/export system for modules
- Template literals
- Arrow functions

**Test Coverage:** 186 tests passing

## Priority Implementation Order

**High Priority (Essential for usability):**
- [x] REPL mode for interactive development
- [x] Standard library built-in functions (`print`, `typeof`, etc.)
- [x] Better error handling with line/column info

**Medium Priority (Completes language features):**
- [x] Switch statement evaluation
- [x] Do-while loops
- [x] Try/catch/finally blocks
- [x] Throw statements
- [x] Import/export system

**Low Priority (Advanced features):**
- [x] Arrow functions
- [x] Template literals
- [ ] Spread/rest elements in more contexts
- [ ] Labeled break/continue
- [ ] Interfaces
**Low Priority (Nice to have):**
- Classes and inheritance
- Advanced features (async, modules, etc.)
- Standard library extensions
- Performance optimizations

## Detailed TODO List

### 4. Evaluator

The evaluator walks the AST and evaluates the code.

-   [x] **Implement Environment**:
    -   [x] Create an `Environment` class to store variable bindings.
    -   [x] It should support creating a new enclosed scope for functions.

-   [x] **Implement Evaluator Class**:
    -   [x] Create an `Eval` method that takes an `AstNode` and an `Environment`.
    -   [x] Use a `switch` statement to handle different node types.

-   [x] **Implement Basic Evaluation Logic**:
    -   [x] Evaluate literals (integers, strings, booleans, null, undefined).
    -   [x] Evaluate binary expressions (arithmetic, comparison).
    -   [x] Evaluate `let` statements and assignments (store/retrieve variables).
    -   [x] Evaluate identifiers (lookup in environment).

-   [x] **Implement Advanced Evaluation Logic**:
    -   [x] Evaluate unary expressions (negation, not).
    -   [x] Evaluate `if/else` statements and conditional expressions.
    -   [x] Evaluate loops (`while`, `for`).
    -   [x] Evaluate `switch` statements.
    -   [x] Evaluate function literals and function calls.
    -   [x] Handle `return` statements.
    -   [x] Handle `continue` statements.
    -   [x] Handle array expressions and member access.
    -   [x] Handle object expressions and property access.
    -   [x] Handle block statements.
-   [x] Handle try/catch/finally blocks.
-   [x] Handle class declarations and instantiation.
-   [x] Handle template literals.
-   [ ] Handle spread/rest elements in more contexts.
-   [x] Evaluate `do-while` loops.
-   [x] Evaluate arrow functions.
-   [x] Handle `throw` statements.
-   [ ] Handle labeled `break`/`continue`.

### 5. Connect Parser and Evaluator

-   [x] In `Program.cs`, after parsing, create an `Evaluator` instance.
-   [x] Call the `Eval` method on the parsed AST.
-   [x] Handle and display evaluation errors with proper formatting and line/column info.

### 6. REPL (Read-Eval-Print Loop)

-   [x] In `Program.cs`, create a REPL mode if no file is provided.
-   [x] The REPL should:
    1.  Read a line of input.
    2.  Create a `Lexer` and `Parser`.
    3.  Parse the input.
    4.  Evaluate the resulting AST.
    5.  Print the result.
    6.  Loop.
-   [x] Maintain persistent environment across REPL sessions.
-   [ ] Handle multi-line input for blocks and functions.
-   [x] Add commands like `.exit`, `.help`, `.clear`.

### 7. Error Handling and Reporting

-   [x] Add proper error classes for evaluation errors.
-   [x] Include line and column numbers in error messages.
-   [x] Handle runtime errors (division by zero, undefined variables, etc.).
-   [x] Improve parser error messages with context.
-   [x] Add evaluation error handling in Program.cs with proper formatting.

### 8. Standard Library

-   [x] Implement built-in functions (`print`, `typeof`, `parseInt`, etc.).
-   [ ] Add math functions (`Math.abs`, `Math.random`, etc.).
-   [ ] Add string manipulation functions.
-   [ ] Add array/object utility functions.
-   [x] Initialize standard library functions in the global environment.

### 9. Advanced Language Features

-   [x] Implement closures and proper function scoping.
-   [x] Add `this` context for object methods.
-   [x] Support for `new` expressions and constructors.
-   [x] Implement prototype-based inheritance.
-   [x] Add destructuring assignments.
-   [x] Support for `const` and `var` declarations.
-   [ ] Implement labeled statements for `break`/`continue`.
-   [x] Add short-circuit evaluation for logical operators (`&&`, `||`).
-   [x] Arrow functions.
-   [x] Template literals.
-   [x] Optional chaining (`?.`).
-   [ ] Spread/rest syntax in more contexts.
-   [x] Try/catch/finally.
-   [ ] Interfaces.
-   [x] Import/export system.

## 📊 Implementation Status

Based on test coverage (186 tests passing), the following features are fully implemented and tested:
- Literals (numbers, string, bool, null, undefined)
- Arithmetic, comparison, and logical operators with short-circuit evaluation
- Destructuring assignments for arrays and objects
- Variable declarations (`let`, `var`, `const`) with proper scoping
- Assignment and compound assignment operators
- If/else statements, ternary expressions, switch statements, and do-while loops
- Block statements with lexical scoping
- While and for loops
- Continue statements
- Unary operators
- Functions with closures
- Objects and arrays with optional chaining
- Classes with inheritance
- `this` and `super` expressions
- Try/catch/finally and throw
- Import/export system
- Template literals
- Arrow functions

## 🔮 Future Goals

-   [x] **Error Handling**: Better error messages with line/column info.
-   [x] **Standard Library Extensions**: Comprehensive standard library with 12+ modules including math, string, array, object, io, types, random, datetime, file, regex, collections, itertools.
-   [ ] **Missing Language Features**: Spread/rest in more contexts, labeled break/continue, interfaces.
-   [ ] **Type System**: Add static type checking and TypeScript-like type annotations.
-   [ ] **Async/Await**: Add support for asynchronous programming.
-   [ ] **Advanced Features**: Generators, regex, decorators, etc.
-   [ ] **I/O Operations**: File system and networking support.
-   [ ] **Performance**: Optimize evaluator performance.
-   [ ] **Tooling**: Debugger, profiler, package manager, and development tools.
-   [ ] **Web Integration**: Compile to WebAssembly or integrate with web APIs.

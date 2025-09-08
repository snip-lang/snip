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

### Examples

Snip supports variables, functions, control flow, objects, arrays, and more. Here are some examples:

#### Variables and Arithmetic
```snip
let x = 5;
let y = 10;
x + y;  // Output: 15
```

#### Functions
```snip
function add(a, b) {
    return a + b;
}
add(3, 4);  // Output: 7
```

#### Control Flow
```snip
let x = 5;
if (x > 3) {
    "greater";
} else {
    "less";
}  // Output: "greater"
```

#### Objects
```snip
let person = { name: "John", age: 30 };
person.name;  // Output: "John"
```

#### Arrays
```snip
let arr = [1, 2, 3];
arr[0];  // Output: 1
```

#### Loops
```snip
let sum = 0;
for (let i = 0; i < 5; i = i + 1) {
    sum = sum + i;
}
sum;  // Output: 10
```

## Standard Library

Snip includes a comprehensive standard library with Python-inspired functions and modules.

### Global Built-in Functions

These functions are available globally without importing any modules:

#### `len(obj)` → number
Returns the length of arrays, strings, and objects.
```snip
len([1, 2, 3]);        // 3
len("hello");           // 5
len({a: 1, b: 2});      // 2
```

#### `type(obj)` → string
Returns the type of an object as a string.
```snip
type(42);               // "number"
type("hello");          // "string"
type([1, 2, 3]);        // "array"
type({a: 1});           // "object"
```

#### `str(obj)` → string
Converts any value to its string representation.
```snip
str(42);                // "42"
str(true);              // "True"
str([1, 2, 3]);         // "[1, 2, 3]"
```

#### `int(obj)` → number
Converts values to integers.
```snip
int("123");             // 123
int(45.67);             // 45
int(true);              // 1
```

#### `bool(obj)` → boolean
Converts values to booleans using Python-like truthiness.
```snip
bool(0);                // false
bool(1);                // true
bool("");               // false
bool("hello");          // true
bool([]);               // false
bool([1, 2]);           // true
```

#### `print(...args)` → null
Prints arguments to console with spaces between them.
```snip
print("Hello", "world", 42);  // Output: Hello world 42
```

#### `input(prompt?)` → string
Reads a line of input from the user.
```snip
let name = input("Enter your name: ");
```

### Standard Library Modules

#### Array Module
Functions for working with arrays.

```snip
import "array";
```

- `array.create(size, value?)` → array  
  Creates an array of given size filled with optional value.
- `array.range(start?, stop, step?)` → array  
  Creates an array with a range of numbers.
- `array.fill(arr, value)` → array  
  Fills an array with a value.
- `array.len(arr)` → number  
  Returns array length.
- `array.empty(arr)` → boolean  
  Checks if array is empty.
- `array.append(arr, value)` → array  
  Appends value to array.
- `array.extend(arr, other)` → array  
  Extends array with another array.
- `array.insert(arr, index, value)` → array  
  Inserts value at index.
- `array.remove(arr, value)` → array  
  Removes first occurrence of value.
- `array.pop(arr, index?)` → value  
  Removes and returns element at index (default: last).
- `array.clear(arr)` → array  
  Removes all elements.
- `array.index(arr, value)` → number  
  Returns index of first occurrence (-1 if not found).
- `array.contains(arr, value)` → boolean  
  Checks if array contains value.
- `array.count(arr, value)` → number  
  Counts occurrences of value.
- `array.reverse(arr)` → array  
  Reverses array in place.
- `array.sort(arr)` → array  
  Sorts array in place (lexicographically).
- `array.slice(arr, start?, end?, step?)` → array  
  Returns a slice of the array.
- `array.concat(...arrays)` → array  
  Concatenates multiple arrays.

#### String Module
Functions for string manipulation.

```snip
import "string";
```

- `string.upper(str)` → string  
  Converts to uppercase.
- `string.lower(str)` → string  
  Converts to lowercase.
- `string.capitalize(str)` → string  
  Capitalizes first character.
- `string.title(str)` → string  
  Capitalizes each word.
- `string.strip(str, chars?)` → string  
  Removes whitespace from both ends.
- `string.lstrip(str, chars?)` → string  
  Removes whitespace from left.
- `string.rstrip(str, chars?)` → string  
  Removes whitespace from right.
- `string.ljust(str, width, fillchar?)` → string  
  Left-justifies string.
- `string.rjust(str, width, fillchar?)` → string  
  Right-justifies string.
- `string.center(str, width, fillchar?)` → string  
  Centers string.
- `string.find(str, sub, start?)` → number  
  Finds substring (returns -1 if not found).
- `string.rfind(str, sub, start?)` → number  
  Finds substring from right.
- `string.replace(str, old, new)` → string  
  Replaces occurrences.
- `string.count(str, sub)` → number  
  Counts substring occurrences.
- `string.split(str, sep?)` → array  
  Splits string into array.
- `string.rsplit(str, sep?, maxsplit?)` → array  
  Splits from right with optional max splits.
- `string.join(sep, arr)` → string  
  Joins array elements with separator.
- `string.startswith(str, prefix)` → boolean  
  Checks if starts with prefix.
- `string.endswith(str, suffix)` → boolean  
  Checks if ends with suffix.
- `string.isalpha(str)` → boolean  
  Checks if all alphabetic.
- `string.isdigit(str)` → boolean  
  Checks if all digits.
- `string.isalnum(str)` → boolean  
  Checks if all alphanumeric.
- `string.isspace(str)` → boolean  
  Checks if all whitespace.

#### Math Module
Mathematical functions and constants.

```snip
import "math";
```

**Constants:**
- `math.pi` ≈ 3.14159
- `math.e` ≈ 2.71828
- `math.tau` ≈ 6.28318
- `math.inf` = ∞
- `math.nan` = NaN

**Functions:**
- `math.abs(x)` → number  
  Absolute value.
- `math.ceil(x)` → number  
  Ceiling function.
- `math.floor(x)` → number  
  Floor function.
- `math.round(x, digits?)` → number  
  Rounds to given decimal places.
- `math.max(...values)` → number  
  Maximum value.
- `math.min(...values)` → number  
  Minimum value.
- `math.pow(base, exp)` → number  
  Power function.
- `math.sqrt(x)` → number  
  Square root.
- `math.sin(x)` → number  
  Sine (radians).
- `math.cos(x)` → number  
  Cosine (radians).
- `math.tan(x)` → number  
  Tangent (radians).
- `math.asin(x)` → number  
  Arcsine.
- `math.acos(x)` → number  
  Arccosine.
- `math.atan(x)` → number  
  Arctangent.
- `math.atan2(y, x)` → number  
  Arctangent of y/x.
- `math.log(x, base?)` → number  
  Logarithm (natural log if base not specified).
- `math.log10(x)` → number  
  Base-10 logarithm.
- `math.log2(x)` → number  
  Base-2 logarithm.
- `math.exp(x)` → number  
  Exponential function.

#### Object Module
Functions for working with objects (dictionaries).

```snip
import "object";
```

- `object.create(obj?)` → object  
  Creates new object, optionally copying from another.
- `object.keys(obj)` → array  
  Returns array of keys.
- `object.values(obj)` → array  
  Returns array of values.
- `object.items(obj)` → array  
  Returns array of [key, value] pairs.
- `object.has(obj, key)` → boolean  
  Checks if key exists.
- `object.get(obj, key, default?)` → value  
  Gets value by key with optional default.
- `object.set(obj, key, value)` → object  
  Sets value and returns object.
- `object.delete(obj, key)` → boolean  
  Deletes key and returns success.
- `object.clear(obj)` → object  
  Removes all properties.
- `object.empty(obj)` → boolean  
  Checks if object has no properties.
- `object.size(obj)` → number  
  Returns number of properties.
- `object.merge(...objs)` → object  
  Merges objects (later objects override earlier ones).
- `object.assign(target, ...sources)` → object  
  Assigns properties from sources to target.

#### Types Module
Type checking and conversion functions.

```snip
import "types";
```

**Type Checking:**
- `types.typeof(value)` → string  
  Legacy function (use global `type()` instead).
- `types.isinstance(value, typeName)` → boolean  
  Checks if value is of given type.

**Conversion:**
- `types.tostring(value)` → string  
  Legacy function (use global `str()` instead).
- `types.tonumber(value)` → number  
  Legacy function (use global `int()` instead).
- `types.toboolean(value)` → boolean  
  Legacy function (use global `bool()` instead).

**JSON:**
- `types.json_parse(jsonString)` → value  
  Parses JSON string to Snip value.
- `types.json_stringify(value, indent?)` → string  
  Converts value to JSON string.

#### IO Module
Input/output functions.

```snip
import "io";
```

- `io.print(...args)` → null  
  Prints arguments without newline.
- `io.println(...args)` → null  
  Prints arguments with newline.
- `io.input(prompt?)` → string  
  Reads line from stdin.
- `io.readline()` → string  
  Reads line from stdin (alias for input()).

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

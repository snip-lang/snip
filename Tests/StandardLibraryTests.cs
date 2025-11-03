using Xunit;
using Snip.Lexer;
using Snip.Parser;
using Snip.Evaluator;

namespace Tests;

public class StandardLibraryTests
{
    private string EvaluateToString(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.Parse();
        var evaluator = new Evaluator();
        var env = new Snip.Evaluator.Environment();
        env.InitializeBuiltins();
        var result = evaluator.Eval(program, env);
        return result.ToString();
    }

    [Fact]
    public void TestGlobalLenFunction()
    {
        Assert.Equal("3", EvaluateToString("len([1, 2, 3]);"));
        Assert.Equal("5", EvaluateToString("len('hello');"));
        Assert.Equal("2", EvaluateToString("len({a: 1, b: 2});"));
    }

    [Fact]
    public void TestGlobalStrFunction()
    {
        Assert.Equal("\"42\"", EvaluateToString("str(42);"));
        Assert.Equal("\"false\"", EvaluateToString("str(false);"));
        Assert.Equal("\"[1, 2, 3]\"", EvaluateToString("str([1, 2, 3]);"));
    }

    [Fact]
    public void TestGlobalIntFunction()
    {
        Assert.Equal("42", EvaluateToString("int('42');"));
        Assert.Equal("45", EvaluateToString("int(45.67);"));
        Assert.Equal("1", EvaluateToString("int(true);"));
    }

    [Fact]
    public void TestGlobalBoolFunction()
    {
        Assert.Equal("false", EvaluateToString("bool(0);"));
        Assert.Equal("true", EvaluateToString("bool(1);"));
        Assert.Equal("false", EvaluateToString("bool('');"));
        Assert.Equal("true", EvaluateToString("bool('hello');"));
        Assert.Equal("false", EvaluateToString("bool([]);"));
        Assert.Equal("true", EvaluateToString("bool([1, 2]);"));
    }

    [Fact]
    public void TestArrayModule()
    {
        // Test len function
        Assert.Equal("3", EvaluateToString("import from \"array\"; array.len([1, 2, 3]);"));
        Assert.Equal("true", EvaluateToString("import from \"array\"; array.empty([]);"));
        Assert.Equal("false", EvaluateToString("import from \"array\"; array.empty([1]);"));

        // Test append and pop
        Assert.Equal("[1, 2, 3, 4]", EvaluateToString("import from \"array\"; let arr = [1, 2, 3]; array.append(arr, 4); arr;"));
        Assert.Equal("3", EvaluateToString("import from \"array\"; let arr = [1, 2, 3]; array.pop(arr);"));
    }

    [Fact]
    public void TestStringModule()
    {
        Assert.Equal("\"HELLO\"", EvaluateToString("import from \"string\"; string.upper('hello');"));
        Assert.Equal("\"Hello\"", EvaluateToString("import from \"string\"; string.capitalize('hello');"));
        Assert.Equal("[\"h\", \"e\", \"l\", \"l\", \"o\"]", EvaluateToString("import from \"string\"; string.split('hello', '');"));
        Assert.Equal("true", EvaluateToString("import from \"string\"; string.startswith('hello', 'he');"));
    }

    [Fact]
    public void TestMathModule()
    {
        Assert.Equal("4", EvaluateToString("import from \"math\"; math.pow(2, 2);"));
        Assert.Equal("3.141592653589793", EvaluateToString("import from \"math\"; math.pi;"));
        Assert.Equal("2.718281828459045", EvaluateToString("import from \"math\"; math.e;"));
        Assert.Equal("5", EvaluateToString("import from \"math\"; math.sqrt(25);"));
    }

    [Fact]
    public void TestObjectModule()
    {
        Assert.Equal("2", EvaluateToString("import from \"object\"; object.size({a: 1, b: 2});"));
        Assert.Equal("[\"a\", \"b\"]", EvaluateToString("import from \"object\"; object.keys({a: 1, b: 2});"));
        Assert.Equal("[1, 2]", EvaluateToString("import from \"object\"; object.values({a: 1, b: 2});"));
        Assert.Equal("true", EvaluateToString("import from \"object\"; object.has({a: 1, b: 2}, 'a');"));
        Assert.Equal("1", EvaluateToString("import from \"object\"; object.get({a: 1, b: 2}, 'a');"));
    }

    [Fact]
    public void TestTypesModule()
    {
        Assert.Equal("true", EvaluateToString("import from \"types\"; types.isinstance(42, 'number');"));
        Assert.Equal("true", EvaluateToString("import from \"types\"; types.isinstance('hello', 'string');"));
        Assert.Equal("\"{\"a\":1}\"", EvaluateToString("import from \"types\"; types.json_stringify({a: 1});"));
    }
    
    [Fact]
    public void TestHaskellPreludeMathFunctions()
    {
        Assert.Equal("true", EvaluateToString("import from \"math\"; math.even(4);"));
        Assert.Equal("false", EvaluateToString("import from \"math\"; math.even(5);"));
        Assert.Equal("false", EvaluateToString("import from \"math\"; math.odd(4);"));
        Assert.Equal("true", EvaluateToString("import from \"math\"; math.odd(5);"));
        Assert.Equal("6", EvaluateToString("import from \"math\"; math.gcd(48, 18);"));
        Assert.Equal("42", EvaluateToString("import from \"math\"; math.lcm(21, 6);"));
        Assert.Equal("-1", EvaluateToString("import from \"math\"; math.signum(-5);"));
        Assert.Equal("0", EvaluateToString("import from \"math\"; math.signum(0);"));
        Assert.Equal("1", EvaluateToString("import from \"math\"; math.signum(7);"));
        Assert.Equal("0.25", EvaluateToString("import from \"math\"; math.recip(4);"));
        Assert.Equal("3", EvaluateToString("import from \"math\"; math.quot(17, 5);"));
        Assert.Equal("2", EvaluateToString("import from \"math\"; math.rem(17, 5);"));
        Assert.Equal("3", EvaluateToString("import from \"math\"; math.div(17, 5);"));
        Assert.Equal("2", EvaluateToString("import from \"math\"; math.mod(17, 5);"));
    }
    
    [Fact]
    public void TestHaskellPreludeArrayFunctions()
    {
        Assert.Equal("1", EvaluateToString("import from \"array\"; array.head([1, 2, 3]);"));
        Assert.Equal("[2, 3]", EvaluateToString("import from \"array\"; array.tail([1, 2, 3]);"));
        Assert.Equal("[1, 2]", EvaluateToString("import from \"array\"; array.init([1, 2, 3]);"));
        Assert.Equal("3", EvaluateToString("import from \"array\"; array.last([1, 2, 3]);"));
        Assert.Equal("true", EvaluateToString("import from \"array\"; array.isNull([]);"));
        Assert.Equal("false", EvaluateToString("import from \"array\"; array.isNull([1]);"));
        Assert.Equal("3", EvaluateToString("import from \"array\"; array.length([1, 2, 3]);"));
        Assert.Equal("[1, 2]", EvaluateToString("import from \"array\"; array.take(2, [1, 2, 3]);"));
        Assert.Equal("[3]", EvaluateToString("import from \"array\"; array.drop(2, [1, 2, 3]);"));
        Assert.Equal("[[1, 2], [3]]", EvaluateToString("import from \"array\"; array.splitAt(2, [1, 2, 3]);"));
        Assert.Equal("true", EvaluateToString("import from \"array\"; array.elem(2, [1, 2, 3]);"));
        Assert.Equal("false", EvaluateToString("import from \"array\"; array.elem(4, [1, 2, 3]);"));
        Assert.Equal("1", EvaluateToString("import from \"array\"; array.elemIndex(2, [1, 2, 3]);"));
        Assert.Equal("-1", EvaluateToString("import from \"array\"; array.elemIndex(4, [1, 2, 3]);"));
    }
    
    [Fact]
    public void TestHaskellPreludeStringFunctions()
    {
        Assert.Equal("[\"hello\", \"world\"]", EvaluateToString("import from \"string\"; string.words(\"hello world\");"));
        Assert.Equal("[\"line1\", \"line2\"]", EvaluateToString("import from \"string\"; string.lines(\"line1\\nline2\");"));
        Assert.Equal("\"line1\nline2\"", EvaluateToString("import from \"string\"; string.unlines([\"line1\", \"line2\"]);"));
        Assert.Equal("\"hello world\"", EvaluateToString("import from \"string\"; string.unwords([\"hello\", \"world\"]);"));
        Assert.Equal("\"dlrow\"", EvaluateToString("import from \"string\"; string.reverse(\"world\");"));
        Assert.Equal("\"hello\"", EvaluateToString("import from \"string\"; string.take(5, \"hello world\");"));
        Assert.Equal("\"world\"", EvaluateToString("import from \"string\"; string.drop(6, \"hello world\");"));
    }
    
    [Fact]
    public void TestHaskellPreludeObjectFunctions()
    {
        Assert.Equal("2", EvaluateToString("import from \"object\"; object.lookup({a: 1, b: 2}, 'b');"));
        Assert.Equal("null", EvaluateToString("import from \"object\"; object.lookup({a: 1, b: 2}, 'c');"));
        Assert.Equal("\"default\"", EvaluateToString("import from \"object\"; object.lookup({a: 1, b: 2}, 'c', 'default');"));
        Assert.Equal("true", EvaluateToString("import from \"object\"; object.member({a: 1, b: 2}, 'a');"));
        Assert.Equal("false", EvaluateToString("import from \"object\"; object.member({a: 1, b: 2}, 'c');"));
        Assert.Equal("{\"a\": 1, \"b\": 2, \"c\": 3}", EvaluateToString("import from \"object\"; object.union({a: 1, b: 2}, {c: 3});"));
        Assert.Equal("{\"b\": 2}", EvaluateToString("import from \"object\"; object.intersection({a: 1, b: 2}, {b: 2, c: 3});"));
        Assert.Equal("{\"a\": 1}", EvaluateToString("import from \"object\"; object.difference({a: 1, b: 2}, {b: 2});"));
    }
}
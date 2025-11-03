using Xunit;
using Snip.Lexer;
using Snip.Parser;
using Snip.Evaluator;

namespace Tests;

public class IntegrationTests
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
    public void TestSimple()
    {
        var code = "let x = 5;\nx + 10;";
        var result = EvaluateToString(code);
        Assert.Equal("15", result);
    }

    [Fact]
    public void TestIf()
    {
        var code = "let x = 5;\nif (x > 3) {\n    x + 10\n} else {\n    x - 10\n}";
        var result = EvaluateToString(code);
        Assert.Equal("15", result);
    }

    [Fact]
    public void TestFunction()
    {
        var code = "function add(a, b) { return a + b; }\nadd(5, 3);";
        var result = EvaluateToString(code);
        Assert.Equal("8", result);
    }

    [Fact]
    public void TestAssign()
    {
        var code = "x = 5;";
        var result = EvaluateToString(code);
        Assert.Equal("5", result);
    }

    [Fact]
    public void TestComplex()
    {
        var code = "a = b = c = 10;";
        var result = EvaluateToString(code);
        Assert.Equal("10", result);
    }

    [Fact]
    public void TestFor()
    {
        var code = "let sum = 0;\nfor (let i = 0; i < 5; i = i + 1) {\n  sum = sum + i;\n}\nsum;";
        var result = EvaluateToString(code);
        Assert.Equal("10", result);
    }

    [Fact]
    public void TestWhile()
    {
        var code = "let x = 0;\nwhile (x < 3) {\n  x = x + 1;\n}\nx;";
        var result = EvaluateToString(code);
        Assert.Equal("3", result);
    }

    [Fact]
    public void TestIfElse()
    {
        var code = "if (5 > 3) { 42 } else { 24 }";
        var result = EvaluateToString(code);
        Assert.Equal("42", result);
    }

    [Fact]
    public void TestSwitch()
    {
        var code = "let x = 2;\nswitch (x) {\n  case 1:\n    \"one\";\n    break;\n  case 2:\n    \"two\";\n    break;\n  case 3:\n    \"three\";\n    break;\n  default:\n    \"unknown\";\n}";
        var result = EvaluateToString(code);
        Assert.Equal("\"two\"", result);
    }

    [Fact]
    public void TestDebugBlock()
    {
        var code = "{ let x = 1; let y = 2; x + y; }";
        var result = EvaluateToString(code);
        Assert.Equal("3", result);
    }

    [Fact]
    public void TestDebugObject()
    {
        var code = "{\"name\": \"John\", \"age\": 30};";
        var result = EvaluateToString(code);
        Assert.Equal("{\"name\": \"John\", \"age\": 30}", result);
    }

    [Fact]
    public void TestDebugObject2()
    {
        var code = "{\"name\": \"John\", \"age\": 30};";
        var result = EvaluateToString(code);
        Assert.Equal("{\"name\": \"John\", \"age\": 30}", result);
    }

    [Fact]
    public void TestSimpleRest()
    {
        var code = "function f(a, ...rest) {\n  return rest;\n}\nf(1, 2, 3);";
        var result = EvaluateToString(code);
        Assert.Equal("[2, 3]", result);
    }

    [Fact]
    public void TestSimpleSpread()
    {
        var code = "let arr1 = [1, 2];\nlet arr2 = [3, 4];\n[...arr1, ...arr2, 5];";
        var result = EvaluateToString(code);
        Assert.Equal("[1, 2, 3, 4, 5]", result);
    }

    [Fact]
    public void TestContinue2()
    {
        var code = "let i = 0;\nlet x = 0;\nwhile (i < 3) {\n    i = i + 1;\n    if (i == 1) {\n        x = 999;\n        break;\n    }\n    x = x + i;\n}\nx;";
        var result = EvaluateToString(code);
        Assert.Equal("999", result);
    }

    [Fact]
    public void TestContinue3()
    {
        var code = "let x = 0;\nif (true) {\n    continue;\n}\nx = 1;\nx;";
        var result = EvaluateToString(code);
        Assert.Equal("1", result);
    }

    [Fact]
    public void TestContinue4()
    {
        var code = "let i = 0;\ni = i + 1;\ni = i + 1;\ni;";
        var result = EvaluateToString(code);
        Assert.Equal("2", result);
    }

    [Fact]
    public void TestContinueSimple()
    {
        var code = "let sum = 0; for (let i = 0; i < 5; i = i + 1) { continue; sum = sum + i; } sum;";
        var result = EvaluateToString(code);
        Assert.Equal("0", result);
    }

    [Fact]
    public void TestContinue()
    {
        var code = "let sum = 0;\nfor (let i = 0; i < 5; i = i + 1) {\n    if (i == 2) {\n        continue;\n    }\n    sum = sum + i;\n}\nsum;";
        var result = EvaluateToString(code);
        Assert.Equal("8", result);
    }

    [Fact]
    public void TestDoWhile()
    {
        var code = "let x = 0;\ndo {\n  x = x + 1;\n} while (x < 3);\nx;";
        var result = EvaluateToString(code);
        Assert.Equal("3", result);
    }

    [Fact]
    public void TestElse()
    {
        var code = "if (false) { 42 } else { 24 }";
        var result = EvaluateToString(code);
        Assert.Equal("24", result);
    }

    [Fact]
    public void TestExpr2()
    {
        var code = "if (false) { 4 + 2 } else { 2 + 4 }";
        var result = EvaluateToString(code);
        Assert.Equal("6", result);
    }

    [Fact]
    public void TestExpr()
    {
        var code = "let x = 5; if (x > 3) { x + 10 } else { x - 10 }";
        var result = EvaluateToString(code);
        Assert.Equal("15", result);
    }

    [Fact]
    public void TestGt()
    {
        var code = "5 > 3;";
        var result = EvaluateToString(code);
        Assert.Equal("true", result);
    }

    [Fact]
    public void TestIf2()
    {
        var code = "if (5 > 3) { 15 }";
        var result = EvaluateToString(code);
        Assert.Equal("15", result);
    }

    [Fact]
    public void TestLetIf()
    {
        var code = "let x = 5; if (x > 3) { 42 } else { 24 }";
        var result = EvaluateToString(code);
        Assert.Equal("42", result);
    }

    [Fact]
    public void TestLoop()
    {
        var code = "let i = 0;\nlet x = 0;\nwhile (i < 3) {\n    i = i + 1;\n    x = x + 1;\n}\nx;";
        var result = EvaluateToString(code);
        Assert.Equal("3", result);
    }

    [Fact]
    public void TestOptional()
    {
        var code = "let obj = { prop: 42 }; print(obj?.prop); print(null?.prop);";
        var result = EvaluateToString(code);
        Assert.Equal("undefined", result);
    }

    [Fact]
    public void TestSemi()
    {
        var code = "let x = 5; if (x > 3) { x + 10; } else { x - 10; }";
        var result = EvaluateToString(code);
        Assert.Equal("15", result);
    }

    [Fact]
    public void TestSpread()
    {
        var code = "let arr1 = [1, 2];\nlet arr2 = [3, 4];\nlet result = [...arr1, ...arr2, 5];\nprint(result);";
        var result = EvaluateToString(code);
        Assert.Equal("undefined", result);
    }

    [Fact]
    public void TestStdlib()
    {
        var code = "print(\"Hello, World!\");";
        var result = EvaluateToString(code);
        Assert.Equal("undefined", result);
    }

    [Fact]
    public void TestVarExpr()
    {
        var code = "let x = 5; if (false) { x + 10 } else { x - 10 }";
        var result = EvaluateToString(code);
        Assert.Equal("-5", result);
    }

    // More tests will be added here
}
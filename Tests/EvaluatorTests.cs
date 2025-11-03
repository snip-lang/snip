using Snip.AST;
using Snip.Lexer;
using Snip.Parser;
using Snip.Evaluator;

namespace Tests;

public class EvaluatorTests
{
    private Value Evaluate(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.Parse();
        var evaluator = new Evaluator();
        var env = new Snip.Evaluator.Environment();
        return evaluator.Eval(program, env);
    }

    [Fact]
    public void EvalNumberLiteral_ShouldReturnNumberValue()
    {
        var result = Evaluate("42;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalFunctionDeclaration_ShouldDefineFunction()
    {
        var result = Evaluate("function add(a, b) { return a + b; } add;");
        Assert.Equal(Snip.Evaluator.ValueType.Function, result.Type);
    }

    [Fact]
    public void EvalFunctionCall_ShouldExecuteFunction()
    {
        var result = Evaluate("function add(a, b) { return a + b; } add(2, 3);");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(5.0, result.Data);
    }

    [Fact]
    public void EvalFunctionCallWithReturn_ShouldReturnValue()
    {
        var result = Evaluate("function test() { return 42; } test();");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalObjectLiteral_ShouldCreateObject()
    {
        var result = Evaluate("{ \"name\": \"John\", \"age\": 30 };");
        Assert.Equal(Snip.Evaluator.ValueType.Object, result.Type);
        var obj = (Dictionary<string, Value>)result.Data!;
        Assert.Equal("John", obj["name"].Data);
        Assert.Equal(30.0, obj["age"].Data);
    }

    [Fact]
    public void EvalMemberAccess_ShouldReturnPropertyValue()
    {
        var result = Evaluate("let obj = { \"name\": \"John\" }; obj.name;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("John", result.Data);
    }

    [Fact]
    public void EvalArrayLiteral_ShouldCreateArray()
    {
        var result = Evaluate("[1, 2, 3];");
        Assert.Equal(Snip.Evaluator.ValueType.Array, result.Type);
        var arr = (List<Value>)result.Data!;
        Assert.Equal(3, arr.Count);
        Assert.Equal(1.0, arr[0].Data);
        Assert.Equal(2.0, arr[1].Data);
        Assert.Equal(3.0, arr[2].Data);
    }

    [Fact]
    public void EvalCompoundAssignment_ShouldWorkCorrectly()
    {
        var result = Evaluate("let x = 5; x += 3; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(8.0, result.Data);

        result = Evaluate("let y = 10; y *= 2; y;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(20.0, result.Data);
    }

    [Fact]
    public void EvalVarStatement_ShouldDefineVariable()
    {
        var result = Evaluate("var x = 42; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalConstStatement_ShouldDefineConstant()
    {
        var result = Evaluate("const PI = 3.14; PI;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(3.14, result.Data);
    }

    [Fact]
    public void EvalStringLiteral_ShouldReturnStringValue()
    {
        var result = Evaluate("\"hello world\";");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("hello world", result.Data);
    }

    [Fact]
    public void EvalBooleanLiteral_ShouldReturnBooleanValue()
    {
        var result = Evaluate("true;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("false;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalNullLiteral_ShouldReturnNullValue()
    {
        var result = Evaluate("null;");
        Assert.Equal(Snip.Evaluator.ValueType.Null, result.Type);
    }

    [Fact]
    public void EvalUndefinedLiteral_ShouldReturnUndefinedValue()
    {
        var result = Evaluate("undefined;");
        Assert.Equal(Snip.Evaluator.ValueType.Undefined, result.Type);
    }

    [Fact]
    public void EvalAddExpression_ShouldReturnSum()
    {
        var result = Evaluate("1 + 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(3.0, result.Data);
    }

    [Fact]
    public void EvalSubtractExpression_ShouldReturnDifference()
    {
        var result = Evaluate("5 - 3;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(2.0, result.Data);
    }

    [Fact]
    public void EvalMultiplyExpression_ShouldReturnProduct()
    {
        var result = Evaluate("4 * 3;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(12.0, result.Data);
    }

    [Fact]
    public void EvalDivideExpression_ShouldReturnQuotient()
    {
        var result = Evaluate("8 / 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(4.0, result.Data);
    }

    [Fact]
    public void EvalModuloExpression_ShouldReturnRemainder()
    {
        var result = Evaluate("7 % 3;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(1.0, result.Data);
    }

    [Fact]
    public void EvalEqualExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("1 == 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("1 == 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalNotEqualExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("1 != 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("1 != 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalLessThanExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("1 < 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("2 < 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalGreaterThanExpression_ShouldReturnBoolean()
    {
        var result = Evaluate("2 > 1;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);

        result = Evaluate("1 > 2;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);
    }

    [Fact]
    public void EvalStringConcatenation_ShouldReturnConcatenatedString()
    {
        var result = Evaluate("\"hello\" + \" \" + \"world\";");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("hello world", result.Data);
    }

    [Fact]
    public void EvalLetStatement_ShouldDefineVariable()
    {
        var result = Evaluate("let x = 42; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalAssignmentExpression_ShouldUpdateVariable()
    {
        var result = Evaluate("let x = 1; x = 2; x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(2.0, result.Data);
    }

    [Fact]
    public void EvalIfStatement_TrueCondition_ShouldExecuteConsequence()
    {
        var result = Evaluate("if (true) { 42; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalIfStatement_FalseCondition_ShouldReturnUndefined()
    {
        var result = Evaluate("if (false) { 42; }");
        Assert.Equal(Snip.Evaluator.ValueType.Undefined, result.Type);
    }

    [Fact]
    public void EvalIfElseStatement_TrueCondition_ShouldExecuteConsequence()
    {
        var result = Evaluate("if (true) { 42; } else { 24; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalIfElseStatement_FalseCondition_ShouldExecuteAlternative()
    {
        var result = Evaluate("if (false) { 42; } else { 24; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(24.0, result.Data);
    }

    [Fact]
    public void EvalConditionalExpression_TrueCondition_ShouldReturnConsequent()
    {
        var result = Evaluate("true ? 42 : 24;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalConditionalExpression_FalseCondition_ShouldReturnAlternative()
    {
        var result = Evaluate("false ? 42 : 24;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(24.0, result.Data);
    }

    [Fact]
    public void EvalBlockStatement_ShouldReturnLastExpression()
    {
        var result = Evaluate("{ let x = 1; let y = 2; x + y; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(3.0, result.Data);
    }

    [Fact]
    public void EvalBlockStatement_Scoping_ShouldIsolateVariables()
    {
        var result = Evaluate("let x = 1; { let x = 2; x; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(1.0, result.Data);
    }

    [Fact]
    public void EvalComplexExpression_ShouldWorkCorrectly()
    {
        var result = Evaluate("let x = 10; if (x > 5) { x * 2; } else { x / 2; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(20.0, result.Data);
    }

    [Fact]
    public void EvalWhileLoop_ShouldExecuteUntilConditionFalse()
    {
        var result = Evaluate("let x = 0; while (x < 3) { x = x + 1; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type); // Addition returns float
        Assert.Equal(3.0, result.Data);
    }

    [Fact]
    public void EvalWhileLoop_ZeroIterations_ShouldNotExecuteBody()
    {
        var result = Evaluate("let x = 5; while (x < 3) { x = x + 1; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(5.0, result.Data);
    }

    [Fact]
    public void EvalForLoop_ShouldExecuteCorrectly()
    {
        var result = Evaluate("let sum = 0; for (let i = 0; i < 5; i = i + 1) { sum = sum + i; } sum;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type); // Addition returns float
        Assert.Equal(10.0, result.Data); // 0+1+2+3+4 = 10
    }

    [Fact]
    public void EvalForLoop_WithoutInitializer_ShouldWork()
    {
        var result = Evaluate("let i = 0; let sum = 0; for (; i < 3; i = i + 1) { sum = sum + i; } sum;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type); // Addition returns float
        Assert.Equal(3.0, result.Data); // 0+1+2 = 3
    }

    [Fact]
    public void EvalWhileLoop_WithContinue_ShouldSkipIteration()
    {
        var result = Evaluate("let x = 0; let sum = 0; while (x < 5) { x = x + 1; if (x == 3) { continue; } sum = sum + x; } sum;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(12.0, result.Data); // 1+2+4+5 = 12 (skips 3)
    }

    [Fact]
    public void EvalForLoop_WithContinue_ShouldSkipIteration()
    {
        var result = Evaluate("let sum = 0; for (let i = 0; i < 5; i = i + 1) { if (i == 2) { continue; } sum = sum + i; } sum;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type); // Addition returns float
        Assert.Equal(8.0, result.Data); // 0+1+3+4 = 8 (skips 2)
    }

    [Fact]
    public void EvalUnaryNotOperator_ShouldNegateBoolean()
    {
        var result = Evaluate("!true;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.False((bool)result.Data!);

        result = Evaluate("!false;");
        Assert.Equal(Snip.Evaluator.ValueType.Boolean, result.Type);
        Assert.True((bool)result.Data!);
    }

    [Fact]
    public void EvalUnaryMinusOperator_ShouldNegateNumber()
    {
        var result = Evaluate("-42;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(-42.0, result.Data);

        result = Evaluate("-3.14;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(-3.14, result.Data);
    }

    [Fact]
    public void EvalUnaryPlusOperator_ShouldReturnNumber()
    {
        var result = Evaluate("+42;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);

        result = Evaluate("+3.14;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(3.14, result.Data);
    }

    [Fact]
    public void EvalClassDeclaration_ShouldDefineClass()
    {
        var result = Evaluate("class Test {} Test;");
        Assert.Equal(Snip.Evaluator.ValueType.Class, result.Type);
    }

    [Fact]
    public void EvalNewExpression_ShouldCreateInstance()
    {
        var result = Evaluate("class Test {} new Test();");
        Assert.Equal(Snip.Evaluator.ValueType.Instance, result.Type);
    }

    [Fact]
    public void EvalClassWithProperty_ShouldInitializeProperty()
    {
        var result = Evaluate("class Test { x = 42; } let t = new Test(); t.x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalClassWithMethod_ShouldExecuteMethod()
    {
        var result = Evaluate("class Test { getValue() { return 42; } } let t = new Test(); t.getValue();");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalClassWithConstructor_ShouldExecuteConstructor()
    {
        var result = Evaluate("class Test { constructor() { this.value = 42; } } let t = new Test(); t.value;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalThisInMethod_ShouldReferToInstance()
    {
        var result = Evaluate("class Test { setValue(v) { this.value = v; } } let t = new Test(); t.setValue(123); t.value;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(123.0, result.Data);
    }

    [Fact]
    public void EvalClassInheritance_ShouldInheritProperties()
    {
        var result = Evaluate("class Animal { name = \"animal\"; } class Dog extends Animal {} let d = new Dog(); d.name;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("animal", result.Data);
    }

    [Fact]
    public void EvalClassInheritance_ShouldInheritMethods()
    {
        var result = Evaluate("class Animal { speak() { return \"sound\"; } } class Dog extends Animal {} let d = new Dog(); d.speak();");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("sound", result.Data);
    }

    [Fact]
    public void EvalSuperConstructor_ShouldCallParentConstructor()
    {
        var result = Evaluate("class Animal { constructor(name) { this.name = name; } } class Dog extends Animal { constructor(name, breed) { super(name); this.breed = breed; } } let d = new Dog(\"Rex\", \"Labrador\"); d.name;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("Rex", result.Data);
    }

    [Fact]
    public void EvalSuperMethod_ShouldCallParentMethod()
    {
        var result = Evaluate("class Animal { speak() { return \"animal sound\"; } } class Dog extends Animal { speak() { return super.speak() + \" woof\"; } } let d = new Dog(); d.speak();");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("animal sound woof", result.Data);
    }

    [Fact]
    public void EvalSuperProperty_ShouldAccessParentProperty()
    {
        var result = Evaluate("class Animal { species = \"animal\"; } class Dog extends Animal { getSpecies() { return super.species; } } let d = new Dog(); d.getSpecies();");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("animal", result.Data);
    }

    [Fact]
    public void EvalSwitchStatement_ShouldExecuteMatchingCase()
    {
        var result = Evaluate("let x = 1; switch (x) { case 1: 42; break; case 2: 24; break; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalSwitchStatement_ShouldFallThroughCases()
    {
        var result = Evaluate("let x = 2; switch (x) { case 1: 10; case 2: 20; case 3: 30; break; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(30.0, result.Data);
    }

    [Fact]
    public void EvalSwitchStatement_ShouldExecuteDefaultWhenNoMatch()
    {
        var result = Evaluate("let x = 5; switch (x) { case 1: 10; break; case 2: 20; break; default: 99; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(99.0, result.Data);
    }

    [Fact]
    public void EvalSwitchStatement_ShouldReturnUndefinedIfNoMatchAndNoDefault()
    {
        var result = Evaluate("let x = 5; switch (x) { case 1: 10; break; case 2: 20; break; }");
        Assert.Equal(Snip.Evaluator.ValueType.Undefined, result.Type);
    }

    [Fact]
    public void EvalDoWhileLoop_ShouldExecuteAtLeastOnce()
    {
        var result = Evaluate("let x = 0; do { x = 42; } while (false); x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalDoWhileLoop_ShouldExecuteMultipleTimes()
    {
        var result = Evaluate("let x = 0; do { x = x + 1; } while (x < 3); x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(3.0, result.Data);
    }

    [Fact]
    public void EvalDoWhileLoop_ShouldHandleBreak()
    {
        var result = Evaluate("let x = 0; do { x = x + 1; if (x == 2) { break; } } while (x < 10); x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(2.0, result.Data);
    }

    [Fact]
    public void EvalDoWhileLoop_ShouldHandleContinue()
    {
        var result = Evaluate("let x = 0; let sum = 0; do { x = x + 1; if (x == 2) { continue; } sum = sum + x; } while (x < 4); sum;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(8.0, result.Data); // 1 + 3 + 4 (skips sum for x=2)
    }

    [Fact]
    public void EvalThrowStatement_ShouldCreateExceptionValue()
    {
        var result = Evaluate("throw \"error message\";");
        Assert.Equal(Snip.Evaluator.ValueType.Exception, result.Type);
        var exceptionValue = (Snip.Evaluator.ExceptionValue)result.Data!;
        Assert.Equal(Snip.Evaluator.ValueType.String, exceptionValue.Value.Type);
        Assert.Equal("error message", exceptionValue.Value.Data);
    }

    [Fact]
    public void EvalTryCatch_ShouldCatchException()
    {
        var result = Evaluate("try { throw \"error\"; } catch (e) { e; }");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("error", result.Data);
    }

    [Fact]
    public void EvalTryCatch_ShouldExecuteTryBlockNormally()
    {
        var result = Evaluate("try { 42; } catch (e) { \"caught\"; }");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalTryFinally_ShouldExecuteFinallyBlock()
    {
        var result = Evaluate("let x = 0; try { x = 42; } finally { x = x + 1; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(43.0, result.Data);
    }

    [Fact]
    public void EvalTryCatchFinally_ShouldExecuteBothCatchAndFinally()
    {
        var result = Evaluate("let x = 0; try { throw \"error\"; x = 1; } catch (e) { x = 2; } finally { x = x + 10; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(12.0, result.Data);
    }

    [Fact]
    public void EvalTryCatchFinally_ShouldExecuteFinallyAfterNormalTry()
    {
        var result = Evaluate("let x = 0; try { x = 5; } catch (e) { x = 10; } finally { x = x * 2; } x;");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(10.0, result.Data);
    }

    [Fact]
    public void EvalNestedTryCatch_ShouldHandleNestedExceptions()
    {
        var result = Evaluate("try { try { throw \"inner\"; } catch (e) { throw \"outer\"; } } catch (e) { e; }");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("outer", result.Data);
    }

    [Fact]
    public void EvalArraySpread_ShouldFlattenArrays()
    {
        var result = Evaluate("let arr1 = [1, 2]; let arr2 = [3, 4]; [...arr1, ...arr2, 5];");
        Assert.Equal(Snip.Evaluator.ValueType.Array, result.Type);
        var arr = Assert.IsType<List<Snip.Evaluator.Value>>(result.Data);
        Assert.Equal(5, arr.Count);
        Assert.Equal(1.0, arr[0].Data);
        Assert.Equal(2.0, arr[1].Data);
        Assert.Equal(3.0, arr[2].Data);
        Assert.Equal(4.0, arr[3].Data);
        Assert.Equal(5.0, arr[4].Data);
    }

    [Fact]
    public void EvalFunctionRestParameters_ShouldCollectRemainingArgs()
    {
        var result = Evaluate("function sum(a, b, ...rest) { let total = a + b; for (let i = 0; i < rest.length; i++) { total += rest[i]; } return total; } sum(1, 2, 3, 4);");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(10.0, result.Data);
    }

    [Fact]
    public void EvalFunctionCallSpread_ShouldExpandArguments()
    {
        var result = Evaluate("function add(a, b, c) { return a + b + c; } let args = [1, 2, 3]; add(...args);");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(6.0, result.Data);
    }

    [Fact]
    public void EvalArrowFunctionExpression_ShouldReturnFunction()
    {
        var result = Evaluate("let add = (a, b) => a + b; add;");
        Assert.Equal(Snip.Evaluator.ValueType.Function, result.Type);
    }

    [Fact]
    public void EvalArrowFunctionCall_ShouldExecuteFunction()
    {
        var result = Evaluate("let add = (a, b) => a + b; add(2, 3);");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(5.0, result.Data);
    }

    [Fact]
    public void EvalArrowFunctionWithBlockBody_ShouldExecuteBlock()
    {
        var result = Evaluate("let test = (x) => { return x * 2; }; test(5);");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(10.0, result.Data);
    }

    [Fact]
    public void EvalArrowFunctionSingleParam_ShouldWork()
    {
        var result = Evaluate("let square = x => x * x; square(4);");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(16.0, result.Data);
    }

    [Fact]
    public void EvalArrowFunctionNoParams_ShouldWork()
    {
        var result = Evaluate("let get42 = () => 42; get42();");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(42.0, result.Data);
    }

    [Fact]
    public void EvalArrowFunctionClosure_ShouldCaptureEnvironment()
    {
        var result = Evaluate("let multiplier = 2; let multiply = x => x * multiplier; multiply(5);");
        Assert.Equal(Snip.Evaluator.ValueType.Number, result.Type);
        Assert.Equal(10.0, result.Data);
    }

    [Fact]
    public void EvalTemplateLiteralNoInterpolation_ShouldReturnString()
    {
        var result = Evaluate("`Hello World`;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("Hello World", result.Data);
    }

    [Fact]
    public void EvalTemplateLiteralWithVariable_ShouldInterpolate()
    {
        var result = Evaluate("let name = \"John\"; `Hello ${name}!`;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("Hello John!", result.Data);
    }

    [Fact]
    public void EvalTemplateLiteralWithNumber_ShouldInterpolate()
    {
        var result = Evaluate("let age = 25; `I am ${age} years old.`;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("I am 25 years old.", result.Data);
    }

    [Fact]
    public void EvalTemplateLiteralMultipleInterpolations_ShouldWork()
    {
        var result = Evaluate("let a = 1; let b = 2; `Values: ${a} and ${b}`;");
        Assert.Equal(Snip.Evaluator.ValueType.String, result.Type);
        Assert.Equal("Values: 1 and 2", result.Data);
    }
}
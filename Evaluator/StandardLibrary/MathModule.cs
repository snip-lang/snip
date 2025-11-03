using Snip.AST;

namespace Snip.Evaluator;

public static class MathModule
{
    public static Module CreateModule()
    {
        var module = new Module("math");
        
        // Constants
        module.Export("pi", new Value(ValueType.Number, Math.PI));
        module.Export("e", new Value(ValueType.Number, Math.E));
        module.Export("tau", new Value(ValueType.Number, 2 * Math.PI));
        module.Export("inf", new Value(ValueType.Number, double.PositiveInfinity));
        module.Export("nan", new Value(ValueType.Number, double.NaN));
        
        // Basic functions
        module.Export("abs", Value.NativeFunction(new NativeFunctionValue("abs", Abs)));
        module.Export("ceil", Value.NativeFunction(new NativeFunctionValue("ceil", Ceil)));
        module.Export("floor", Value.NativeFunction(new NativeFunctionValue("floor", Floor)));
        module.Export("round", Value.NativeFunction(new NativeFunctionValue("round", Round)));
        module.Export("max", Value.NativeFunction(new NativeFunctionValue("max", Max)));
        module.Export("min", Value.NativeFunction(new NativeFunctionValue("min", Min)));
        module.Export("pow", Value.NativeFunction(new NativeFunctionValue("pow", Pow)));
        module.Export("sqrt", Value.NativeFunction(new NativeFunctionValue("sqrt", Sqrt)));
        
        // Trigonometry
        module.Export("sin", Value.NativeFunction(new NativeFunctionValue("sin", Sin)));
        module.Export("cos", Value.NativeFunction(new NativeFunctionValue("cos", Cos)));
        module.Export("tan", Value.NativeFunction(new NativeFunctionValue("tan", Tan)));
        module.Export("asin", Value.NativeFunction(new NativeFunctionValue("asin", Asin)));
        module.Export("acos", Value.NativeFunction(new NativeFunctionValue("acos", Acos)));
        module.Export("atan", Value.NativeFunction(new NativeFunctionValue("atan", Atan)));
        module.Export("atan2", Value.NativeFunction(new NativeFunctionValue("atan2", Atan2)));
        
        // Logarithmic
        module.Export("log", Value.NativeFunction(new NativeFunctionValue("log", Log)));
        module.Export("log10", Value.NativeFunction(new NativeFunctionValue("log10", Log10)));
        module.Export("log2", Value.NativeFunction(new NativeFunctionValue("log2", Log2)));
        module.Export("exp", Value.NativeFunction(new NativeFunctionValue("exp", Exp)));
        
        // Haskell Prelude functions
        module.Export("even", Value.NativeFunction(new NativeFunctionValue("even", Even)));
        module.Export("odd", Value.NativeFunction(new NativeFunctionValue("odd", Odd)));
        module.Export("gcd", Value.NativeFunction(new NativeFunctionValue("gcd", Gcd)));
        module.Export("lcm", Value.NativeFunction(new NativeFunctionValue("lcm", Lcm)));
        module.Export("signum", Value.NativeFunction(new NativeFunctionValue("signum", Signum)));
        module.Export("recip", Value.NativeFunction(new NativeFunctionValue("recip", Recip)));
        module.Export("quot", Value.NativeFunction(new NativeFunctionValue("quot", Quot)));
        module.Export("rem", Value.NativeFunction(new NativeFunctionValue("rem", Rem)));
        module.Export("div", Value.NativeFunction(new NativeFunctionValue("div", Div)));
        module.Export("mod", Value.NativeFunction(new NativeFunctionValue("mod", Mod)));
        
        return module;
    }
    
    private static Value Abs(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "abs");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Abs(num));
    }
    
    private static Value Ceil(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "ceil");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Ceiling(num));
    }
    
    private static Value Floor(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "floor");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Floor(num));
    }
    
    private static Value Round(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "round");
        var num = ValidationHelpers.GetNumber(args, 0);
        var digits = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : 0;
        var factor = Math.Pow(10, digits);
        return new Value(ValueType.Number, Math.Round(num * factor) / factor);
    }
    
    private static Value Max(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, -1, "max");
        var max = ValidationHelpers.GetNumber(args, 0);
        for (int i = 1; i < args.Count; i++)
        {
            var num = ValidationHelpers.GetNumber(args, i);
            max = Math.Max(max, num);
        }
        return new Value(ValueType.Number, max);
    }
    
    private static Value Min(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, -1, "min");
        var min = ValidationHelpers.GetNumber(args, 0);
        for (int i = 1; i < args.Count; i++)
        {
            var num = ValidationHelpers.GetNumber(args, i);
            min = Math.Min(min, num);
        }
        return new Value(ValueType.Number, min);
    }
    
    private static Value Pow(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "pow");
        var @base = ValidationHelpers.GetNumber(args, 0);
        var exp = ValidationHelpers.GetNumber(args, 1);
        return new Value(ValueType.Number, Math.Pow(@base, exp));
    }
    
    private static Value Sqrt(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "sqrt");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Sqrt(num));
    }
    
    private static Value Sin(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "sin");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Sin(num));
    }
    
    private static Value Cos(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "cos");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Cos(num));
    }
    
    private static Value Tan(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "tan");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Tan(num));
    }
    
    private static Value Asin(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "asin");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Asin(num));
    }
    
    private static Value Acos(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "acos");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Acos(num));
    }
    
    private static Value Atan(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "atan");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Atan(num));
    }
    
    private static Value Atan2(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "atan2");
        var y = ValidationHelpers.GetNumber(args, 0);
        var x = ValidationHelpers.GetNumber(args, 1);
        return new Value(ValueType.Number, Math.Atan2(y, x));
    }
    
    private static Value Log(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "log");
        var num = ValidationHelpers.GetNumber(args, 0);
        if (args.Count == 1)
        {
            return new Value(ValueType.Number, Math.Log(num));
        }
        var @base = ValidationHelpers.GetNumber(args, 1);
        return new Value(ValueType.Number, Math.Log(num, @base));
    }
    
    private static Value Log10(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "log10");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Log10(num));
    }
    
    private static Value Log2(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "log2");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Log2(num));
    }
    
    private static Value Exp(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "exp");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Number, Math.Exp(num));
    }
    
    // Haskell Prelude functions
    private static Value Even(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "even");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Boolean, num % 2 == 0 && num != 0);
    }
    
    private static Value Odd(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "odd");
        var num = ValidationHelpers.GetNumber(args, 0);
        return new Value(ValueType.Boolean, num % 2 != 0);
    }
    
    private static Value Gcd(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "gcd");
        var a = Math.Abs(ValidationHelpers.GetNumber(args, 0));
        var b = Math.Abs(ValidationHelpers.GetNumber(args, 1));
        
        while (b > 0.0001) // Use epsilon for floating point comparison
        {
            var temp = b;
            b = a % b;
            a = temp;
        }
        
        return new Value(ValueType.Number, a);
    }
    
    private static Value Lcm(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "lcm");
        var a = Math.Abs(ValidationHelpers.GetNumber(args, 0));
        var b = Math.Abs(ValidationHelpers.GetNumber(args, 1));
        
        var gcdValue = Gcd(new List<Value> { Value.Number(a), Value.Number(b) });
        return new Value(ValueType.Number, Math.Abs(a * b) / (gcdValue.Data as double? ?? 1));
    }
    
    private static Value Signum(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "signum");
        var num = ValidationHelpers.GetNumber(args, 0);
        
        if (num > 0) return new Value(ValueType.Number, 1);
        if (num < 0) return new Value(ValueType.Number, -1);
        return new Value(ValueType.Number, 0);
    }
    
    private static Value Recip(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "recip");
        var num = ValidationHelpers.GetNumber(args, 0);
        
        if (num == 0)
            throw new Exception("Division by zero in recip");
            
        return new Value(ValueType.Number, 1.0 / num);
    }
    
    private static Value Quot(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "quot");
        var a = ValidationHelpers.GetNumber(args, 0);
        var b = ValidationHelpers.GetNumber(args, 1);
        
        if (Math.Abs(b) < 0.0001)
            throw new Exception("Division by zero in quot");
            
        return new Value(ValueType.Number, Math.Truncate(a / b));
    }
    
    private static Value Rem(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "rem");
        var a = ValidationHelpers.GetNumber(args, 0);
        var b = ValidationHelpers.GetNumber(args, 1);
        
        if (Math.Abs(b) < 0.0001)
            throw new Exception("Division by zero in rem");
            
        return new Value(ValueType.Number, a % b);
    }
    
    private static Value Div(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "div");
        var a = ValidationHelpers.GetNumber(args, 0);
        var b = ValidationHelpers.GetNumber(args, 1);
        
        if (Math.Abs(b) < 0.0001)
            throw new Exception("Division by zero in div");
            
        return new Value(ValueType.Number, Math.Floor(a / b));
    }
    
    private static Value Mod(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "mod");
        var a = ValidationHelpers.GetNumber(args, 0);
        var b = ValidationHelpers.GetNumber(args, 1);
        
        if (Math.Abs(b) < 0.0001)
            throw new Exception("Division by zero in mod");
            
        var result = a % b;
        if ((result < 0 && b > 0) || (result > 0 && b < 0))
            result += b;
            
        return new Value(ValueType.Number, result);
    }
}
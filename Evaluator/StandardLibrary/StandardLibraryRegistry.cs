using Snip.AST;
using System;

namespace Snip.Evaluator;

public static class StandardLibraryRegistry
{
    private static readonly Dictionary<string, Func<Module>> _modules = new();
    
    static StandardLibraryRegistry()
    {
        _modules["math"] = MathModule.CreateModule;
        _modules["string"] = StringModule.CreateModule;
        _modules["array"] = ArrayModule.CreateModule;
        _modules["object"] = ObjectModule.CreateModule;
        _modules["io"] = IOModule.CreateModule;
        _modules["types"] = TypesModule.CreateModule;
        _modules["random"] = RandomModule.CreateModule;
        _modules["datetime"] = DateTimeModule.CreateModule;
        _modules["file"] = FileModule.CreateModule;
        _modules["regex"] = RegexModule.CreateModule;
        _modules["collections"] = CollectionsModule.CreateModule;
        _modules["itertools"] = ItertoolsModule.CreateModule;
    }
    
    public static Module GetModule(string moduleName)
    {
        if (_modules.TryGetValue(moduleName, out var creator))
        {
            return creator();
        }
        throw new Exception($"Module '{moduleName}' not found");
    }
    
    public static void RegisterAllModules(Environment env)
    {
        // Register commonly used functions directly into global scope
        env.ImportModule("io");  // print, input always available
        env.ImportModule("types"); // JSON functions always available

        // Register pythonic global functions
        env.Define("len", Value.NativeFunction(new NativeFunctionValue("len", Len)));
        env.Define("type", Value.NativeFunction(new NativeFunctionValue("type", TypeOf)));
        env.Define("str", Value.NativeFunction(new NativeFunctionValue("str", Str)));
        env.Define("int", Value.NativeFunction(new NativeFunctionValue("int", Int)));
        env.Define("bool", Value.NativeFunction(new NativeFunctionValue("bool", Bool)));
        env.Define("print", Value.NativeFunction(new NativeFunctionValue("print", Print)));
        env.Define("input", Value.NativeFunction(new NativeFunctionValue("input", Input)));
    }

    private static Value Len(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "len");
        var value = args[0];

        switch (value.Type)
        {
            case ValueType.Array:
                return Value.Number(((List<Value>)value.Data!).Count);
            case ValueType.String:
                return Value.Number(((string)value.Data!).Length);
            case ValueType.Object:
                return Value.Number(((Dictionary<string, Value>)value.Data!).Count);
            default:
                throw new Exception($"len() argument must be array, string, or object, got {value.Type}");
        }
    }

    private static Value TypeOf(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "type");
        var value = args[0];
        return Value.String(value.Type.ToString().ToLower());
    }

    private static Value Str(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "str");
        var value = args[0];

        if (value.Type == ValueType.Boolean)
        {
            return Value.String(((bool)value.Data!).ToString().ToLower());
        }

        return Value.String(value.ToString());
    }

    private static Value Int(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "int");
        var value = args[0];

        if (value.Type == ValueType.Number)
            return Value.Number(Math.Floor((double)value.Data!));
        else if (value.Type == ValueType.String)
        {
            if (double.TryParse((string)value.Data!, out var num))
                return Value.Number(Math.Floor(num));
            else
                return Value.Number(0);
        }
        else if (value.Type == ValueType.Boolean)
            return Value.Number((bool)value.Data! ? 1 : 0);
        else if (value.Type == ValueType.Null)
            return Value.Number(0);
        else
            return Value.Number(0);
    }

    private static Value Bool(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "bool");
        var value = args[0];

        if (value.Type == ValueType.Boolean)
            return value;
        else if (value.Type == ValueType.Number)
        {
            var num = (double)value.Data!;
            return Value.Boolean(num != 0 && !double.IsNaN(num));
        }
        else if (value.Type == ValueType.String)
            return Value.Boolean(!string.IsNullOrEmpty((string)value.Data!));
        else if (value.Type == ValueType.Null || value.Type == ValueType.Undefined)
            return Value.Boolean(false);
        else if (value.Type == ValueType.Array)
            return Value.Boolean(((List<Value>)value.Data!).Count > 0);
        else
            return Value.Boolean(true);
    }

    private static Value Print(List<Value> args)
    {
        var output = string.Join(" ", args.Select(arg => arg.ToString()));
        Console.WriteLine(output);
        return Value.Null();
    }

    private static Value Input(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 1, "input");
        if (args.Count > 0)
        {
            var prompt = ValidationHelpers.GetString(args, 0);
            Console.Write(prompt);
        }
        var input = Console.ReadLine();
        return Value.String(input ?? "");
    }
}
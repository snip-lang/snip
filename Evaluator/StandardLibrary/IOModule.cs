using Snip.AST;

namespace Snip.Evaluator;

public static class IOModule
{
    public static Module CreateModule()
    {
        var module = new Module("io");
        
        // Console operations
        module.Export("print", Value.NativeFunction(new NativeFunctionValue("print", Print)));
        module.Export("println", Value.NativeFunction(new NativeFunctionValue("println", PrintLn)));
        module.Export("input", Value.NativeFunction(new NativeFunctionValue("input", Input)));
        module.Export("readline", Value.NativeFunction(new NativeFunctionValue("readline", ReadLine)));
        
        return module;
    }
    
    private static Value Print(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, -1, "print");
        var output = string.Join(" ", args.Select(arg => arg.ToString()));
        Console.Write(output);
        return Value.Null();
    }
    
    private static Value PrintLn(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, -1, "println");
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
    
    private static Value ReadLine(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 0, "readline");
        var input = Console.ReadLine();
        return Value.String(input ?? "");
    }
}
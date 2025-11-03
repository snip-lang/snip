using Snip.AST;

namespace Snip.Evaluator;

public static class ValidationHelpers
{
    public static void ValidateArgs(List<Value> args, int min, int max, string funcName)
    {
        if (args.Count < min)
            throw new Exception($"{funcName}() takes at least {min} argument(s)");
        if (max != -1 && args.Count > max)
            throw new Exception($"{funcName}() takes at most {max} argument(s)");
    }
    
    public static string GetString(List<Value> args, int index)
    {
        var value = args[index];
        if (value.Type != ValueType.String)
            throw new Exception($"Argument {index} must be a string");
        return (string)value.Data!;
    }
    
    public static double GetNumber(List<Value> args, int index)
    {
        var value = args[index];
        if (value.Type != ValueType.Number)
            throw new Exception($"Argument {index} must be a number");
        return (double)value.Data!;
    }
    
    public static int GetInt(List<Value> args, int index)
    {
        var value = args[index];
        if (value.Type != ValueType.Number)
            throw new Exception($"Argument {index} must be a number");
        var num = (double)value.Data!;
        if (num != (int)num)
            throw new Exception($"Argument {index} must be an integer");
        return (int)num;
    }
    
    public static List<Value> GetArray(List<Value> args, int index)
    {
        var value = args[index];
        if (value.Type != ValueType.Array)
            throw new Exception($"Argument {index} must be an array");
        return (List<Value>)value.Data!;
    }
    
    public static Dictionary<string, Value> GetObject(List<Value> args, int index)
    {
        var value = args[index];
        if (value.Type != ValueType.Object)
            throw new Exception($"Argument {index} must be an object");
        return (Dictionary<string, Value>)value.Data!;
    }
    
    public static bool GetBoolean(List<Value> args, int index)
    {
        var value = args[index];
        if (value.Type != ValueType.Boolean)
            throw new Exception($"Argument {index} must be a boolean");
        return (bool)value.Data!;
    }
}
using Snip.AST;

namespace Snip.Evaluator;

public static class ObjectModule
{
    public static Module CreateModule()
    {
        var module = new Module("object");
        
        // Creation
        module.Export("create", Value.NativeFunction(new NativeFunctionValue("create", Create)));
        module.Export("keys", Value.NativeFunction(new NativeFunctionValue("keys", Keys)));
        module.Export("values", Value.NativeFunction(new NativeFunctionValue("values", Values)));
        module.Export("items", Value.NativeFunction(new NativeFunctionValue("items", Items)));
        
        // Information
        module.Export("has", Value.NativeFunction(new NativeFunctionValue("has", Has)));
        module.Export("get", Value.NativeFunction(new NativeFunctionValue("get", Get)));
        module.Export("set", Value.NativeFunction(new NativeFunctionValue("set", Set)));
        module.Export("delete", Value.NativeFunction(new NativeFunctionValue("delete", Delete)));
        module.Export("clear", Value.NativeFunction(new NativeFunctionValue("clear", Clear)));
        
        // Testing
        module.Export("isempty", Value.NativeFunction(new NativeFunctionValue("isempty", IsEmpty)));
        module.Export("size", Value.NativeFunction(new NativeFunctionValue("size", Size)));
        
        // Merging
        module.Export("merge", Value.NativeFunction(new NativeFunctionValue("merge", Merge)));
        module.Export("assign", Value.NativeFunction(new NativeFunctionValue("assign", Assign)));
        
        // Functional programming
        module.Export("map", Value.NativeFunction(new NativeFunctionValue("map", Map)));
        module.Export("filter", Value.NativeFunction(new NativeFunctionValue("filter", Filter)));
        module.Export("reduce", Value.NativeFunction(new NativeFunctionValue("reduce", Reduce)));
        
        // Aliases
        module.Export("entries", Value.NativeFunction(new NativeFunctionValue("entries", Items)));
        module.Export("fromEntries", Value.NativeFunction(new NativeFunctionValue("fromEntries", FromEntries)));
        
        // Haskell Prelude functions
        module.Export("lookup", Value.NativeFunction(new NativeFunctionValue("lookup", Lookup)));
        module.Export("member", Value.NativeFunction(new NativeFunctionValue("member", Member)));
        module.Export("union", Value.NativeFunction(new NativeFunctionValue("union", Union)));
        module.Export("intersection", Value.NativeFunction(new NativeFunctionValue("intersection", Intersection)));
        module.Export("difference", Value.NativeFunction(new NativeFunctionValue("difference", Difference)));
        
        return module;
    }
    
    private static Value Create(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 1, "create");
        var obj = new Dictionary<string, Value>();
        if (args.Count > 0)
        {
            var source = ValidationHelpers.GetObject(args, 0);
            foreach (var kvp in source)
                obj[kvp.Key] = kvp.Value;
        }
        return Value.Object(obj);
    }
    
    private static Value Keys(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "keys");
        var obj = ValidationHelpers.GetObject(args, 0);
        var keys = obj.Keys.Select(k => Value.String(k)).ToList();
        return Value.Array(keys);
    }
    
    private static Value Values(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "values");
        var obj = ValidationHelpers.GetObject(args, 0);
        return Value.Array(obj.Values.ToList());
    }
    
    private static Value Items(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "items");
        var obj = ValidationHelpers.GetObject(args, 0);
        var items = obj.Select(kvp => Value.Array(new List<Value> { Value.String(kvp.Key), kvp.Value })).ToList();
        return Value.Array(items);
    }
    
    private static Value Has(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "has");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        return Value.Boolean(obj.ContainsKey(key));
    }
    
    private static Value Get(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "get");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        var defaultValue = args.Count > 2 ? args[2] : Value.Null();
        
        return obj.TryGetValue(key, out var value) ? value : defaultValue;
    }
    
    private static Value Set(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 3, "set");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        var value = args[2];
        obj[key] = value;
        return Value.Object(obj);
    }
    
    private static Value Delete(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "delete");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        var removed = obj.Remove(key);
        return Value.Boolean(removed);
    }
    
    private static Value Clear(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "clear");
        var obj = ValidationHelpers.GetObject(args, 0);
        obj.Clear();
        return Value.Object(obj);
    }
    
    private static Value IsEmpty(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isempty");
        var obj = ValidationHelpers.GetObject(args, 0);
        return Value.Boolean(obj.Count == 0);
    }
    
    private static Value Size(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "size");
        var obj = ValidationHelpers.GetObject(args, 0);
        return Value.Number(obj.Count);
    }
    
    private static Value Merge(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "merge");
        var result = new Dictionary<string, Value>();
        
        foreach (var arg in args)
        {
            var obj = ValidationHelpers.GetObject(new List<Value> { arg }, 0);
            foreach (var kvp in obj)
                result[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(result);
    }
    
    private static Value Assign(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, -1, "assign");
        var target = ValidationHelpers.GetObject(args, 0);
        
        for (int i = 1; i < args.Count; i++)
        {
            var source = ValidationHelpers.GetObject(new List<Value> { args[i] }, 0);
            foreach (var kvp in source)
                target[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(target);
    }
    
    private static Value Map(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "map");
        var obj = ValidationHelpers.GetObject(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to object.map must be a function");
        
        var result = new Dictionary<string, Value>();
        foreach (var kvp in obj)
        {
            var callArgs = new List<Value> { kvp.Value, Value.String(kvp.Key) };
            var mapped = CallFunction(func, callArgs);
            result[kvp.Key] = mapped;
        }
        
        return Value.Object(result);
    }
    
    private static Value Filter(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "filter");
        var obj = ValidationHelpers.GetObject(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to object.filter must be a function");
        
        var result = new Dictionary<string, Value>();
        foreach (var kvp in obj)
        {
            var callArgs = new List<Value> { kvp.Value, Value.String(kvp.Key) };
            var shouldInclude = CallFunction(func, callArgs);
            if (shouldInclude.Type == ValueType.Boolean && (bool)shouldInclude.Data!)
                result[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(result);
    }
    
    private static Value Reduce(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "reduce");
        var obj = ValidationHelpers.GetObject(args, 0);
        var func = args[1];
        var initialValue = args.Count > 2 ? args[2] : null;
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to object.reduce must be a function");
        
        if (obj.Count == 0 && initialValue == null)
            throw new Exception("Reduce of empty object with no initial value");
        
        var accumulator = initialValue;
        var keys = obj.Keys.ToList();
        var startIndex = initialValue != null ? 0 : 1;
        
        if (initialValue == null && keys.Count > 0)
            accumulator = obj[keys[0]];
        
        for (int i = startIndex; i < keys.Count; i++)
        {
            var key = keys[i];
            var callArgs = new List<Value> { accumulator!, obj[key], Value.String(key) };
            accumulator = CallFunction(func, callArgs);
        }
        
        return accumulator!;
    }
    
    private static Value FromEntries(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "fromEntries");
        var entries = ValidationHelpers.GetArray(args, 0);
        var result = new Dictionary<string, Value>();
        
        foreach (var entry in entries)
        {
            if (entry.Type != ValueType.Array)
                throw new Exception("fromEntries requires an array of [key, value] pairs");
            
            var pair = ValidationHelpers.GetArray(new List<Value> { entry }, 0);
            if (pair.Count != 2)
                throw new Exception("Each entry must be a [key, value] pair");
            
            var key = ValidationHelpers.GetString(new List<Value> { pair[0] }, 0);
            result[key] = pair[1];
        }
        
        return Value.Object(result);
    }
    
    private static Value CallFunction(Value func, List<Value> args)
    {
        if (func.Type == ValueType.NativeFunction)
        {
            var nativeFunc = (NativeFunctionValue)func.Data!;
            return nativeFunc.Function(args);
        }
        else if (func.Type == ValueType.Function)
        {
            // For user-defined functions, we'd need the evaluator
            // For now, return null as a placeholder
            throw new Exception("User-defined function calls not yet supported in object operations");
        }
        else
        {
            throw new Exception("Not a function");
        }
    }
    
    // Haskell Prelude functions
    private static Value Lookup(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "lookup");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        var defaultValue = args.Count > 2 ? args[2] : Value.Null();
        
        return obj.TryGetValue(key, out var value) ? value : defaultValue;
    }
    
    private static Value Member(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "member");
        var obj = ValidationHelpers.GetObject(args, 0);
        var key = ValidationHelpers.GetString(args, 1);
        
        return Value.Boolean(obj.ContainsKey(key));
    }
    
    private static Value Union(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "union");
        var result = new Dictionary<string, Value>();
        
        foreach (var arg in args)
        {
            var obj = ValidationHelpers.GetObject(new List<Value> { arg }, 0);
            foreach (var kvp in obj)
                result[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(result);
    }
    
    private static Value Intersection(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "intersection");
        if (args.Count == 0)
            return Value.Object(new Dictionary<string, Value>());
            
        var objects = args.Select(arg => ValidationHelpers.GetObject(new List<Value> { arg }, 0)).ToList();
        var firstObj = objects[0];
        var result = new Dictionary<string, Value>();
        
        foreach (var kvp in firstObj)
        {
            if (objects.All(obj => obj.ContainsKey(kvp.Key)))
                result[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(result);
    }
    
    private static Value Difference(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "difference");
        if (args.Count == 0)
            return Value.Object(new Dictionary<string, Value>());
            
        var objects = args.Select(arg => ValidationHelpers.GetObject(new List<Value> { arg }, 0)).ToList();
        var firstObj = objects[0];
        var result = new Dictionary<string, Value>();
        
        foreach (var kvp in firstObj)
        {
            if (!objects.Skip(1).Any(obj => obj.ContainsKey(kvp.Key)))
                result[kvp.Key] = kvp.Value;
        }
        
        return Value.Object(result);
    }
}
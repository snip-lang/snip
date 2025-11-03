using Snip.AST;

namespace Snip.Evaluator;

public static class ArrayModule
{
    public static Module CreateModule()
    {
        var module = new Module("array");

        // Creation
        module.Export("create", Value.NativeFunction(new NativeFunctionValue("create", Create)));
        module.Export("range", Value.NativeFunction(new NativeFunctionValue("range", Range)));
        module.Export("fill", Value.NativeFunction(new NativeFunctionValue("fill", Fill)));

        // Information
        module.Export("len", Value.NativeFunction(new NativeFunctionValue("len", ArrayLength)));
        module.Export("empty", Value.NativeFunction(new NativeFunctionValue("empty", IsEmpty)));

        // Modification
        module.Export("append", Value.NativeFunction(new NativeFunctionValue("append", Append)));
        module.Export("extend", Value.NativeFunction(new NativeFunctionValue("extend", Extend)));
        module.Export("insert", Value.NativeFunction(new NativeFunctionValue("insert", Insert)));
        module.Export("remove", Value.NativeFunction(new NativeFunctionValue("remove", Remove)));
        module.Export("pop", Value.NativeFunction(new NativeFunctionValue("pop", Pop)));
        module.Export("clear", Value.NativeFunction(new NativeFunctionValue("clear", Clear)));

        // Searching
        module.Export("index", Value.NativeFunction(new NativeFunctionValue("index", Index)));
        module.Export("contains", Value.NativeFunction(new NativeFunctionValue("contains", Contains)));
        module.Export("count", Value.NativeFunction(new NativeFunctionValue("count", Count)));

        // Manipulation
        module.Export("reverse", Value.NativeFunction(new NativeFunctionValue("reverse", Reverse)));
        module.Export("sort", Value.NativeFunction(new NativeFunctionValue("sort", Sort)));
        module.Export("slice", Value.NativeFunction(new NativeFunctionValue("slice", Slice)));
        module.Export("concat", Value.NativeFunction(new NativeFunctionValue("concat", Concat)));
        
        // Functional programming
        module.Export("map", Value.NativeFunction(new NativeFunctionValue("map", Map)));
        module.Export("filter", Value.NativeFunction(new NativeFunctionValue("filter", Filter)));
        module.Export("reduce", Value.NativeFunction(new NativeFunctionValue("reduce", Reduce)));
        module.Export("find", Value.NativeFunction(new NativeFunctionValue("find", Find)));
        module.Export("findIndex", Value.NativeFunction(new NativeFunctionValue("findIndex", FindIndex)));
        module.Export("some", Value.NativeFunction(new NativeFunctionValue("some", Some)));
        module.Export("every", Value.NativeFunction(new NativeFunctionValue("every", Every)));
        module.Export("zip", Value.NativeFunction(new NativeFunctionValue("zip", Zip)));
        module.Export("unzip", Value.NativeFunction(new NativeFunctionValue("unzip", Unzip)));
        
        // Haskell Prelude functions
        module.Export("head", Value.NativeFunction(new NativeFunctionValue("head", Head)));
        module.Export("tail", Value.NativeFunction(new NativeFunctionValue("tail", Tail)));
        module.Export("init", Value.NativeFunction(new NativeFunctionValue("init", Init)));
        module.Export("last", Value.NativeFunction(new NativeFunctionValue("last", Last)));
        module.Export("isNull", Value.NativeFunction(new NativeFunctionValue("isNull", IsNull)));
        module.Export("length", Value.NativeFunction(new NativeFunctionValue("length", Length)));
        module.Export("take", Value.NativeFunction(new NativeFunctionValue("take", Take)));
        module.Export("drop", Value.NativeFunction(new NativeFunctionValue("drop", Drop)));
        module.Export("splitAt", Value.NativeFunction(new NativeFunctionValue("splitAt", SplitAt)));
        module.Export("elem", Value.NativeFunction(new NativeFunctionValue("elem", Elem)));
        module.Export("notElem", Value.NativeFunction(new NativeFunctionValue("notElem", NotElem)));
        module.Export("lookup", Value.NativeFunction(new NativeFunctionValue("lookup", Lookup)));
        module.Export("elemIndex", Value.NativeFunction(new NativeFunctionValue("elemIndex", ElemIndex)));
        module.Export("findIndices", Value.NativeFunction(new NativeFunctionValue("findIndices", FindIndices)));
        module.Export("union", Value.NativeFunction(new NativeFunctionValue("union", Union)));
        module.Export("intersect", Value.NativeFunction(new NativeFunctionValue("intersect", Intersect)));
        module.Export("difference", Value.NativeFunction(new NativeFunctionValue("difference", Difference)));
        
        return module;
    }
    
    private static Value Create(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "create");
        var size = ValidationHelpers.GetInt(args, 0);
        var value = args.Count > 1 ? args[1] : Value.Null();
        var array = new List<Value>();
        for (int i = 0; i < size; i++)
            array.Add(value);
        return Value.Array(array);
    }
    
    private static Value Range(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 3, "range");
        var start = ValidationHelpers.GetInt(args, 0);
        var stop = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : start;
        var step = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : 1;
        
        if (args.Count == 1)
        {
            start = 0;
            stop = start;
        }
        
        var array = new List<Value>();
        for (int i = start; i < stop; i += step)
            array.Add(Value.Number(i));
        return Value.Array(array);
    }
    
    private static Value Fill(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "fill");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        for (int i = 0; i < array.Count; i++)
            array[i] = value;
        return Value.Array(array);
    }
    
    private static Value ArrayLength(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "len");
        var array = ValidationHelpers.GetArray(args, 0);
        return Value.Number(array.Count);
    }

    private static Value IsEmpty(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "empty");
        var array = ValidationHelpers.GetArray(args, 0);
        return Value.Boolean(array.Count == 0);
    }
    
    private static Value Append(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "append");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        array.Add(value);
        return Value.Array(array);
    }
    
    private static Value Extend(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "extend");
        var array = ValidationHelpers.GetArray(args, 0);
        var other = ValidationHelpers.GetArray(args, 1);
        array.AddRange(other);
        return Value.Array(array);
    }
    
    private static Value Insert(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 3, "insert");
        var array = ValidationHelpers.GetArray(args, 0);
        var index = ValidationHelpers.GetInt(args, 1);
        var value = args[2];
        array.Insert(index, value);
        return Value.Array(array);
    }
    
    private static Value Remove(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "remove");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        array.Remove(value);
        return Value.Array(array);
    }
    
    private static Value Pop(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "pop");
        var array = ValidationHelpers.GetArray(args, 0);
        var index = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : array.Count - 1;
        var value = array[index];
        array.RemoveAt(index);
        return value;
    }
    
    private static Value Clear(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "clear");
        var array = ValidationHelpers.GetArray(args, 0);
        array.Clear();
        return Value.Array(array);
    }
    
    private static Value Index(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "index");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        var index = array.IndexOf(value);
        return Value.Number(index);
    }
    
    private static Value Contains(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "contains");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        return Value.Boolean(array.Contains(value));
    }
    
    private static Value Count(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "count");
        var array = ValidationHelpers.GetArray(args, 0);
        var value = args[1];
        var count = array.Count(v => v.Equals(value));
        return Value.Number(count);
    }
    
    private static Value Reverse(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "reverse");
        var array = ValidationHelpers.GetArray(args, 0);
        array.Reverse();
        return Value.Array(array);
    }
    
    private static Value Sort(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "sort");
        var array = ValidationHelpers.GetArray(args, 0);
        array.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
        return Value.Array(array);
    }
    
    private static Value Slice(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 4, "slice");
        var array = ValidationHelpers.GetArray(args, 0);
        var start = ValidationHelpers.GetInt(args, 1);
        var end = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : array.Count;
        var step = args.Count > 3 ? ValidationHelpers.GetInt(args, 3) : 1;
        
        var result = new List<Value>();
        for (int i = start; i < end; i += step)
        {
            if (i >= 0 && i < array.Count)
                result.Add(array[i]);
        }
        return Value.Array(result);
    }
    
    private static Value Concat(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "concat");
        var result = new List<Value>();
        foreach (var arg in args)
        {
            var array = ValidationHelpers.GetArray(new List<Value> { arg }, 0);
            result.AddRange(array);
        }
        return Value.Array(result);
    }
    
    private static Value Map(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "map");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to map must be a function");
        
        var result = new List<Value>();
        for (int i = 0; i < array.Count; i++)
        {
            var callArgs = new List<Value> { array[i], Value.Number(i) };
            var mapped = CallFunction(func, callArgs);
            result.Add(mapped);
        }
        
        return Value.Array(result);
    }
    
    private static Value Filter(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "filter");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to filter must be a function");
        
        var result = new List<Value>();
        for (int i = 0; i < array.Count; i++)
        {
            var callArgs = new List<Value> { array[i], Value.Number(i) };
            var shouldInclude = CallFunction(func, callArgs);
            if (shouldInclude.Type == ValueType.Boolean && (bool)shouldInclude.Data!)
                result.Add(array[i]);
        }
        
        return Value.Array(result);
    }
    
    private static Value Reduce(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "reduce");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args[1];
        var initialValue = args.Count > 2 ? args[2] : null;
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to reduce must be a function");
        
        if (array.Count == 0 && initialValue == null)
            throw new Exception("Reduce of empty array with no initial value");
        
        var accumulator = initialValue ?? array[0];
        var startIndex = initialValue != null ? 0 : 1;
        
        for (int i = startIndex; i < array.Count; i++)
        {
            var callArgs = new List<Value> { accumulator, array[i], Value.Number(i) };
            accumulator = CallFunction(func, callArgs);
        }
        
        return accumulator;
    }
    
    private static Value Find(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "find");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to find must be a function");
        
        for (int i = 0; i < array.Count; i++)
        {
            var callArgs = new List<Value> { array[i], Value.Number(i) };
            var found = CallFunction(func, callArgs);
            if (found.Type == ValueType.Boolean && (bool)found.Data!)
                return array[i];
        }
        
        return Value.Null();
    }
    
    private static Value FindIndex(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "findIndex");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to findIndex must be a function");
        
        for (int i = 0; i < array.Count; i++)
        {
            var callArgs = new List<Value> { array[i], Value.Number(i) };
            var found = CallFunction(func, callArgs);
            if (found.Type == ValueType.Boolean && (bool)found.Data!)
                return Value.Number(i);
        }
        
        return Value.Number(-1);
    }
    
    private static Value Some(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "some");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to some must be a function");
        
        for (int i = 0; i < array.Count; i++)
        {
            var callArgs = new List<Value> { array[i], Value.Number(i) };
            var result = CallFunction(func, callArgs);
            if (result.Type == ValueType.Boolean && (bool)result.Data!)
                return Value.Boolean(true);
        }
        
        return Value.Boolean(false);
    }
    
    private static Value Every(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "every");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args[1];
        
        if (func.Type != ValueType.Function && func.Type != ValueType.NativeFunction)
            throw new Exception("Second argument to every must be a function");
        
        for (int i = 0; i < array.Count; i++)
        {
            var callArgs = new List<Value> { array[i], Value.Number(i) };
            var result = CallFunction(func, callArgs);
            if (result.Type == ValueType.Boolean && !(bool)result.Data!)
                return Value.Boolean(false);
        }
        
        return Value.Boolean(true);
    }
    
    private static Value Zip(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, -1, "zip");
        if (args.Count < 2)
            throw new Exception("zip requires at least 2 arrays");
        
        var arrays = args.Select(arg => ValidationHelpers.GetArray(new List<Value> { arg }, 0)).ToList();
        var minLength = arrays.Min(arr => arr.Count);
        var result = new List<Value>();
        
        for (int i = 0; i < minLength; i++)
        {
            var zipped = new List<Value>();
            for (int j = 0; j < arrays.Count; j++)
                zipped.Add(arrays[j][i]);
            result.Add(Value.Array(zipped));
        }
        
        return Value.Array(result);
    }
    
    private static Value Unzip(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "unzip");
        var arrays = ValidationHelpers.GetArray(args, 0);
        
        if (arrays.Count == 0)
            return Value.Array(new List<Value>());
            
        var maxLength = arrays.Max(a => a.Type == ValueType.Array ? ((List<Value>)a.Data!).Count : 0);
        var result = new List<Value>();
        
        for (int i = 0; i < maxLength; i++)
        {
            var row = new List<Value>();
            foreach (var array in arrays)
            {
                if (array.Type == ValueType.Array && i < ((List<Value>)array.Data!).Count)
                {
                    row.Add(((List<Value>)array.Data!)[i]);
                }
                else
                {
                    row.Add(Value.Null());
                }
            }
            result.Add(Value.Array(row));
        }
        
        return Value.Array(result);
    }
    
    // Haskell Prelude functions
    private static Value Head(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "head");
        var array = ValidationHelpers.GetArray(args, 0);
        
        if (array.Count == 0)
            throw new Exception("head: empty array");
            
        return array[0];
    }
    
    private static Value Tail(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "tail");
        var array = ValidationHelpers.GetArray(args, 0);
        
        if (array.Count == 0)
            return Value.Array(new List<Value>());
            
        return Value.Array(array.Skip(1).ToList());
    }
    
    private static Value Init(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "init");
        var array = ValidationHelpers.GetArray(args, 0);
        
        if (array.Count == 0)
            return Value.Array(new List<Value>());
            
        return Value.Array(array.Take(array.Count - 1).ToList());
    }
    
    private static Value Last(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "last");
        var array = ValidationHelpers.GetArray(args, 0);
        
        if (array.Count == 0)
            throw new Exception("last: empty array");
            
        return array[array.Count - 1];
    }
    
    private static Value IsNull(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isNull");
        var array = ValidationHelpers.GetArray(args, 0);
        return Value.Boolean(array.Count == 0);
    }
    
    private static Value Length(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "length");
        var array = ValidationHelpers.GetArray(args, 0);
        return Value.Number(array.Count);
    }
    
    private static Value Take(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "take");
        var n = ValidationHelpers.GetInt(args, 0);
        var array = ValidationHelpers.GetArray(args, 1);
        
        n = n < 0 ? array.Count + n : n;
        n = Math.Min(n, array.Count);
        
        return Value.Array(array.Take(n).ToList());
    }
    
    private static Value Drop(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "drop");
        var n = ValidationHelpers.GetInt(args, 0);
        var array = ValidationHelpers.GetArray(args, 1);
        
        n = n < 0 ? array.Count + n : n;
        n = Math.Min(n, array.Count);
        
        return Value.Array(array.Skip(n).ToList());
    }
    
    private static Value SplitAt(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "splitAt");
        var n = ValidationHelpers.GetInt(args, 0);
        var array = ValidationHelpers.GetArray(args, 1);
        
        if (n < 0)
            n = array.Count + n;
        n = Math.Min(n, array.Count);
        
        var first = array.Take(n).ToList();
        var second = array.Skip(n).ToList();
        
        return Value.Array(new List<Value> { Value.Array(first), Value.Array(second) });
    }
    
    private static Value Elem(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "elem");
        var target = args[0];
        var array = ValidationHelpers.GetArray(args, 1);
        
        return Value.Boolean(array.Any(item => item.Equals(target)));
    }
    
    private static Value NotElem(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "notElem");
        var target = args[0];
        var array = ValidationHelpers.GetArray(args, 1);
        
        return Value.Boolean(!array.Any(item => item.Equals(target)));
    }
    
    private static Value Lookup(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "lookup");
        var array = ValidationHelpers.GetArray(args, 0);
        var target = args[1];
        var defaultValue = args.Count > 2 ? args[2] : Value.Null();
        
        var index = array.FindIndex(item => item.Equals(target));
        return index >= 0 ? array[index] : defaultValue;
    }
    
    private static Value ElemIndex(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "elemIndex");
        var target = args[0];
        var array = ValidationHelpers.GetArray(args, 1);
        
        var index = array.FindIndex(item => item.Equals(target));
        return Value.Number(index >= 0 ? index : -1);
    }
    
    private static Value FindIndices(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "findIndices");
        var target = args[0];
        var array = ValidationHelpers.GetArray(args, 1);
        
        var indices = new List<Value>();
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i].Equals(target))
            {
                indices.Add(Value.Number(i));
            }
        }
        return Value.Array(indices);
    }
    
    private static Value Union(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "union");
        var result = new List<Value>();
        var seen = new HashSet<Value>();
        
        foreach (var arg in args)
        {
            if (arg.Type == ValueType.Array)
            {
                var array = ValidationHelpers.GetArray(new List<Value> { arg }, 0);
                foreach (var item in array)
                {
                    if (!seen.Any(s => s.Equals(item)))
                    {
                        result.Add(item);
                        seen.Add(item);
                    }
                }
            }
            else
            {
                if (!seen.Any(s => s.Equals(arg)))
                {
                    result.Add(arg);
                    seen.Add(arg);
                }
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value Intersect(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "intersect");
        if (args.Count == 0)
            return Value.Array(new List<Value>());
            
        var arrays = args.Select(arg => arg.Type == ValueType.Array ? ValidationHelpers.GetArray(new List<Value> { arg }, 0) : new List<Value> { arg }).ToList();
        var firstArray = arrays[0];
        
        var result = new List<Value>();
        foreach (var item in firstArray)
        {
            if (arrays.All(arr => arr.Contains(item)))
                result.Add(item);
        }
        
        return Value.Array(result);
    }
    
    private static Value Difference(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "difference");
        if (args.Count == 0)
            return Value.Array(new List<Value>());
            
        var arrays = args.Select(arg => arg.Type == ValueType.Array ? ValidationHelpers.GetArray(new List<Value> { arg }, 0) : new List<Value> { arg }).ToList();
        var firstArray = arrays[0];
        
        var result = new List<Value>();
        foreach (var item in firstArray)
        {
            if (!arrays.Skip(1).Any(arr => arr.Contains(item)))
                result.Add(item);
        }
        
        return Value.Array(result);
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
            throw new Exception("User-defined function calls not yet supported in array operations");
        }
        else
        {
            throw new Exception("Not a function");
        }
    }
}
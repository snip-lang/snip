using Snip.AST;

namespace Snip.Evaluator;

public static class ItertoolsModule
{
    public static Module CreateModule()
    {
        var module = new Module("itertools");
        
        // Infinite iterators
        module.Export("count", Value.NativeFunction(new NativeFunctionValue("count", Count)));
        module.Export("cycle", Value.NativeFunction(new NativeFunctionValue("cycle", Cycle)));
        module.Export("repeat", Value.NativeFunction(new NativeFunctionValue("repeat", Repeat)));
        
        // Iterators terminating on shortest input sequence
        module.Export("accumulate", Value.NativeFunction(new NativeFunctionValue("accumulate", Accumulate)));
        module.Export("chain", Value.NativeFunction(new NativeFunctionValue("chain", Chain)));
        module.Export("chain_from_iterable", Value.NativeFunction(new NativeFunctionValue("chain_from_iterable", ChainFromIterable)));
        module.Export("compress", Value.NativeFunction(new NativeFunctionValue("compress", Compress)));
        module.Export("dropwhile", Value.NativeFunction(new NativeFunctionValue("dropwhile", DropWhile)));
        module.Export("filterfalse", Value.NativeFunction(new NativeFunctionValue("filterfalse", FilterFalse)));
        module.Export("groupby", Value.NativeFunction(new NativeFunctionValue("groupby", GroupBy)));
        module.Export("islice", Value.NativeFunction(new NativeFunctionValue("islice", Islice)));
        module.Export("starmap", Value.NativeFunction(new NativeFunctionValue("starmap", StarMap)));
        module.Export("takewhile", Value.NativeFunction(new NativeFunctionValue("takewhile", TakeWhile)));
        module.Export("tee", Value.NativeFunction(new NativeFunctionValue("tee", Tee)));
        module.Export("zip_longest", Value.NativeFunction(new NativeFunctionValue("zip_longest", ZipLongest)));
        module.Export("product", Value.NativeFunction(new NativeFunctionValue("product", Product)));
        module.Export("permutations", Value.NativeFunction(new NativeFunctionValue("permutations", Permutations)));
        module.Export("combinations", Value.NativeFunction(new NativeFunctionValue("combinations", Combinations)));
        module.Export("combinations_with_replacement", Value.NativeFunction(new NativeFunctionValue("combinations_with_replacement", CombinationsWithReplacement)));
        
        // Combinatoric iterators
        module.Export("product", Value.NativeFunction(new NativeFunctionValue("product", Product)));
        module.Export("permutations", Value.NativeFunction(new NativeFunctionValue("permutations", Permutations)));
        module.Export("combinations", Value.NativeFunction(new NativeFunctionValue("combinations", Combinations)));
        module.Export("combinations_with_replacement", Value.NativeFunction(new NativeFunctionValue("combinations_with_replacement", CombinationsWithReplacement)));
        
        return module;
    }
    
    private static Value Count(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 2, "count");
        var start = args.Count > 0 ? ValidationHelpers.GetNumber(args, 0) : 0;
        var step = args.Count > 1 ? ValidationHelpers.GetNumber(args, 1) : 1;
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["_type"] = Value.String("count"),
            ["_start"] = Value.Number(start),
            ["_step"] = Value.Number(step),
            ["__iter__"] = Value.NativeFunction(new NativeFunctionValue("__iter__", (_) => Value.Null())),
            ["__next__"] = Value.NativeFunction(new NativeFunctionValue("__next__", (_) => {
                var current = start;
                start += step;
                return Value.Number(current);
            }))
        });
    }
    
    private static Value Cycle(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "cycle");
        var iterable = args[0];
        var array = iterable.Type == ValueType.Array ? ValidationHelpers.GetArray(args, 0) : new List<Value>();
        var index = 0;
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["_type"] = Value.String("cycle"),
            ["_iterable"] = iterable,
            ["_index"] = Value.Number(index),
            ["__iter__"] = Value.NativeFunction(new NativeFunctionValue("__iter__", (_) => Value.Null())),
            ["__next__"] = Value.NativeFunction(new NativeFunctionValue("__next__", (_) => {
                if (array.Count == 0)
                    throw new Exception("Cannot cycle empty iterable");
                    
                if (index >= array.Count)
                    index = 0;
                    
                return array[index++];
            }))
        });
    }
    
    private static Value Repeat(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "repeat");
        var element = args[0];
        var times = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : -1;
        var count = 0;
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["_type"] = Value.String("repeat"),
            ["_element"] = element,
            ["_times"] = Value.Number(times),
            ["_count"] = Value.Number(count),
            ["__iter__"] = Value.NativeFunction(new NativeFunctionValue("__iter__", (_) => Value.Null())),
            ["__next__"] = Value.NativeFunction(new NativeFunctionValue("__next__", (_) => {
                if (times >= 0 && count >= times)
                    throw new Exception("StopIteration");
                    
                count++;
                return element;
            }))
        });
    }
    
    private static Value Accumulate(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "accumulate");
        var iterable = args[0];
        var func = args.Count > 1 ? args[1] : null;
        
        var array = iterable.Type == ValueType.Array ? ValidationHelpers.GetArray(args, 0) : new List<Value>();
        var result = new List<Value>();
        
        if (array.Count == 0) return Value.Array(result);
        
        if (func != null && func.Type == ValueType.NativeFunction)
        {
            var accumulator = array[0];
            result.Add(accumulator);
            
            for (int i = 1; i < array.Count; i++)
            {
                var funcResult = ((NativeFunctionValue)func.Data!).Function(new List<Value> { accumulator, array[i] });
                accumulator = funcResult;
                result.Add(accumulator);
            }
        }
        else
        {
            var sum = 0.0;
            for (int i = 0; i < array.Count; i++)
            {
                sum += ValidationHelpers.GetNumber(new List<Value> { array[i] }, 0);
                result.Add(Value.Number(sum));
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value Chain(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "chain");
        var result = new List<Value>();
        
        foreach (var arg in args)
        {
            if (arg.Type == ValueType.Array)
            {
                var array = ValidationHelpers.GetArray(new List<Value> { arg }, 0);
                result.AddRange(array);
            }
            else
            {
                result.Add(arg);
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value ChainFromIterable(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "chain_from_iterable");
        var iterables = args[0];
        var result = new List<Value>();
        
        if (iterables.Type == ValueType.Array)
        {
            var arrays = ValidationHelpers.GetArray(args, 0);
            foreach (var iterable in arrays)
            {
                if (iterable.Type == ValueType.Array)
                {
                    var array = ValidationHelpers.GetArray(new List<Value> { iterable }, 0);
                    result.AddRange(array);
                }
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value Compress(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "compress");
        var data = ValidationHelpers.GetArray(args, 0);
        var selectors = ValidationHelpers.GetArray(args, 1);
        
        var result = new List<Value>();
        for (int i = 0; i < Math.Min(data.Count, selectors.Count); i++)
        {
            if (ValidationHelpers.GetBoolean(new List<Value> { selectors[i] }, 0))
            {
                result.Add(data[i]);
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value DropWhile(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "dropwhile");
        var predicate = args[0];
        var iterable = ValidationHelpers.GetArray(args, 1);
        
        var result = new List<Value>();
        var dropping = true;
        
        foreach (var item in iterable)
        {
            if (dropping)
            {
                var shouldDrop = false;
                
                if (predicate.Type == ValueType.NativeFunction)
                {
                    var predicateResult = ((NativeFunctionValue)predicate.Data!).Function(new List<Value> { item });
                    shouldDrop = predicateResult.Type == ValueType.Boolean && (bool)predicateResult.Data!;
                }
                else
                {
                    shouldDrop = ValidationHelpers.GetBoolean(new List<Value> { item }, 0);
                }
                
                if (!shouldDrop)
                {
                    dropping = false;
                    result.Add(item);
                }
            }
            else
            {
                result.Add(item);
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value FilterFalse(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "filterfalse");
        var predicate = args[0];
        var iterable = ValidationHelpers.GetArray(args, 1);
        
        var result = new List<Value>();
        foreach (var item in iterable)
        {
            var shouldInclude = false;
            
            if (predicate.Type == ValueType.NativeFunction)
            {
                var predicateResult = ((NativeFunctionValue)predicate.Data!).Function(new List<Value> { item });
                shouldInclude = predicateResult.Type == ValueType.Boolean && !(bool)predicateResult.Data!;
            }
            else
            {
                shouldInclude = !ValidationHelpers.GetBoolean(new List<Value> { item }, 0);
            }
            
            if (shouldInclude)
            {
                result.Add(item);
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value GroupBy(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "groupby");
        var iterable = ValidationHelpers.GetArray(args, 0);
        var keyFunc = args.Count > 1 ? args[1] : null;
        
        var groups = new Dictionary<string, List<Value>>();
        
        foreach (var item in iterable)
        {
            var key = keyFunc != null && keyFunc.Type == ValueType.NativeFunction
                ? ((NativeFunctionValue)keyFunc.Data!).Function(new List<Value> { item }).ToString()
                : item.ToString();
            
            if (!groups.ContainsKey(key))
            {
                groups[key] = new List<Value>();
            }
            groups[key].Add(item);
        }
        
        var result = new List<Value>();
        foreach (var kvp in groups)
        {
            result.Add(Value.Array(new List<Value>
            {
                Value.String(kvp.Key),
                Value.Array(kvp.Value)
            }));
        }
        
        return Value.Array(result);
    }
    
    private static Value Islice(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 4, "islice");
        var iterable = ValidationHelpers.GetArray(args, 0);
        var start = ValidationHelpers.GetInt(args, 1);
        var stop = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : iterable.Count;
        var step = args.Count > 3 ? ValidationHelpers.GetInt(args, 3) : 1;
        
        var result = new List<Value>();
        for (int i = start; i < stop && i < iterable.Count; i += step)
        {
            result.Add(iterable[i]);
        }
        
        return Value.Array(result);
    }
    
    private static Value StarMap(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "starmap");
        var func = args[0];
        var iterable = ValidationHelpers.GetArray(args, 1);
        
        var result = new List<Value>();
        foreach (var item in iterable)
        {
            if (item.Type == ValueType.Array)
            {
                var itemArray = ValidationHelpers.GetArray(new List<Value> { item }, 0);
                if (func.Type == ValueType.NativeFunction)
                {
                    var funcResult = ((NativeFunctionValue)func.Data!).Function(itemArray);
                    result.Add(funcResult);
                }
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value TakeWhile(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "takewhile");
        var predicate = args[0];
        var iterable = ValidationHelpers.GetArray(args, 1);
        
        var result = new List<Value>();
        foreach (var item in iterable)
        {
            var shouldContinue = false;
            
            if (predicate.Type == ValueType.NativeFunction)
            {
                var predicateResult = ((NativeFunctionValue)predicate.Data!).Function(new List<Value> { item });
                shouldContinue = predicateResult.Type == ValueType.Boolean && (bool)predicateResult.Data!;
            }
            else
            {
                shouldContinue = ValidationHelpers.GetBoolean(new List<Value> { item }, 0);
            }
            
            if (shouldContinue)
            {
                result.Add(item);
            }
            else
            {
                break;
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value Tee(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "tee");
        var iterable = ValidationHelpers.GetArray(args, 0);
        var n = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : 2;
        
        var result = new List<Value>();
        for (int i = 0; i < n; i++)
        {
            result.Add(Value.Array(new List<Value>(iterable)));
        }
        
        return Value.Array(result);
    }
    
    private static Value ZipLongest(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "zip_longest");
        var fillvalue = args.Count > 0 ? args[0] : Value.Null();
        var arrays = new List<List<Value>>();
        
        for (int i = 1; i < args.Count; i++)
        {
            if (args[i].Type == ValueType.Array)
            {
                arrays.Add(ValidationHelpers.GetArray(args, i));
            }
        }
        
        if (arrays.Count == 0) return Value.Array(new List<Value>());
        
        var maxLength = arrays.Max(a => a.Count);
        var result = new List<Value>();
        
        for (int i = 0; i < maxLength; i++)
        {
            var row = new List<Value>();
            foreach (var array in arrays)
            {
                if (i < array.Count)
                {
                    row.Add(array[i]);
                }
                else
                {
                    row.Add(fillvalue);
                }
            }
            result.Add(Value.Array(row));
        }
        
        return Value.Array(result);
    }
    
    private static Value Product(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "product");
        var arrays = new List<List<Value>>();
        
        foreach (var arg in args)
        {
            if (arg.Type == ValueType.Array)
            {
                arrays.Add(ValidationHelpers.GetArray(new List<Value> { arg }, 0));
            }
        }
        
        var result = new List<Value>();
        CartesianProduct(arrays, 0, new List<Value>(), result);
        
        return Value.Array(result);
    }
    
    private static void CartesianProduct(List<List<Value>> arrays, int index, List<Value> current, List<Value> result)
    {
        if (index == arrays.Count)
        {
            result.Add(Value.Array(new List<Value>(current)));
            return;
        }
        
        foreach (var item in arrays[index])
        {
            current.Add(item);
            CartesianProduct(arrays, index + 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }
    
    private static Value Permutations(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "permutations");
        var array = ValidationHelpers.GetArray(args, 0);
        var r = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : array.Count;
        
        var result = new List<Value>();
        GeneratePermutations(array, 0, r, new List<Value>(), new HashSet<int>(), result);
        
        return Value.Array(result);
    }
    
    private static void GeneratePermutations(List<Value> array, int depth, int r, List<Value> current, HashSet<int> used, List<Value> result)
    {
        if (depth == r)
        {
            result.Add(Value.Array(new List<Value>(current)));
            return;
        }
        
        for (int i = 0; i < array.Count; i++)
        {
            if (!used.Contains(i))
            {
                used.Add(i);
                current.Add(array[i]);
                GeneratePermutations(array, depth + 1, r, current, used, result);
                current.RemoveAt(current.Count - 1);
                used.Remove(i);
            }
        }
    }
    
    private static Value Combinations(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "combinations");
        var array = ValidationHelpers.GetArray(args, 0);
        var r = ValidationHelpers.GetInt(args, 1);
        
        var result = new List<Value>();
        GenerateCombinations(array, 0, r, new List<Value>(), result);
        
        return Value.Array(result);
    }
    
    private static void GenerateCombinations(List<Value> array, int start, int r, List<Value> current, List<Value> result)
    {
        if (current.Count == r)
        {
            result.Add(Value.Array(new List<Value>(current)));
            return;
        }
        
        for (int i = start; i < array.Count; i++)
        {
            current.Add(array[i]);
            GenerateCombinations(array, i + 1, r, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }
    
    private static Value CombinationsWithReplacement(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "combinations_with_replacement");
        var array = ValidationHelpers.GetArray(args, 0);
        var r = ValidationHelpers.GetInt(args, 1);
        
        var result = new List<Value>();
        GenerateCombinationsWithReplacement(array, 0, r, new List<Value>(), result);
        
        return Value.Array(result);
    }
    
    private static void GenerateCombinationsWithReplacement(List<Value> array, int start, int r, List<Value> current, List<Value> result)
    {
        if (current.Count == r)
        {
            result.Add(Value.Array(new List<Value>(current)));
            return;
        }
        
        for (int i = start; i < array.Count; i++)
        {
            current.Add(array[i]);
            GenerateCombinationsWithReplacement(array, i, r, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }
}
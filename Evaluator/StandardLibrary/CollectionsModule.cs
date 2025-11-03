using Snip.AST;

namespace Snip.Evaluator;

public static class CollectionsModule
{
    public static Module CreateModule()
    {
        var module = new Module("collections");
        
        // Specialized containers
        module.Export("namedtuple", Value.NativeFunction(new NativeFunctionValue("namedtuple", NamedTuple)));
        module.Export("defaultdict", Value.NativeFunction(new NativeFunctionValue("defaultdict", DefaultDict)));
        module.Export("counter", Value.NativeFunction(new NativeFunctionValue("counter", Counter)));
        module.Export("deque", Value.NativeFunction(new NativeFunctionValue("deque", Deque)));
        module.Export("ordereddict", Value.NativeFunction(new NativeFunctionValue("ordereddict", OrderedDict)));
        
        // Chain operations
        module.Export("chain", Value.NativeFunction(new NativeFunctionValue("chain", Chain)));
        module.Export("chain_from_iterable", Value.NativeFunction(new NativeFunctionValue("chain_from_iterable", ChainFromIterable)));
        
        // Combinatorics
        module.Export("product", Value.NativeFunction(new NativeFunctionValue("product", Product)));
        module.Export("permutations", Value.NativeFunction(new NativeFunctionValue("permutations", Permutations)));
        module.Export("combinations", Value.NativeFunction(new NativeFunctionValue("combinations", Combinations)));
        module.Export("combinations_with_replacement", Value.NativeFunction(new NativeFunctionValue("combinations_with_replacement", CombinationsWithReplacement)));
        
        // Iteration tools
        module.Export("accumulate", Value.NativeFunction(new NativeFunctionValue("accumulate", Accumulate)));
        module.Export("groupby", Value.NativeFunction(new NativeFunctionValue("groupby", GroupBy)));
        module.Export("zip_longest", Value.NativeFunction(new NativeFunctionValue("zip_longest", ZipLongest)));
        module.Export("tee", Value.NativeFunction(new NativeFunctionValue("tee", Tee)));
        
        // Filtering and selection
        module.Export("filterfalse", Value.NativeFunction(new NativeFunctionValue("filterfalse", FilterFalse)));
        module.Export("islice", Value.NativeFunction(new NativeFunctionValue("islice", Islice)));
        module.Export("takewhile", Value.NativeFunction(new NativeFunctionValue("takewhile", TakeWhile)));
        module.Export("dropwhile", Value.NativeFunction(new NativeFunctionValue("dropwhile", DropWhile)));
        module.Export("compress", Value.NativeFunction(new NativeFunctionValue("compress", Compress)));
        
        return module;
    }
    
    private static Value NamedTuple(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "namedtuple");
        var name = ValidationHelpers.GetString(args, 0);
        var fieldNames = ValidationHelpers.GetArray(args, 1);
        
        var fields = new List<string>();
        foreach (var field in fieldNames)
        {
            fields.Add(ValidationHelpers.GetString(new List<Value> { field }, 0));
        }
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["name"] = Value.String(name),
            ["fields"] = Value.Array(fieldNames),
            ["_create"] = Value.NativeFunction(new NativeFunctionValue("_create", (createArgs) => {
                ValidationHelpers.ValidateArgs(createArgs, fields.Count, fields.Count, "namedtuple_create");
                var obj = new Dictionary<string, Value>();
                for (int i = 0; i < fields.Count; i++)
                {
                    obj[fields[i]] = createArgs[i];
                }
                obj["_type"] = Value.String("namedtuple");
                obj["_name"] = Value.String(name);
                return Value.Object(obj);
            }))
        });
    }
    
    private static Value DefaultDict(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "defaultdict");
        var defaultValue = args[0];
        var initialData = args.Count > 1 ? ValidationHelpers.GetObject(args, 1) : new Dictionary<string, Value>();
        
        var data = new Dictionary<string, Value>(initialData);
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["_default"] = defaultValue,
            ["_data"] = Value.Object(data),
            ["get"] = Value.NativeFunction(new NativeFunctionValue("get", (getArgs) => {
                ValidationHelpers.ValidateArgs(getArgs, 1, 2, "defaultdict_get");
                var key = ValidationHelpers.GetString(getArgs, 0);
                if (data.ContainsKey(key))
                {
                    return data[key];
                }
                else if (getArgs.Count > 1)
                {
                    return getArgs[1];
                }
                else
                {
                    return defaultValue;
                }
            })),
            ["set"] = Value.NativeFunction(new NativeFunctionValue("set", (setArgs) => {
                ValidationHelpers.ValidateArgs(setArgs, 2, 2, "defaultdict_set");
                var key = ValidationHelpers.GetString(setArgs, 0);
                data[key] = setArgs[1];
                return Value.Null();
            })),
            ["keys"] = Value.NativeFunction(new NativeFunctionValue("keys", (_) => {
                var keys = data.Keys.Select(k => Value.String(k)).ToList();
                return Value.Array(keys);
            })),
            ["values"] = Value.NativeFunction(new NativeFunctionValue("values", (_) => {
                var values = data.Values.ToList();
                return Value.Array(values);
            }))
        });
    }
    
    private static Value Counter(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 1, "counter");
        var data = new Dictionary<string, double>();
        
        if (args.Count > 0)
        {
            if (args[0].Type == ValueType.Array)
            {
                var array = ValidationHelpers.GetArray(args, 0);
                foreach (var item in array)
                {
                    var key = item.ToString();
                    data[key] = data.GetValueOrDefault(key, 0) + 1;
                }
            }
            else if (args[0].Type == ValueType.Object)
            {
                var obj = ValidationHelpers.GetObject(args, 0);
                foreach (var kvp in obj)
                {
                    if (kvp.Value.Type == ValueType.Number)
                    {
                        data[kvp.Key] = ValidationHelpers.GetNumber(new List<Value> { kvp.Value }, 0);
                    }
                    else
                    {
                        data[kvp.Key] = 1;
                    }
                }
            }
        }
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["_data"] = Value.Object(data.ToDictionary(kvp => kvp.Key, kvp => Value.Number(kvp.Value))),
            ["get"] = Value.NativeFunction(new NativeFunctionValue("get", (getArgs) => {
                ValidationHelpers.ValidateArgs(getArgs, 1, 2, "counter_get");
                var key = ValidationHelpers.GetString(getArgs, 0);
                var defaultValue = getArgs.Count > 1 ? ValidationHelpers.GetNumber(getArgs, 1) : 0;
                return Value.Number(data.GetValueOrDefault(key, defaultValue));
            })),
            ["increment"] = Value.NativeFunction(new NativeFunctionValue("increment", (incArgs) => {
                ValidationHelpers.ValidateArgs(incArgs, 1, 2, "counter_increment");
                var key = ValidationHelpers.GetString(incArgs, 0);
                var amount = incArgs.Count > 1 ? ValidationHelpers.GetNumber(incArgs, 1) : 1;
                data[key] = data.GetValueOrDefault(key, 0) + amount;
                return Value.Number(data[key]);
            })),
            ["most_common"] = Value.NativeFunction(new NativeFunctionValue("most_common", (mcArgs) => {
                ValidationHelpers.ValidateArgs(mcArgs, 0, 1, "counter_most_common");
                var n = mcArgs.Count > 0 ? ValidationHelpers.GetInt(mcArgs, 0) : data.Count;
                var sorted = data.OrderByDescending(kvp => kvp.Value).Take(n);
                var result = new List<Value>();
                foreach (var kvp in sorted)
                {
                    result.Add(Value.Array(new List<Value> { Value.String(kvp.Key), Value.Number(kvp.Value) }));
                }
                return Value.Array(result);
            })),
            ["keys"] = Value.NativeFunction(new NativeFunctionValue("keys", (_) => {
                var keys = data.Keys.Select(k => Value.String(k)).ToList();
                return Value.Array(keys);
            })),
            ["values"] = Value.NativeFunction(new NativeFunctionValue("values", (_) => {
                var values = data.Values.Select(v => Value.Number(v)).ToList();
                return Value.Array(values);
            }))
        });
    }
    
    private static Value Deque(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 2, "deque");
        var data = args.Count > 0 ? ValidationHelpers.GetArray(args, 0) : new List<Value>();
        var maxLength = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : -1;
        
        var deque = new List<Value>(data);
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["_data"] = Value.Array(deque),
            ["_max_length"] = Value.Number(maxLength),
            ["append"] = Value.NativeFunction(new NativeFunctionValue("append", (appendArgs) => {
                ValidationHelpers.ValidateArgs(appendArgs, 1, 1, "deque_append");
                if (maxLength > 0 && deque.Count >= maxLength)
                {
                    deque.RemoveAt(0);
                }
                deque.Add(appendArgs[0]);
                return Value.Null();
            })),
            ["appendleft"] = Value.NativeFunction(new NativeFunctionValue("appendleft", (appendArgs) => {
                ValidationHelpers.ValidateArgs(appendArgs, 1, 1, "deque_appendleft");
                if (maxLength > 0 && deque.Count >= maxLength)
                {
                    deque.RemoveAt(deque.Count - 1);
                }
                deque.Insert(0, appendArgs[0]);
                return Value.Null();
            })),
            ["pop"] = Value.NativeFunction(new NativeFunctionValue("pop", (_) => {
                if (deque.Count == 0)
                    throw new Exception("Deque is empty");
                var item = deque[deque.Count - 1];
                deque.RemoveAt(deque.Count - 1);
                return item;
            })),
            ["popleft"] = Value.NativeFunction(new NativeFunctionValue("popleft", (_) => {
                if (deque.Count == 0)
                    throw new Exception("Deque is empty");
                var item = deque[0];
                deque.RemoveAt(0);
                return item;
            })),
            ["count"] = Value.NativeFunction(new NativeFunctionValue("count", (countArgs) => {
                ValidationHelpers.ValidateArgs(countArgs, 1, 1, "deque_count");
                var target = countArgs[0];
                return Value.Number(deque.Count(item => item.Equals(target)));
            })),
            ["rotate"] = Value.NativeFunction(new NativeFunctionValue("rotate", (rotateArgs) => {
                ValidationHelpers.ValidateArgs(rotateArgs, 1, 1, "deque_rotate");
                var n = ValidationHelpers.GetInt(rotateArgs, 0);
                if (deque.Count > 0)
                {
                    n = n % deque.Count;
                    if (n > 0)
                    {
                        var temp = deque.Skip(deque.Count - n).Take(n).ToList();
                        deque.RemoveRange(deque.Count - n, n);
                        deque.InsertRange(0, temp);
                    }
                    else if (n < 0)
                    {
                        var temp = deque.Take(Math.Abs(n)).ToList();
                        deque.RemoveRange(0, Math.Abs(n));
                        deque.AddRange(temp);
                    }
                }
                return Value.Null();
            })),
            ["to_array"] = Value.NativeFunction(new NativeFunctionValue("to_array", (_) => {
                return Value.Array(new List<Value>(deque));
            }))
        });
    }
    
    private static Value OrderedDict(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 1, "ordereddict");
        var data = args.Count > 0 ? ValidationHelpers.GetObject(args, 0) : new Dictionary<string, Value>();
        var orderedKeys = new List<string>(data.Keys);
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["_data"] = Value.Object(data),
            ["_keys"] = Value.Array(orderedKeys.Select(k => Value.String(k)).ToList()),
            ["set"] = Value.NativeFunction(new NativeFunctionValue("set", (setArgs) => {
                ValidationHelpers.ValidateArgs(setArgs, 2, 2, "ordereddict_set");
                var key = ValidationHelpers.GetString(setArgs, 0);
                data[key] = setArgs[1];
                if (!orderedKeys.Contains(key))
                {
                    orderedKeys.Add(key);
                }
                return Value.Null();
            })),
            ["get"] = Value.NativeFunction(new NativeFunctionValue("get", (getArgs) => {
                ValidationHelpers.ValidateArgs(getArgs, 1, 2, "ordereddict_get");
                var key = ValidationHelpers.GetString(getArgs, 0);
                if (data.ContainsKey(key))
                {
                    return data[key];
                }
                return getArgs.Count > 1 ? getArgs[1] : Value.Null();
            })),
            ["keys"] = Value.NativeFunction(new NativeFunctionValue("keys", (_) => {
                return Value.Array(orderedKeys.Select(k => Value.String(k)).ToList());
            })),
            ["values"] = Value.NativeFunction(new NativeFunctionValue("values", (_) => {
                var values = orderedKeys.Where(k => data.ContainsKey(k)).Select(k => data[k]).ToList();
                return Value.Array(values);
            })),
            ["move_to_end"] = Value.NativeFunction(new NativeFunctionValue("move_to_end", (moveArgs) => {
                ValidationHelpers.ValidateArgs(moveArgs, 1, 2, "ordereddict_move_to_end");
                var key = ValidationHelpers.GetString(moveArgs, 0);
                var last = moveArgs.Count > 1 ? ValidationHelpers.GetBoolean(moveArgs, 1) : true;
                
                if (orderedKeys.Contains(key))
                {
                    orderedKeys.Remove(key);
                    if (last)
                    {
                        orderedKeys.Add(key);
                    }
                    else
                    {
                        orderedKeys.Insert(0, key);
                    }
                }
                return Value.Null();
            }))
        });
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
        var iterables = ValidationHelpers.GetArray(args, 0);
        var result = new List<Value>();
        
        foreach (var iterable in iterables)
        {
            if (iterable.Type == ValueType.Array)
            {
                var array = ValidationHelpers.GetArray(new List<Value> { iterable }, 0);
                result.AddRange(array);
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value Product(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "product");
        var repeat = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : 1;
        var arrays = new List<List<Value>>();
        
        for (int i = 0; i < Math.Min(args.Count, 1); i++)
        {
            if (args[i].Type == ValueType.Array)
            {
                arrays.Add(ValidationHelpers.GetArray(args, i));
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
        ValidationHelpers.ValidateArgs(args, 1, 2, "combinations");
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
        ValidationHelpers.ValidateArgs(args, 1, 2, "combinations_with_replacement");
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
    
    private static Value Accumulate(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "accumulate");
        var array = ValidationHelpers.GetArray(args, 0);
        var func = args.Count > 1 ? args[1] : null;
        
        var result = new List<Value>();
        if (array.Count == 0) return Value.Array(result);
        
        if (func != null && func.Type == ValueType.NativeFunction)
        {
            // Custom function accumulation
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
            // Default sum accumulation
            var sum = 0.0;
            foreach (var item in array)
            {
                sum += ValidationHelpers.GetNumber(new List<Value> { item }, 0);
                result.Add(Value.Number(sum));
            }
        }
        
        return Value.Array(result);
    }
    
    private static Value GroupBy(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "groupby");
        var array = ValidationHelpers.GetArray(args, 0);
        var keyFunc = args.Count > 1 ? args[1] : null;
        
        var groups = new Dictionary<string, List<Value>>();
        
        foreach (var item in array)
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
    
    private static Value ZipLongest(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 10, "zip_longest");
        var fillvalue = args.Count > 1 ? args[1] : Value.Null();
        var arrays = new List<List<Value>>();
        
        for (int i = 0; i < args.Count; i++)
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
    
    private static Value Tee(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "tee");
        var array = ValidationHelpers.GetArray(args, 0);
        var n = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : 2;
        
        var result = new List<Value>();
        for (int i = 0; i < n; i++)
        {
            result.Add(Value.Array(new List<Value>(array)));
        }
        
        return Value.Array(result);
    }
    
    private static Value FilterFalse(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "filterfalse");
        var predicate = args[0];
        var iterable = args.Count > 1 ? ValidationHelpers.GetArray(args, 1) : new List<Value>();
        
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
}
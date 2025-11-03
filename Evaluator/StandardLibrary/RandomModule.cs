using Snip.AST;

namespace Snip.Evaluator;

public static class RandomModule
{
    private static System.Random _random = new();
    
    public static Module CreateModule()
    {
        var module = new Module("random");
        
        // Basic random functions
        module.Export("random", Value.NativeFunction(new NativeFunctionValue("random", Random)));
        module.Export("randint", Value.NativeFunction(new NativeFunctionValue("randint", RandInt)));
        module.Export("randfloat", Value.NativeFunction(new NativeFunctionValue("randfloat", RandFloat)));
        module.Export("choice", Value.NativeFunction(new NativeFunctionValue("choice", Choice)));
        module.Export("shuffle", Value.NativeFunction(new NativeFunctionValue("shuffle", Shuffle)));
        module.Export("sample", Value.NativeFunction(new NativeFunctionValue("sample", Sample)));
        module.Export("seed", Value.NativeFunction(new NativeFunctionValue("seed", Seed)));
        
        // Weighted random
        module.Export("weighted_choice", Value.NativeFunction(new NativeFunctionValue("weighted_choice", WeightedChoice)));
        module.Export("normal", Value.NativeFunction(new NativeFunctionValue("normal", Normal)));
        module.Export("uniform", Value.NativeFunction(new NativeFunctionValue("uniform", Uniform)));
        
        return module;
    }
    
    private static Value Random(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 0, 0, "random");
        return Value.Number(_random.NextDouble());
    }
    
    private static Value RandInt(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "randint");
        var min = ValidationHelpers.GetInt(args, 0);
        var max = ValidationHelpers.GetInt(args, 1);
        
        if (min > max)
            throw new Exception("min must be less than or equal to max");
            
        return Value.Number(_random.Next(min, max + 1));
    }
    
    private static Value RandFloat(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "randfloat");
        var min = ValidationHelpers.GetNumber(args, 0);
        var max = ValidationHelpers.GetNumber(args, 1);
        
        if (min > max)
            throw new Exception("min must be less than or equal to max");
            
        return Value.Number(min + (_random.NextDouble() * (max - min)));
    }
    
    private static Value Choice(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "choice");
        var array = ValidationHelpers.GetArray(args, 0);
        
        if (array.Count == 0)
            throw new Exception("Cannot choose from empty array");
            
        var index = _random.Next(array.Count);
        return array[index];
    }
    
    private static Value Shuffle(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "shuffle");
        var array = ValidationHelpers.GetArray(args, 0);
        
        var result = new List<Value>(array);
        var n = result.Count;
        for (int i = 0; i < n - 1; i++)
        {
            var j = _random.Next(i, n);
            (result[i], result[j]) = (result[j], result[i]);
        }
        
        return Value.Array(result);
    }
    
    private static Value Sample(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "sample");
        var array = ValidationHelpers.GetArray(args, 0);
        var k = ValidationHelpers.GetInt(args, 1);
        
        if (k < 0 || k > array.Count)
            throw new Exception("Sample size must be between 0 and array length");
            
        if (k == 0)
            return Value.Array(new List<Value>());
            
        var shuffled = new List<Value>(array);
        var n = shuffled.Count;
        for (int i = 0; i < n - 1; i++)
        {
            var j = _random.Next(i, n);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        
        return Value.Array(shuffled.GetRange(0, k));
    }
    
    private static Value Seed(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "seed");
        var seed = ValidationHelpers.GetInt(args, 0);
        _random = new System.Random(seed);
        return Value.Null();
    }
    
    private static Value WeightedChoice(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "weighted_choice");
        var obj = ValidationHelpers.GetObject(args, 0);
        
        if (obj.Count == 0)
            throw new Exception("Cannot choose from empty object");
            
        var totalWeight = 0.0;
        foreach (var kvp in obj)
        {
            var weight = ValidationHelpers.GetNumber(new List<Value> { kvp.Value }, 0);
            if (weight < 0)
                throw new Exception("Weights must be non-negative");
            totalWeight += weight;
        }
        
        if (totalWeight <= 0)
            throw new Exception("Total weight must be positive");
            
        var randomValue = _random.NextDouble() * totalWeight;
        var currentWeight = 0.0;
        
        foreach (var kvp in obj)
        {
            var weight = ValidationHelpers.GetNumber(new List<Value> { kvp.Value }, 0);
            currentWeight += weight;
            if (randomValue <= currentWeight)
                return Value.String(kvp.Key);
        }
        
        // Fallback to last item (shouldn't happen)
        return Value.String(obj.Keys.Last());
    }
    
    private static Value Normal(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "normal");
        var mean = ValidationHelpers.GetNumber(args, 0);
        var stddev = ValidationHelpers.GetNumber(args, 1);
        var count = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : 1;
        
        if (stddev < 0)
            throw new Exception("Standard deviation must be non-negative");
            
        var result = new List<Value>();
        for (int i = 0; i < count; i++)
        {
            // Box-Muller transform
            var u1 = _random.NextDouble();
            var u2 = _random.NextDouble();
            var z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            var z1 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            
            result.Add(Value.Number(mean + stddev * z0));
        }
        
        return count == 1 ? result[0] : Value.Array(result);
    }
    
    private static Value Uniform(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "uniform");
        var min = ValidationHelpers.GetNumber(args, 0);
        var max = ValidationHelpers.GetNumber(args, 1);
        var count = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : 1;
        
        if (min > max)
            throw new Exception("min must be less than or equal to max");
            
        var result = new List<Value>();
        for (int i = 0; i < count; i++)
        {
            result.Add(Value.Number(min + (_random.NextDouble() * (max - min))));
        }
        
        return count == 1 ? result[0] : Value.Array(result);
    }
}
using Snip.AST;
using System.Text.RegularExpressions;

namespace Snip.Evaluator;

public static class RegexModule
{
    public static Module CreateModule()
    {
        var module = new Module("regex");
        
        // Pattern matching
        module.Export("match", Value.NativeFunction(new NativeFunctionValue("match", Match)));
        module.Export("matches", Value.NativeFunction(new NativeFunctionValue("matches", Matches)));
        module.Export("search", Value.NativeFunction(new NativeFunctionValue("search", Search)));
        module.Export("findall", Value.NativeFunction(new NativeFunctionValue("findall", FindAll)));
        module.Export("finditer", Value.NativeFunction(new NativeFunctionValue("finditer", FindIter)));
        
        // String operations
        module.Export("split", Value.NativeFunction(new NativeFunctionValue("split", Split)));
        module.Export("sub", Value.NativeFunction(new NativeFunctionValue("sub", Sub)));
        module.Export("subn", Value.NativeFunction(new NativeFunctionValue("subn", Subn)));
        
        // Pattern compilation
        module.Export("compile", Value.NativeFunction(new NativeFunctionValue("compile", Compile)));
        module.Export("escape", Value.NativeFunction(new NativeFunctionValue("escape", Escape)));
        
        // Pattern testing
        module.Export("fullmatch", Value.NativeFunction(new NativeFunctionValue("fullmatch", FullMatch)));
        module.Export("test", Value.NativeFunction(new NativeFunctionValue("test", Test)));
        
        return module;
    }
    
    private static Value Match(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "match");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var flags = args.Count > 2 ? GetRegexOptions(args, 2) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var match = regex.Match(text);
            
            if (match.Success)
            {
                return MatchToObject(match);
            }
            else
            {
                return Value.Null();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex match failed: {ex.Message}");
        }
    }
    
    private static Value Matches(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "matches");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var flags = args.Count > 2 ? GetRegexOptions(args, 2) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var matches = regex.Matches(text);
            var result = new List<Value>();
            
            foreach (Match match in matches)
            {
                result.Add(MatchToObject(match));
            }
            
            return Value.Array(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex matches failed: {ex.Message}");
        }
    }
    
    private static Value Search(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "search");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var flags = args.Count > 2 ? GetRegexOptions(args, 2) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var match = regex.Match(text);
            
            if (match.Success)
            {
                return MatchToObject(match);
            }
            else
            {
                return Value.Null();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex search failed: {ex.Message}");
        }
    }
    
    private static Value FindAll(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "findall");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var flags = args.Count > 2 ? GetRegexOptions(args, 2) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var matches = regex.Matches(text);
            var result = new List<Value>();
            
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    // If there are capture groups, return the captured groups
                    var groups = new List<Value>();
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        groups.Add(Value.String(match.Groups[i].Value));
                    }
                    result.Add(groups.Count == 1 ? groups[0] : Value.Array(groups));
                }
                else
                {
                    // Otherwise return the full match
                    result.Add(Value.String(match.Value));
                }
            }
            
            return Value.Array(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex findall failed: {ex.Message}");
        }
    }
    
    private static Value FindIter(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "finditer");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var flags = args.Count > 2 ? GetRegexOptions(args, 2) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var matches = regex.Matches(text);
            var result = new List<Value>();
            
            foreach (Match match in matches)
            {
                result.Add(MatchToObject(match));
            }
            
            return Value.Array(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex finditer failed: {ex.Message}");
        }
    }
    
    private static Value Split(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 4, "split");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var maxSplit = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : 0;
        var flags = args.Count > 3 ? GetRegexOptions(args, 3) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var parts = regex.Split(text);
            var result = new List<Value>();
            
            int limit = maxSplit > 0 ? Math.Min(maxSplit + 1, parts.Length) : parts.Length;
            for (int i = 0; i < limit; i++)
            {
                result.Add(Value.String(parts[i]));
            }
            
            return Value.Array(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex split failed: {ex.Message}");
        }
    }
    
    private static Value Sub(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 5, "sub");
        var pattern = ValidationHelpers.GetString(args, 0);
        var replacement = ValidationHelpers.GetString(args, 1);
        var text = ValidationHelpers.GetString(args, 2);
        var count = args.Count > 3 ? ValidationHelpers.GetInt(args, 3) : 0;
        var flags = args.Count > 4 ? GetRegexOptions(args, 4) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var result = count > 0 ? regex.Replace(text, replacement, count) : regex.Replace(text, replacement);
            return Value.String(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex sub failed: {ex.Message}");
        }
    }
    
    private static Value Subn(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 5, "subn");
        var pattern = ValidationHelpers.GetString(args, 0);
        var replacement = ValidationHelpers.GetString(args, 1);
        var text = ValidationHelpers.GetString(args, 2);
        var count = args.Count > 3 ? ValidationHelpers.GetInt(args, 3) : 0;
        var flags = args.Count > 4 ? GetRegexOptions(args, 4) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            var replacementCount = 0;
            
            string result = count > 0 
                ? regex.Replace(text, m => { replacementCount++; return replacement; }, count)
                : regex.Replace(text, m => { replacementCount++; return replacement; });
            
            return Value.Array(new List<Value>
            {
                Value.String(result),
                Value.Number(replacementCount)
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex subn failed: {ex.Message}");
        }
    }
    
    private static Value Compile(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "compile");
        var pattern = ValidationHelpers.GetString(args, 0);
        var flags = args.Count > 1 ? GetRegexOptions(args, 1) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            return Value.Object(new Dictionary<string, Value>
            {
                ["pattern"] = Value.String(regex.ToString()),
                ["options"] = Value.Number((int)regex.Options),
                ["match"] = Value.NativeFunction(new NativeFunctionValue("match", (matchArgs) => {
                    ValidationHelpers.ValidateArgs(matchArgs, 1, 1, "compiled_match");
                    var text = ValidationHelpers.GetString(matchArgs, 0);
                    var match = regex.Match(text);
                    return match.Success ? MatchToObject(match) : Value.Null();
                })),
                ["findall"] = Value.NativeFunction(new NativeFunctionValue("findall", (findArgs) => {
                    ValidationHelpers.ValidateArgs(findArgs, 1, 1, "compiled_findall");
                    var text = ValidationHelpers.GetString(findArgs, 0);
                    var matches = regex.Matches(text);
                    var result = new List<Value>();
                    foreach (Match match in matches)
                    {
                        result.Add(Value.String(match.Value));
                    }
                    return Value.Array(result);
                }))
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex compile failed: {ex.Message}");
        }
    }
    
    private static Value Escape(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "escape");
        var text = ValidationHelpers.GetString(args, 0);
        return Value.String(Regex.Escape(text));
    }
    
    private static Value FullMatch(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "fullmatch");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var flags = args.Count > 2 ? GetRegexOptions(args, 2) : RegexOptions.None;
        
        try
        {
            var regex = new Regex($"^{pattern}$", flags);
            var match = regex.Match(text);
            
            if (match.Success)
            {
                return MatchToObject(match);
            }
            else
            {
                return Value.Null();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex fullmatch failed: {ex.Message}");
        }
    }
    
    private static Value Test(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "test");
        var pattern = ValidationHelpers.GetString(args, 0);
        var text = ValidationHelpers.GetString(args, 1);
        var flags = args.Count > 2 ? GetRegexOptions(args, 2) : RegexOptions.None;
        
        try
        {
            var regex = new Regex(pattern, flags);
            return Value.Boolean(regex.IsMatch(text));
        }
        catch (Exception ex)
        {
            throw new Exception($"Regex test failed: {ex.Message}");
        }
    }
    
    private static Value MatchToObject(Match match)
    {
        var groups = new Dictionary<string, Value>();
        
        for (int i = 0; i < match.Groups.Count; i++)
        {
            var group = match.Groups[i];
            groups[i.ToString()] = Value.Object(new Dictionary<string, Value>
            {
                ["value"] = Value.String(group.Value),
                ["start"] = Value.Number(group.Index),
                ["end"] = Value.Number(group.Index + group.Length),
                ["length"] = Value.Number(group.Length)
            });
        }
        
        // Add named groups
        foreach (string groupName in match.Groups.Keys)
        {
            if (groupName != "0") // Skip the default group
            {
                var group = match.Groups[groupName];
                groups[groupName] = Value.Object(new Dictionary<string, Value>
                {
                    ["value"] = Value.String(group.Value),
                    ["start"] = Value.Number(group.Index),
                    ["end"] = Value.Number(group.Index + group.Length),
                    ["length"] = Value.Number(group.Length)
                });
            }
        }
        
        return Value.Object(new Dictionary<string, Value>
        {
            ["value"] = Value.String(match.Value),
            ["start"] = Value.Number(match.Index),
            ["end"] = Value.Number(match.Index + match.Length),
            ["length"] = Value.Number(match.Length),
            ["groups"] = Value.Object(groups),
            ["success"] = Value.Boolean(match.Success)
        });
    }
    
    private static RegexOptions GetRegexOptions(List<Value> args, int index)
    {
        var options = RegexOptions.None;
        var flagsValue = args[index];
        
        if (flagsValue.Type == ValueType.String)
        {
            var flags = ValidationHelpers.GetString(args, index).ToLower();
            if (flags.Contains("i")) options |= RegexOptions.IgnoreCase;
            if (flags.Contains("m")) options |= RegexOptions.Multiline;
            if (flags.Contains("s")) options |= RegexOptions.Singleline;
            if (flags.Contains("x")) options |= RegexOptions.IgnorePatternWhitespace;
        }
        else if (flagsValue.Type == ValueType.Array)
        {
            var flagsArray = ValidationHelpers.GetArray(args, index);
            foreach (var flag in flagsArray)
            {
                var flagStr = ValidationHelpers.GetString(new List<Value> { flag }, 0).ToLower();
                switch (flagStr)
                {
                    case "ignorecase": options |= RegexOptions.IgnoreCase; break;
                    case "multiline": options |= RegexOptions.Multiline; break;
                    case "singleline": options |= RegexOptions.Singleline; break;
                    case "ignorepatternwhitespace": options |= RegexOptions.IgnorePatternWhitespace; break;
                }
            }
        }
        
        return options;
    }
}
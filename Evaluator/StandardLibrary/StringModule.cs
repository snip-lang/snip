using Snip.AST;

namespace Snip.Evaluator;

public static class StringModule
{
    public static Module CreateModule()
    {
        var module = new Module("string");
        
        // Case conversion
        module.Export("upper", Value.NativeFunction(new NativeFunctionValue("upper", Upper)));
        module.Export("lower", Value.NativeFunction(new NativeFunctionValue("lower", Lower)));
        module.Export("capitalize", Value.NativeFunction(new NativeFunctionValue("capitalize", Capitalize)));
        module.Export("title", Value.NativeFunction(new NativeFunctionValue("title", Title)));
        
        // Trimming and padding
        module.Export("strip", Value.NativeFunction(new NativeFunctionValue("strip", Strip)));
        module.Export("lstrip", Value.NativeFunction(new NativeFunctionValue("lstrip", LStrip)));
        module.Export("rstrip", Value.NativeFunction(new NativeFunctionValue("rstrip", RStrip)));
        module.Export("ljust", Value.NativeFunction(new NativeFunctionValue("ljust", LJust)));
        module.Export("rjust", Value.NativeFunction(new NativeFunctionValue("rjust", RJust)));
        module.Export("center", Value.NativeFunction(new NativeFunctionValue("center", Center)));
        
        // Searching and replacing
        module.Export("find", Value.NativeFunction(new NativeFunctionValue("find", Find)));
        module.Export("rfind", Value.NativeFunction(new NativeFunctionValue("rfind", RFind)));
        module.Export("replace", Value.NativeFunction(new NativeFunctionValue("replace", Replace)));
        module.Export("count", Value.NativeFunction(new NativeFunctionValue("count", Count)));
        
        // Splitting and joining
        module.Export("split", Value.NativeFunction(new NativeFunctionValue("split", Split)));
        module.Export("rsplit", Value.NativeFunction(new NativeFunctionValue("rsplit", RSplit)));
        module.Export("join", Value.NativeFunction(new NativeFunctionValue("join", Join)));
        
        // Testing
        module.Export("startswith", Value.NativeFunction(new NativeFunctionValue("startswith", StartsWith)));
        module.Export("endswith", Value.NativeFunction(new NativeFunctionValue("endswith", EndsWith)));
        module.Export("isalpha", Value.NativeFunction(new NativeFunctionValue("isalpha", IsAlpha)));
        module.Export("isdigit", Value.NativeFunction(new NativeFunctionValue("isdigit", IsDigit)));
        module.Export("isalnum", Value.NativeFunction(new NativeFunctionValue("isalnum", IsAlnum)));
        module.Export("isspace", Value.NativeFunction(new NativeFunctionValue("isspace", IsSpace)));
        
        // Haskell Prelude functions
        module.Export("words", Value.NativeFunction(new NativeFunctionValue("words", Words)));
        module.Export("lines", Value.NativeFunction(new NativeFunctionValue("lines", Lines)));
        module.Export("unlines", Value.NativeFunction(new NativeFunctionValue("unlines", UnLines)));
        module.Export("unwords", Value.NativeFunction(new NativeFunctionValue("unwords", UnWords)));
        module.Export("reverse", Value.NativeFunction(new NativeFunctionValue("reverse", Reverse)));
        module.Export("take", Value.NativeFunction(new NativeFunctionValue("take", Take)));
        module.Export("drop", Value.NativeFunction(new NativeFunctionValue("drop", Drop)));
        
        return module;
    }
    
    private static Value Upper(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "upper");
        var str = ValidationHelpers.GetString(args, 0);
        return Value.String(str.ToUpper());
    }
    
    private static Value Lower(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "lower");
        var str = ValidationHelpers.GetString(args, 0);
        return Value.String(str.ToLower());
    }
    
    private static Value Capitalize(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "capitalize");
        var str = ValidationHelpers.GetString(args, 0);
        if (string.IsNullOrEmpty(str))
            return Value.String(str);
        return Value.String(char.ToUpper(str[0]) + str.Substring(1).ToLower());
    }
    
    private static Value Title(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "title");
        var str = ValidationHelpers.GetString(args, 0);
        var words = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
        return Value.String(string.Join(" ", words));
    }
    
    private static Value Strip(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "strip");
        var str = ValidationHelpers.GetString(args, 0);
        var chars = args.Count > 1 ? ValidationHelpers.GetString(args, 1) : " \t\n\r";
        return Value.String(str.Trim(chars.ToCharArray()));
    }
    
    private static Value LStrip(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "lstrip");
        var str = ValidationHelpers.GetString(args, 0);
        var chars = args.Count > 1 ? ValidationHelpers.GetString(args, 1) : " \t\n\r";
        return Value.String(str.TrimStart(chars.ToCharArray()));
    }
    
    private static Value RStrip(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "rstrip");
        var str = ValidationHelpers.GetString(args, 0);
        var chars = args.Count > 1 ? ValidationHelpers.GetString(args, 1) : " \t\n\r";
        return Value.String(str.TrimEnd(chars.ToCharArray()));
    }
    
    private static Value LJust(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "ljust");
        var str = ValidationHelpers.GetString(args, 0);
        var width = ValidationHelpers.GetInt(args, 1);
        var fillchar = args.Count > 2 ? ValidationHelpers.GetString(args, 1) : " ";
        if (fillchar.Length != 1)
            throw new Exception("fillchar must be a single character");
        return Value.String(str.PadRight(width, fillchar[0]));
    }
    
    private static Value RJust(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "rjust");
        var str = ValidationHelpers.GetString(args, 0);
        var width = ValidationHelpers.GetInt(args, 1);
        var fillchar = args.Count > 2 ? ValidationHelpers.GetString(args, 1) : " ";
        if (fillchar.Length != 1)
            throw new Exception("fillchar must be a single character");
        return Value.String(str.PadLeft(width, fillchar[0]));
    }
    
    private static Value Center(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "center");
        var str = ValidationHelpers.GetString(args, 0);
        var width = ValidationHelpers.GetInt(args, 1);
        var fillchar = args.Count > 2 ? ValidationHelpers.GetString(args, 1) : " ";
        if (fillchar.Length != 1)
            throw new Exception("fillchar must be a single character");
        
        var totalPad = width - str.Length;
        if (totalPad <= 0)
            return Value.String(str);
        
        var leftPad = totalPad / 2;
        var rightPad = totalPad - leftPad;
        return Value.String(new string(fillchar[0], leftPad) + str + new string(fillchar[0], rightPad));
    }
    
    private static Value Find(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "find");
        var str = ValidationHelpers.GetString(args, 0);
        var sub = ValidationHelpers.GetString(args, 1);
        var start = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : 0;
        var index = str.IndexOf(sub, start);
        return Value.Number(index);
    }
    
    private static Value RFind(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 3, "rfind");
        var str = ValidationHelpers.GetString(args, 0);
        var sub = ValidationHelpers.GetString(args, 1);
        var start = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : str.Length;
        var index = str.LastIndexOf(sub, start - 1);
        return Value.Number(index);
    }
    
    private static Value Replace(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 3, 3, "replace");
        var str = ValidationHelpers.GetString(args, 0);
        var old = ValidationHelpers.GetString(args, 1);
        var @new = ValidationHelpers.GetString(args, 2);
        return Value.String(str.Replace(old, @new));
    }
    
    private static Value Count(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "count");
        var str = ValidationHelpers.GetString(args, 0);
        var sub = ValidationHelpers.GetString(args, 1);
        var count = 0;
        var index = 0;
        while ((index = str.IndexOf(sub, index)) != -1)
        {
            count++;
            index += sub.Length;
        }
        return Value.Number(count);
    }
    
    private static Value Split(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "split");
        var str = ValidationHelpers.GetString(args, 0);
        var sep = args.Count > 1 ? ValidationHelpers.GetString(args, 1) : " ";
        var parts = string.IsNullOrEmpty(sep) ? str.ToCharArray().Select(c => Value.String(c.ToString())).ToList() 
                     : str.Split(sep).Select(s => Value.String(s)).ToList();
        return Value.Array(parts);
    }
    
    private static Value RSplit(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 3, "rsplit");
        var str = ValidationHelpers.GetString(args, 0);
        var sep = args.Count > 1 ? ValidationHelpers.GetString(args, 1) : null;
        var maxsplit = args.Count > 2 ? ValidationHelpers.GetInt(args, 2) : -1;

        string[] parts;
        if (sep == null)
        {
            // Split on whitespace, removing empty entries
            parts = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            parts = str.Split(sep);
        }

        if (maxsplit < 0 || parts.Length <= maxsplit + 1)
        {
            return Value.Array(parts.Select(s => Value.String(s)).ToList());
        }

        // Need to limit splits from the right
        var result = new List<string>();
        var splitPoint = parts.Length - maxsplit - 1;
        var leftPart = string.Join(sep ?? " ", parts[..splitPoint]);
        result.Add(leftPart);
        result.AddRange(parts[splitPoint..]);

        return Value.Array(result.Select(s => Value.String(s)).ToList());
    }
    
    private static Value Join(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "join");
        var sep = ValidationHelpers.GetString(args, 0);
        var array = ValidationHelpers.GetArray(args, 1);
        var strings = array.Select(v => v.Type == ValueType.String ? (string)v.Data! : v.ToString()).ToList();
        return Value.String(string.Join(sep, strings));
    }
    
    private static Value StartsWith(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "startswith");
        var str = ValidationHelpers.GetString(args, 0);
        var prefix = ValidationHelpers.GetString(args, 1);
        return Value.Boolean(str.StartsWith(prefix));
    }
    
    private static Value EndsWith(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "endswith");
        var str = ValidationHelpers.GetString(args, 0);
        var suffix = ValidationHelpers.GetString(args, 1);
        return Value.Boolean(str.EndsWith(suffix));
    }
    
    private static Value IsAlpha(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isalpha");
        var str = ValidationHelpers.GetString(args, 0);
        return Value.Boolean(str.All(char.IsLetter));
    }
    
    private static Value IsDigit(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isdigit");
        var str = ValidationHelpers.GetString(args, 0);
        return Value.Boolean(str.All(char.IsDigit));
    }
    
    private static Value IsAlnum(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isalnum");
        var str = ValidationHelpers.GetString(args, 0);
        return Value.Boolean(str.All(char.IsLetterOrDigit));
    }
    
    private static Value IsSpace(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isspace");
        var str = ValidationHelpers.GetString(args, 0);
        return Value.Boolean(str.All(char.IsWhiteSpace));
    }
    
    // Haskell Prelude functions
    private static Value Words(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "words");
        var str = ValidationHelpers.GetString(args, 0);
        var words = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return Value.Array(words.Select(w => Value.String(w)).ToList());
    }
    
    private static Value Lines(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "lines");
        var str = ValidationHelpers.GetString(args, 0);
        var lines = str.Split('\n');
        return Value.Array(lines.Select(l => Value.String(l)).ToList());
    }
    
    private static Value UnLines(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "unlines");
        var array = ValidationHelpers.GetArray(args, 0);
        var strings = array.Select(v => v.Type == ValueType.String ? (string)v.Data! : v.ToString()).ToList();
        return Value.String(string.Join("\n", strings));
    }
    
    private static Value UnWords(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "unwords");
        var array = ValidationHelpers.GetArray(args, 0);
        var strings = array.Select(v => v.Type == ValueType.String ? (string)v.Data! : v.ToString()).ToList();
        return Value.String(string.Join(" ", strings));
    }
    
    private static Value Reverse(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "reverse");
        var str = ValidationHelpers.GetString(args, 0);
        var chars = str.ToCharArray();
        Array.Reverse(chars);
        return Value.String(new string(chars));
    }
    
    private static Value Take(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "take");
        var n = ValidationHelpers.GetInt(args, 0);
        var str = ValidationHelpers.GetString(args, 1);
        
        n = n < 0 ? str.Length + n : n;
        n = Math.Min(n, str.Length);
        
        return Value.String(str.Substring(0, n));
    }
    
    private static Value Drop(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "drop");
        var n = ValidationHelpers.GetInt(args, 0);
        var str = ValidationHelpers.GetString(args, 1);
        
        n = n < 0 ? str.Length + n : n;
        n = Math.Min(n, str.Length);
        
        return Value.String(str.Substring(n));
    }
}
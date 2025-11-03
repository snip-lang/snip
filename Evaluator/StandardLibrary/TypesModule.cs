using Snip.AST;
using System.Text.Json;

namespace Snip.Evaluator;

public static class TypesModule
{
    public static Module CreateModule()
    {
        var module = new Module("types");
        
        // Type checking
        module.Export("type", Value.NativeFunction(new NativeFunctionValue("type", TypeOf)));
        module.Export("isinstance", Value.NativeFunction(new NativeFunctionValue("isinstance", IsInstance)));
        module.Export("isnumber", Value.NativeFunction(new NativeFunctionValue("isnumber", IsNumber)));
        module.Export("isstring", Value.NativeFunction(new NativeFunctionValue("isstring", IsString)));
        module.Export("isboolean", Value.NativeFunction(new NativeFunctionValue("isboolean", IsBoolean)));
        module.Export("isnull", Value.NativeFunction(new NativeFunctionValue("isnull", IsNull)));
        module.Export("isundefined", Value.NativeFunction(new NativeFunctionValue("isundefined", IsUndefined)));
        module.Export("isarray", Value.NativeFunction(new NativeFunctionValue("isarray", IsArray)));
        module.Export("isobject", Value.NativeFunction(new NativeFunctionValue("isobject", IsObject)));
        module.Export("isfunction", Value.NativeFunction(new NativeFunctionValue("isfunction", IsFunction)));

        // Conversion
        module.Export("str", Value.NativeFunction(new NativeFunctionValue("str", ToString)));
        module.Export("int", Value.NativeFunction(new NativeFunctionValue("int", ToNumber)));
        module.Export("bool", Value.NativeFunction(new NativeFunctionValue("bool", ToBoolean)));
        
        // JSON
        module.Export("json_parse", Value.NativeFunction(new NativeFunctionValue("json_parse", JsonParse)));
        module.Export("json_stringify", Value.NativeFunction(new NativeFunctionValue("json_stringify", JsonStringify)));
        
        return module;
    }
    
    private static Value TypeOf(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "type");
        var value = args[0];
        return Value.String(value.Type.ToString().ToLower());
    }

    private static Value IsInstance(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 2, 2, "isinstance");
        var value = args[0];
        var typeName = ValidationHelpers.GetString(args, 1);
        var actualType = value.Type.ToString().ToLower();
        return Value.Boolean(actualType == typeName);
    }
    
    private static Value IsNumber(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isnumber");
        return Value.Boolean(args[0].Type == ValueType.Number);
    }
    
    private static Value IsString(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isstring");
        return Value.Boolean(args[0].Type == ValueType.String);
    }
    
    private static Value IsBoolean(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isboolean");
        return Value.Boolean(args[0].Type == ValueType.Boolean);
    }
    
    private static Value IsNull(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isnull");
        return Value.Boolean(args[0].Type == ValueType.Null);
    }
    
    private static Value IsUndefined(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isundefined");
        return Value.Boolean(args[0].Type == ValueType.Undefined);
    }
    
    private static Value IsArray(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isarray");
        return Value.Boolean(args[0].Type == ValueType.Array);
    }
    
    private static Value IsObject(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isobject");
        return Value.Boolean(args[0].Type == ValueType.Object);
    }
    
    private static Value IsFunction(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "isfunction");
        var value = args[0];
        return Value.Boolean(value.Type == ValueType.Function || value.Type == ValueType.NativeFunction);
    }
    
    private static Value ToString(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "tostring");
        return Value.String(args[0].ToString());
    }
    
    private static Value ToNumber(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "tonumber");
        var value = args[0];
        
        if (value.Type == ValueType.Number)
            return value;
        else if (value.Type == ValueType.String)
        {
            if (double.TryParse((string)value.Data!, out var num))
                return Value.Number(num);
            else
                return Value.Number(0);
        }
        else if (value.Type == ValueType.Boolean)
            return Value.Number((bool)value.Data! ? 1 : 0);
        else if (value.Type == ValueType.Null)
            return Value.Number(0);
        else
            return Value.Number(double.NaN);
    }
    
    private static Value ToBoolean(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "toboolean");
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
    
    private static Value JsonParse(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 1, "json_parse");
        var json = ValidationHelpers.GetString(args, 0);
        
        try
        {
            var element = JsonSerializer.Deserialize<JsonElement>(json);
            return ConvertJsonElementToValue(element);
        }
        catch (JsonException)
        {
            throw new Exception("Invalid JSON string");
        }
    }
    
    private static Value JsonStringify(List<Value> args)
    {
        ValidationHelpers.ValidateArgs(args, 1, 2, "json_stringify");
        var value = args[0];
        var indent = args.Count > 1 ? ValidationHelpers.GetInt(args, 1) : 0;
        
        try
        {
            var jsonValue = ConvertValueToJsonElement(value);
            var options = new JsonSerializerOptions { WriteIndented = indent > 0 };
            return Value.String(JsonSerializer.Serialize(jsonValue, options));
        }
        catch
        {
            throw new Exception("Value cannot be converted to JSON");
        }
    }
    
    private static Value ConvertJsonElementToValue(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return Value.String(element.GetString()!);
            case JsonValueKind.Number:
                return Value.Number(element.GetDouble());
            case JsonValueKind.True:
                return Value.Boolean(true);
            case JsonValueKind.False:
                return Value.Boolean(false);
            case JsonValueKind.Null:
                return Value.Null();
            case JsonValueKind.Array:
                var array = new List<Value>();
                foreach (var item in element.EnumerateArray())
                    array.Add(ConvertJsonElementToValue(item));
                return Value.Array(array);
            case JsonValueKind.Object:
                var obj = new Dictionary<string, Value>();
                foreach (var property in element.EnumerateObject())
                    obj[property.Name] = ConvertJsonElementToValue(property.Value);
                return Value.Object(obj);
            default:
                return Value.Null();
        }
    }
    
    private static JsonElement ConvertValueToJsonElement(Value value)
    {
        string json;
        
        switch (value.Type)
        {
            case ValueType.String:
                json = $"\"{value.Data}\"";
                break;
            case ValueType.Number:
                json = value.Data?.ToString() ?? "0";
                break;
            case ValueType.Boolean:
                json = value.Data?.ToString().ToLower() ?? "false";
                break;
            case ValueType.Null:
            case ValueType.Undefined:
                json = "null";
                break;
            case ValueType.Array:
                var array = (List<Value>)value.Data!;
                var items = new List<string>();
                foreach (var item in array)
                    items.Add(ConvertValueToJsonElement(item).GetRawText());
                json = $"[{string.Join(",", items)}]";
                break;
            case ValueType.Object:
                var obj = (Dictionary<string, Value>)value.Data!;
                var properties = new List<string>();
                foreach (var kvp in obj)
                    properties.Add($"\"{kvp.Key}\":{ConvertValueToJsonElement(kvp.Value).GetRawText()}");
                json = $"{{{string.Join(",", properties)}}}";
                break;
            default:
                json = "null";
                break;
        }
        
        return JsonSerializer.Deserialize<JsonElement>(json)!;
    }
}
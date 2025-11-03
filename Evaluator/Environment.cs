namespace Snip.Evaluator;

public class Environment
{
    private readonly Dictionary<string, Value> _bindings = new();
    private readonly Environment? _parent;
    private readonly Dictionary<string, Module> _modules = new();
    private readonly HashSet<string> _importedModules = new();

    public Environment(Environment? parent = null)
    {
        _parent = parent;
    }

    public void Define(string name, Value value)
    {
        _bindings[name] = value;
    }

    public Value Get(string name)
    {
        if (_bindings.TryGetValue(name, out var value))
        {
            return value;
        }

        if (_parent != null)
        {
            return _parent.Get(name);
        }

        throw new Exception($"ReferenceError: '{name}' is not defined");
    }

    public void Assign(string name, Value value)
    {
        if (_bindings.ContainsKey(name))
        {
            _bindings[name] = value;
            return;
        }

        if (_parent != null)
        {
            _parent.Assign(name, value);
            return;
        }

        // Allow assignment to create variables (like JavaScript)
        _bindings[name] = value;
    }

    public Environment CreateChild()
    {
        return new Environment(this);
    }

    public void InitializeBuiltins()
    {
        // Register standard library modules
        StandardLibraryRegistry.RegisterAllModules(this);

        // print function (overridden by std lib)
        Define("print", Value.NativeFunction(new NativeFunctionValue("print", args =>
        {
            foreach (var arg in args)
            {
                Console.Write(arg.ToString());
            }
            Console.WriteLine();
            return Value.Undefined();
        })));

        // typeof function
        Define("typeof", Value.NativeFunction(new NativeFunctionValue("typeof", args =>
        {
            if (args.Count == 0) return Value.String("undefined");
            var arg = args[0];
            var typeStr = arg.Type switch
            {
                ValueType.Number => "number",
                ValueType.String => "string",
                ValueType.Boolean => "boolean",
                ValueType.Null => "object",
                ValueType.Undefined => "undefined",
                ValueType.Array => "object",
                ValueType.Object => "object",
                ValueType.Function => "function",
                ValueType.NativeFunction => "function",
                ValueType.Class => "function",
                ValueType.Instance => "object",
                _ => "unknown"
            };
            return Value.String(typeStr);
        })));

        // parseInt function
        Define("parseInt", Value.NativeFunction(new NativeFunctionValue("parseInt", args =>
        {
            if (args.Count == 0) return Value.Number(0.0);
            var str = args[0].ToString().Trim('"');
            if (long.TryParse(str, out var result))
            {
                return Value.Number((double)result);
            }
            return Value.Number(0.0);
        })));

        // Math object with some functions
        var mathObj = new Dictionary<string, Value>();
        mathObj["abs"] = Value.NativeFunction(new NativeFunctionValue("Math.abs", args =>
        {
            if (args.Count == 0) return Value.Number(0.0);
            var arg = args[0];
            if (arg.Type == ValueType.Number)
            {
                return Value.Number(Math.Abs((double)arg.Data!));
            }
            return Value.Number(0.0);
        }));

        mathObj["random"] = Value.NativeFunction(new NativeFunctionValue("Math.random", args =>
        {
            return Value.Number(new Random().NextDouble());
        }));

        Define("Math", Value.Object(mathObj));
    }

    public void ImportModule(string moduleName)
    {
        if (_importedModules.Contains(moduleName))
            return; // Already imported

        var module = StandardLibraryRegistry.GetModule(moduleName);
        _modules[moduleName] = module;
        _importedModules.Add(moduleName);

        // Create module object with exports
        var moduleObject = new Dictionary<string, Value>();
        foreach (var export in module.Exports)
        {
            moduleObject[export.Key] = export.Value;
        }
        Define(moduleName, Value.Object(moduleObject));

        // Also export all module functions to current environment
        foreach (var export in module.Exports)
        {
            Define(export.Key, export.Value);
        }
    }

    public void ImportFromModule(string moduleName, List<string> names)
    {
        Module module;
        
        // Check if module is already loaded
        if (_modules.TryGetValue(moduleName, out module))
        {
            // Use existing module
        }
        else
        {
            // Load new module
            module = StandardLibraryRegistry.GetModule(moduleName);
            _modules[moduleName] = module;
        }

        // Export only requested names
        foreach (var name in names)
        {
            var export = module.GetExport(name);
            if (export != null)
            {
                Define(name, export);
            }
            else
            {
                throw new Exception($"Module '{moduleName}' does not export '{name}'");
            }
        }
    }
}
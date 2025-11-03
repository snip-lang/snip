namespace Snip.Evaluator;

public class Module
{
    public string Name { get; }
    public Dictionary<string, Value> Exports { get; }
    
    public Module(string name)
    {
        Name = name;
        Exports = new Dictionary<string, Value>();
    }
    
    public void Export(string name, Value value)
    {
        Exports[name] = value;
    }
    
    public Value? GetExport(string name)
    {
        return Exports.TryGetValue(name, out var value) ? value : null;
    }
}
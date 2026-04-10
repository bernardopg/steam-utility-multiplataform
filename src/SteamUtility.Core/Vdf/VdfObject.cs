namespace SteamUtility.Core.Vdf;

public sealed class VdfObject
{
    private readonly Dictionary<string, List<string>> _values = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<VdfObject>> _children = new(StringComparer.OrdinalIgnoreCase);

    public VdfObject(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public IReadOnlyDictionary<string, List<string>> Values => _values;

    public IReadOnlyDictionary<string, List<VdfObject>> Children => _children;

    public void AddValue(string key, string value)
    {
        if (!_values.TryGetValue(key, out var values))
        {
            values = [];
            _values[key] = values;
        }

        values.Add(value);
    }

    public void AddChild(string key, VdfObject child)
    {
        if (!_children.TryGetValue(key, out var children))
        {
            children = [];
            _children[key] = children;
        }

        children.Add(child);
    }

    public string? GetSingleValue(string key)
        => _values.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;

    public IReadOnlyList<VdfObject> GetChildren(string key)
        => _children.TryGetValue(key, out var children) ? children : [];
}

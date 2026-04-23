// tests/GitVisualizer.Tests/Fakes/FakeLocalStorageService.cs
using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeLocalStorageService : ILocalStorageService
{
    private readonly Dictionary<string, string> _store = new();

    public double Length => _store.Count;

    public void Clear() => _store.Clear();

    public TValue? GetItem<TValue>(string key, JsonTypeInfo<TValue>? typeInfo = null)
    {
        if (!_store.TryGetValue(key, out var json)) return default;
        if (typeInfo is not null)
            return JsonSerializer.Deserialize(json, typeInfo);
        return (TValue?)JsonSerializer.Deserialize(json, typeof(TValue));
    }

    public string Key(double index) => _store.Keys.ElementAtOrDefault((int)index) ?? string.Empty;

    public void RemoveItem(string key) => _store.Remove(key);

    public void SetItem<TValue>(string key, TValue value, JsonTypeInfo<TValue>? typeInfo = null)
    {
        _store[key] = typeInfo is not null
            ? JsonSerializer.Serialize(value, typeInfo)
            : JsonSerializer.Serialize(value);
    }
}
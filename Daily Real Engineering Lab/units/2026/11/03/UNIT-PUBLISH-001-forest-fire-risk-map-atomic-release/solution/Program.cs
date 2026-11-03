using System;
using System.Collections.Generic;
using System.Linq;

record Tile(string Release, string Cell, int Risk);

sealed class TileStore
{
    private readonly Dictionary<string, Tile> _objects = new();
    public string CurrentRelease { get; private set; } = "r1";
    public void Put(string key, Tile tile) => _objects[key] = tile;
    public Tile Get(string key) => _objects[key];
    public void SetCurrent(string release) => CurrentRelease = release;
    public IReadOnlyList<Tile> ReadMap()
    {
        string release = CurrentRelease;
        return new[] { "north", "south" }.Select(cell => Get($"map/{release}/{cell}")).ToArray();
    }
}

static class Scenario
{
    public static TileStore Seed()
    {
        var store = new TileStore();
        store.Put("map/r1/north", new Tile("r1", "north", 10));
        store.Put("map/r1/south", new Tile("r1", "south", 20));
        return store;
    }
    public static void Publish(TileStore store)
    {
        store.Put("map/r2/north", new Tile("r2", "north", 90));
        Console.WriteLine("checkpoint=after-first-tile");
        Report(store);
        store.Put("map/r2/south", new Tile("r2", "south", 80));
        store.SetCurrent("r2");
    }
    public static void Report(TileStore store)
    {
        var map = store.ReadMap();
        Console.WriteLine($"visible={string.Join(",", map.Select(t => t.Cell + ":" + t.Release))}");
        Console.WriteLine($"consistent={map.All(t => t.Release == map[0].Release)}");
    }
}

var store = Scenario.Seed();
Scenario.Publish(store);
Scenario.Report(store);

using BridgeBriefingLab;

var cases = new (string Name, Func<bool> Run)[]
{
    ("NORMAL_REPEAT", NormalRepeat),
    ("COLLISION_FORWARD", CollisionForward),
    ("COLLISION_REVERSE", CollisionReverse),
    ("MISSING_ORIGIN", MissingOrigin)
};

int passed = 0;
foreach (var test in cases)
{
    try
    {
        bool success = test.Run();
        Console.WriteLine((success ? "PASS " : "FAIL ") + test.Name);
        if (success) passed++;
    }
    catch (Exception exception)
    {
        Console.WriteLine("FAIL " + test.Name + " (" + exception.GetType().Name + ": " + exception.Message + ")");
    }
}

Console.WriteLine("RESULT " + passed + "/" + cases.Length);
return passed == cases.Length ? 0 : 12;

static Dictionary<BriefingId, string> Seed() => new()
{
    [new("agency:west", "deck-9")] = "WEST:restricted-inspection",
    [new("agency", "west:deck-9")] = "CENTRAL:public-summary",
    [new("north", "bridge-1")] = "NORTH:normal"
};

static bool NormalRepeat()
{
    var cache = new BriefingCache(Seed());
    string first = cache.Get("north", "bridge-1");
    string second = cache.Get("north", "bridge-1");
    return first == "NORTH:normal" && second == first
        && cache.OriginReads == 1 && cache.CacheHits == 1 && cache.CacheEntries == 1;
}

static bool CollisionForward()
{
    var cache = new BriefingCache(Seed());
    string west = cache.Get("agency:west", "deck-9");
    string central = cache.Get("agency", "west:deck-9");
    return west == "WEST:restricted-inspection" && central == "CENTRAL:public-summary"
        && cache.OriginReads == 2 && cache.CacheEntries == 2;
}

static bool CollisionReverse()
{
    var cache = new BriefingCache(Seed());
    string central = cache.Get("agency", "west:deck-9");
    string west = cache.Get("agency:west", "deck-9");
    return central == "CENTRAL:public-summary" && west == "WEST:restricted-inspection"
        && cache.OriginReads == 2 && cache.CacheEntries == 2;
}

static bool MissingOrigin()
{
    var origin = new Dictionary<BriefingId, string>
    {
        [new("agency:west", "deck-9")] = "WEST:restricted-inspection"
    };
    var cache = new BriefingCache(origin);
    if (cache.Get("agency:west", "deck-9") != "WEST:restricted-inspection") return false;

    try
    {
        cache.Get("agency", "west:deck-9");
        return false;
    }
    catch (KeyNotFoundException)
    {
        return cache.OriginReads == 2 && cache.CacheEntries == 1;
    }
}

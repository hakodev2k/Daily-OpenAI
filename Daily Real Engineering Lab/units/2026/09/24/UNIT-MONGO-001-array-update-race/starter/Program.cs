var store = new TeamStore(new Team("team-7", new List<string> { "owner" }, 0));
var service = new TeamService(store);

var a = service.AddMemberAsync("alice");
var b = service.AddMemberAsync("bob");
await Task.WhenAll(a, b);

var final = await store.ReadAsync();
Console.WriteLine($"members={string.Join(',', final.Members.Order())}; version={final.Version}");

sealed record Team(string Id, List<string> Members, int Version);

sealed class TeamStore
{
    private Team _team;
    private readonly SemaphoreSlim _writeGate = new(1, 1);
    public TeamStore(Team team) => _team = Clone(team);

    public async Task<Team> ReadAsync()
    {
        await Task.Delay(30);
        return Clone(_team);
    }

    public async Task ReplaceAsync(Team replacement)
    {
        await Task.Delay(40);
        await _writeGate.WaitAsync();
        try { _team = Clone(replacement with { Version = _team.Version + 1 }); }
        finally { _writeGate.Release(); }
    }

    private static Team Clone(Team t) => new(t.Id, new List<string>(t.Members), t.Version);
}

sealed class TeamService
{
    private readonly TeamStore _store;
    public TeamService(TeamStore store) => _store = store;

    public async Task AddMemberAsync(string userId)
    {
        var team = await _store.ReadAsync();
        if (!team.Members.Contains(userId)) team.Members.Add(userId);
        await Task.Delay(60);
        await _store.ReplaceAsync(team);
        Console.WriteLine($"AddMember({userId}) succeeded");
    }
}
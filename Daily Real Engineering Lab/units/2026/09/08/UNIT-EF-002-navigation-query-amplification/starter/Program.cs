using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var sectionCount = 8;
var counter = new CommandCounterInterceptor();
var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<CmsDbContext>()
    .UseSqlite(connection)
    .AddInterceptors(counter)
    .Options;

using (var seedDb = new CmsDbContext(options))
{
    seedDb.Database.EnsureCreated();

    for (var i = 1; i <= sectionCount; i++)
    {
        seedDb.Sections.Add(new Section { Id = i, Name = $"Section {i}", SortOrder = i, IsActive = true });
        seedDb.Pages.Add(new Page { Id = i * 10 + 1, SectionId = i, Title = $"Featured {i}", Slug = $"featured-{i}", IsPublished = true, SortOrder = 1 });
        seedDb.Pages.Add(new Page { Id = i * 10 + 2, SectionId = i, Title = $"Secondary {i}", Slug = $"secondary-{i}", IsPublished = true, SortOrder = 2 });
    }

    seedDb.SaveChanges();
}

counter.Reset();

using (var db = new CmsDbContext(options))
{
    var sections = db.Sections
        .AsNoTracking()
        .Where(x => x.IsActive)
        .OrderBy(x => x.SortOrder)
        .ToList();

    var navigation = new List<NavigationItem>();

    foreach (var section in sections)
    {
        var featuredSlug = db.Pages
            .AsNoTracking()
            .Where(x => x.SectionId == section.Id && x.IsPublished)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.Slug)
            .FirstOrDefault();

        navigation.Add(new NavigationItem(section.Name, featuredSlug));
    }

    Console.WriteLine($"Items={navigation.Count}");
    Console.WriteLine($"MissingFeatured={navigation.Count(x => x.FeaturedSlug is null)}");
    Console.WriteLine($"Commands={counter.ReaderCommands}");
}

public sealed record NavigationItem(string SectionName, string? FeaturedSlug);

public sealed class Section
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class Page
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public bool IsPublished { get; set; }
    public int SortOrder { get; set; }
}

public sealed class CmsDbContext(DbContextOptions<CmsDbContext> options) : DbContext(options)
{
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Page> Pages => Set<Page>();
}

public sealed class CommandCounterInterceptor : DbCommandInterceptor
{
    public int ReaderCommands { get; private set; }

    public void Reset() => ReaderCommands = 0;

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        ReaderCommands++;
        return result;
    }
}

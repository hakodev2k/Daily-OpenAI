// Reference change inside starter/Program.cs:
// keep the query read-only but enable identity resolution for repeated entity keys.

var orders = await db.Orders
    .Include(x => x.Customer)
    .AsNoTrackingWithIdentityResolution()
    .OrderBy(x => x.Id)
    .ToListAsync();

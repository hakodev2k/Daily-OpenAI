using Microsoft.EntityFrameworkCore;

internal sealed class InspectionImportService(InspectionDbContext db)
{
    public void Import(IReadOnlyList<Inspection> batch, bool injectFailure = false)
    {
        var original = db.ChangeTracker.AutoDetectChangesEnabled;
        try
        {
            db.ChangeTracker.AutoDetectChangesEnabled = false;

            foreach (var incoming in batch)
            {
                db.Inspections.Add(new Inspection
                {
                    Id = incoming.Id,
                    SiteCode = incoming.SiteCode,
                    ReviewStatus = incoming.ReviewStatus
                });

                if (injectFailure)
                    throw new InvalidOperationException("Simulated import failure");
            }

            db.SaveChanges();
        }
        finally
        {
            db.ChangeTracker.AutoDetectChangesEnabled = original;
        }
    }
}

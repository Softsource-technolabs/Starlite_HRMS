using Microsoft.EntityFrameworkCore;

namespace StarLine.Infrastructure.Models
{
    public partial class StarLiteContext
    {
        public override int SaveChanges()
        {
            AddAuditInfo();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddAuditInfo();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditInfo()
        {
            var entries = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var properties = entry.CurrentValues.Properties.Select(p => p.Name).ToHashSet();

                var now = DateTime.UtcNow;

                // Handle CreatedDate
                if (entry.State == EntityState.Added && properties.Contains("CreatedDate"))
                {
                    entry.CurrentValues["CreatedDate"] = now;
                }
                // Handle UpdatedDate
                if (entry.State == EntityState.Modified && properties.Contains("UpdatedDate"))
                    entry.CurrentValues["UpdatedDate"] = now;

                // Handle IsDeleted & DeletedDate
                if (properties.Contains("IsDeleted") && Convert.ToBoolean(entry.CurrentValues["IsDeleted"]) && properties.Contains("DeletedDate"))
                    entry.CurrentValues["DeletedDate"] = now;
            }
        }
    }
}

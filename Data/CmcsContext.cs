using CMCS.Mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Mvc.Data
{
    public class CmcsContext : DbContext
    {
        public CmcsContext(DbContextOptions<CmcsContext> options) : base(options) { }

        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<Lecturer> Lecturers => Set<Lecturer>();
        public DbSet<SupportingDocument> SupportingDocuments => Set<SupportingDocument>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Lecturer>().HasData(
              new Lecturer { LecturerId = 1, FullName = "A. Smith", Email = "asmith@college.edu" },
              new Lecturer { LecturerId = 2, FullName = "B. Naidoo", Email = "bnaidoo@college.edu" },
              new Lecturer { LecturerId = 3, FullName = "C. Dlamini", Email = "cdlamini@college.edu" }
         );

            // Lecturer → Claim (1 to many)
            b.Entity<Claim>()
                .HasOne<Lecturer>()
                .WithMany()
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Claim → SupportingDocument (1 to many)
            b.Entity<SupportingDocument>()
                .HasOne<Claim>()
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId);

            base.OnModelCreating(b);
        }
    }
}

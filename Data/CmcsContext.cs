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
            // Seed Lecturers
            b.Entity<Lecturer>().HasData(
                new Lecturer { LecturerId = 1, FullName = "A. Smith", Email = "asmith@college.edu", HourlyRate = 550 },
                new Lecturer { LecturerId = 2, FullName = "B. Naidoo", Email = "bnaidoo@college.edu", HourlyRate = 480 },
                new Lecturer { LecturerId = 3, FullName = "C. Dlamini", Email = "cdlamini@college.edu", HourlyRate = 600 }
            );
            // Seed Admin Users (Password = "Pass123")
            var pass = "Pass123";
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pass));
            var hashedPass = System.Convert.ToHexString(hashBytes);

            b.Entity<AdminUser>().HasData(
                new AdminUser
                {
                    AdminId = 1,
                    FullName = "HR User",
                    Email = "hr@college.edu",
                    PasswordHash = hashedPass,
                    Role = "HR"
                },
                new AdminUser
                {
                    AdminId = 2,
                    FullName = "Coordinator User",
                    Email = "coord@college.edu",
                    PasswordHash = hashedPass,
                    Role = "Coordinator"
                },
                new AdminUser
                {
                    AdminId = 3,
                    FullName = "Manager User",
                    Email = "manager@college.edu",
                    PasswordHash = hashedPass,
                    Role = "Manager"
                }
            );


            // Lecturer → Claim (1-many)
            b.Entity<Claim>()
                .HasOne(c => c.Lecturer)
                .WithMany()
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Claim → Supporting Documents (1-many)
            b.Entity<SupportingDocument>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId);

            base.OnModelCreating(b);
        }
    }
}

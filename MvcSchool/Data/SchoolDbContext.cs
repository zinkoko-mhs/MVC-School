using Microsoft.EntityFrameworkCore;
using MvcSchool.Models.Domain;

namespace MvcSchool.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Enrollment> Enrollmentss { get; set; }
    }
}

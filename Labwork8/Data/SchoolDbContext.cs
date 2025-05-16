using Microsoft.EntityFrameworkCore;
using Labwork5.Models;
namespace Labwork5.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
        {
        }
        public DbSet<Class> Classes { get; set; }
    }
}
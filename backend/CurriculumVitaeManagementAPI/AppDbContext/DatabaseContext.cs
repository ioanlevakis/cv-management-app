using CurriculumVitaeManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CurriculumVitaeManagementAPI.AppDbContext
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Degree> Degrees { get; set; }
    }
}

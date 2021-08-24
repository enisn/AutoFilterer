using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using WebApplication.API.Models.LiveDemo;

namespace WebApplication.API.Contexts
{
    public class LiveDemoDbContext : DbContext
    {
        public LiveDemoDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected LiveDemoDbContext()
        {
        }

        public DbSet<Visit> Visits { get; set; }
    }
}

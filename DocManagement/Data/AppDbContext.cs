using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DocManagement.Models;

namespace DocManagement.Data
{
    public class AppDbContext : IdentityDbContext // 👈 burada dəyişiklik
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
    }
}

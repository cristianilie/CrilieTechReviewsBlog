using CrilieTechReviewsBlog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrilieTechReviewsBlog.DataManagement
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)     {         }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MainComment> MainComments { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<BlacklistedUser> BlacklistedUsers { get; set; }
    }
}

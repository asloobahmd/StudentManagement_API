using Microsoft.EntityFrameworkCore;
using StudentManagementSystemAPI.Models;

namespace StudentManagementSystemAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Student> Students { get; set; }
    }
}

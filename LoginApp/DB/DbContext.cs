using LoginApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginApp.DB
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }
    }
}

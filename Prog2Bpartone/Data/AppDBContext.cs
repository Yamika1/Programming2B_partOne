using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prog2Bpartone.Models;
using System.Security.Claims;

namespace Prog2Bpartone.Data
{
    public class AppDBContext : IdentityDbContext<Users>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Claims> Claims { get; set; }
    }
}

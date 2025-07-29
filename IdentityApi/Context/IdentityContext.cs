using IdentityApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace IdentityApi.Context
{
    public class IdentityContext:IdentityDbContext<IdentityUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext>options):base(options)
        {
            
        }
        public DbSet<UserEntity> Users { get; set; }
    }
}

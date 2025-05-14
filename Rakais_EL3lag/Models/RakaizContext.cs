using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rakais_EL3lag.Models
{
    public class RakaizContext : IdentityDbContext 
    {
       
        public RakaizContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Image> Images => Set<Image>();
        public DbSet<Question> Questions => Set<Question>();

    }
}

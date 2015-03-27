using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WeddingAPI.Models.Auth;

namespace WeddingAPI.DAL
{
    public class WeddingContext : DbContext
    {
        public DbSet<UserModel> UserTable { get; set; }
        public DbSet<SessionModel> SessionTable { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
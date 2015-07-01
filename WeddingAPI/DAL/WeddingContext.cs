using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WeddingAPI.Models.Database.Admin.About;
using WeddingAPI.Models.Database.Auth;
using WeddingAPI.Models.Database.Common;

namespace WeddingAPI.DAL
{
    public class WeddingContext : DbContext
    {
        public DbSet<UserModel> UserTable { get; set; }
        public DbSet<SessionModel> SessionTable { get; set; }
        public DbSet<ImagesModel> ImagesTable { get; set; }
        public DbSet<AdminAboutModel> AdminAboutTable { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
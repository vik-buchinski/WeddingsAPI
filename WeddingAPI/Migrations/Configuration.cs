using WeddingAPI.Models.Database.Auth;
using WeddingAPI.Utils;

namespace WeddingAPI.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.WeddingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DAL.WeddingContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            var saltValue = Common.GenerateSaltValue();
            var passwordHash = Common.HashPassword("testPassword", saltValue);
             context.UserTable.AddOrUpdate(p=> p.Id,
                 new UserModel { Email = "vik-buchinski@ya.ru", PasswordHash = passwordHash, SaltValue = saltValue }
                );
            context.SaveChanges();

        }
    }
}

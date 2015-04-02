using WeddingAPI.Models.Database.Auth;
using WeddingAPI.Utils;

namespace WeddingAPI.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.WeddingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DAL.WeddingContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            var password = "testPassword";
            var passwordHash = PasswordHash.CreateHash(password);
             context.UserTable.AddOrUpdate(p=> p.Id,
                 new UserModel { Email = "vik-buchinski@ya.ru", PasswordHash = passwordHash }
                );
            context.SaveChanges();

        }
    }
}

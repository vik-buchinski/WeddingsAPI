using WeddingAPI.Models.Database.Auth;
using WeddingAPI.Utils;

namespace WeddingAPI.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<WeddingAPI.DAL.WeddingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WeddingAPI.DAL.WeddingContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            var password = "4205distum4205";
            var passwordHash = PasswordHash.CreateHash(password);
            context.UserTable.AddOrUpdate(p => p.Id,
                new UserModel { Email = "vik-buchinski@ya.ru", PasswordHash = passwordHash }
               );
            context.UserTable.AddOrUpdate(p => p.Id,
                new UserModel { Email = "marinagrishel@gmail.com", PasswordHash = "granada01" }
               );
            context.SaveChanges();
        }
    }
}

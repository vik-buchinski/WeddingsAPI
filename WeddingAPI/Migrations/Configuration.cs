using WeddingAPI.Models.Database.Admin.About;
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
        }

        protected override void Seed(DAL.WeddingContext context)
        {

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            var password = "4205distum4205";
            var passwordHash = PasswordHash.CreateHash(password);
            context.UserTable.AddOrUpdate(u => u.Email,
                new UserModel { Email = "vik-buchinski@ya.ru", PasswordHash = passwordHash }
               );
            context.UserTable.AddOrUpdate(u => u.Email,
                new UserModel { Email = "marinagrishel@gmail.com", PasswordHash = PasswordHash.CreateHash("granada01") }
               );
            /*
            context.AdminAboutTable.AddOrUpdate(a => a.Description,
                new AdminAboutModel { Description = "this is test description"}
               );*/
            context.SaveChanges();
        }
    }
}

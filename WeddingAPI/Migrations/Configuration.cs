using WeddingAPI.Models.Auth;

namespace WeddingAPI.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

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
             context.UserTable.AddOrUpdate(p=> p.Id,
                 new UserModel { Email = "vik-buchinski@ya.ru", PasswordHash = "test hash"}
                );
            context.SaveChanges();

        }
    }
}

namespace WeddingAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserModel", "SaltValue", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModel", "SaltValue");
        }
    }
}

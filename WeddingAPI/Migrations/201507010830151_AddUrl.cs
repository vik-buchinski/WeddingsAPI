namespace WeddingAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImagesModel", "AlbumType", c => c.String());
            DropTable("dbo.AlbumModel");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AlbumModel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.ImagesModel", "AlbumType");
        }
    }
}

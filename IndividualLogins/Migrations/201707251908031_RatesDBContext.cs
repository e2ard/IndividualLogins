namespace IndividualLogins.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RatesDBContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocationId = c.Int(nullable: false),
                        Name = c.String(),
                        IsAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Updates",
                c => new
                    {
                        UpdateId = c.Int(nullable: false, identity: true),
                        IntervalNum = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        Username = c.String(),
                        UpdateTime = c.DateTime(nullable: false),
                        PickupTime = c.DateTime(nullable: false),
                        DropoffTime = c.DateTime(nullable: false),
                        Params = c.String(),
                    })
                .PrimaryKey(t => t.UpdateId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Updates");
            DropTable("dbo.News");
            DropTable("dbo.Locations");
        }
    }
}

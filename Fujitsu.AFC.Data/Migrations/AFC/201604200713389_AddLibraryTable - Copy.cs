namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLibraryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Library",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteId = c.Int(nullable: false),
                        CaseId = c.Int(nullable: false),
                        Title = c.String(),
                        ProjectId = c.Int(nullable: false),
                        IsPrimaryCase = c.Boolean(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Library", "SiteId", "dbo.Site");
            DropIndex("dbo.Library", new[] { "SiteId" });
            DropTable("dbo.Library");
        }
    }
}

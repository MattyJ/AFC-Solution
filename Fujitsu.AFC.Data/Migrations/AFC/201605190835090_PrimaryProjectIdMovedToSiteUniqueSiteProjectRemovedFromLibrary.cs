namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrimaryProjectIdMovedToSiteUniqueSiteProjectRemovedFromLibrary : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Library", "IX_SiteIdProjectId");
            AddColumn("dbo.Site", "PrimaryProjectId", c => c.Int());
            CreateIndex("dbo.Library", "SiteId");
            DropColumn("dbo.Library", "IsPrimaryCase");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Library", "IsPrimaryCase", c => c.Boolean(nullable: false));
            DropIndex("dbo.Library", new[] { "SiteId" });
            DropColumn("dbo.Site", "PrimaryProjectId");
            CreateIndex("dbo.Library", new[] { "SiteId", "ProjectId" }, unique: true, name: "IX_SiteIdProjectId");
        }
    }
}

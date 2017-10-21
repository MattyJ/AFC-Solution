namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LibrarySiteIdProjectIdUniqueKeyAdded : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Library", new[] { "SiteId" });
            CreateIndex("dbo.Library", new[] { "SiteId", "ProjectId" }, unique: true, name: "IX_SiteIdProjectId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Library", "IX_SiteIdProjectId");
            CreateIndex("dbo.Library", "SiteId");
        }
    }
}

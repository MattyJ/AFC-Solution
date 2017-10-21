namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveVirtualSite : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HistoryLog", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Task", "SiteId", "dbo.Site");
            DropIndex("dbo.HistoryLog", new[] { "SiteId" });
            DropIndex("dbo.Task", new[] { "SiteId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Task", "SiteId");
            CreateIndex("dbo.HistoryLog", "SiteId");
            AddForeignKey("dbo.Task", "SiteId", "dbo.Site", "Id");
            AddForeignKey("dbo.HistoryLog", "SiteId", "dbo.Site", "Id");
        }
    }
}

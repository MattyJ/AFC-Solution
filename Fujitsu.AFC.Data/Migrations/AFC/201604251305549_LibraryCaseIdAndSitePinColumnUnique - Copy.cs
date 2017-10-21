namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LibraryCaseIdAndSitePinColumnUnique : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Library", "CaseId", unique: true);
            CreateIndex("dbo.Site", "Pin", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Site", new[] { "Pin" });
            DropIndex("dbo.Library", new[] { "CaseId" });
        }
    }
}

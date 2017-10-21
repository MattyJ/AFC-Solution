namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectNameColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistoryLog", "ProjectName", c => c.String());
            AddColumn("dbo.Task", "ProjectName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Task", "ProjectName");
            DropColumn("dbo.HistoryLog", "ProjectName");
        }
    }
}

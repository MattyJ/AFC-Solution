namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectNameLengthAdded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HistoryLog", "ProjectName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Task", "ProjectName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Task", "ProjectName", c => c.String());
            AlterColumn("dbo.HistoryLog", "ProjectName", c => c.String());
        }
    }
}

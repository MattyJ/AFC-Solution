namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HistoryLogTaskIdColumnMadeMandatory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HistoryLog", "TaskId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HistoryLog", "TaskId", c => c.Int());
        }
    }
}

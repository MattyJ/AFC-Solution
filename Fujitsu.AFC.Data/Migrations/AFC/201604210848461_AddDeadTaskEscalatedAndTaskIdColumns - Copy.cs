namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeadTaskEscalatedAndTaskIdColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeadTask", "TaskId", c => c.Int(nullable: false));
            AddColumn("dbo.DeadTask", "Escalated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeadTask", "Escalated");
            DropColumn("dbo.DeadTask", "TaskId");
        }
    }
}

namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeadTaskTableRemovedEscalateColumnAddedToHistoryLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HistoryLog", "Escalated", c => c.Boolean());
            DropTable("dbo.DeadTask");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DeadTask",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventDetail = c.String(),
                        TaskId = c.Int(nullable: false),
                        Escalated = c.Boolean(nullable: false),
                        Handler = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        Frequency = c.String(nullable: false),
                        Pin = c.Int(),
                        ProjectId = c.Int(),
                        CaseId = c.Int(),
                        SiteTitle = c.String(maxLength: 100),
                        CaseTitle = c.String(maxLength: 100),
                        Dictionary = c.String(),
                        IsPrimary = c.Boolean(),
                        CurrentProjectId = c.Int(),
                        NewProjectId = c.Int(),
                        FromPin = c.Int(),
                        ToPin = c.Int(),
                        CurrentCaseId = c.Int(),
                        NewCaseId = c.Int(),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.HistoryLog", "Escalated");
        }
    }
}

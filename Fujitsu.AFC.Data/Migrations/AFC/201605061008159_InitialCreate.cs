namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
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
                        ListId = c.Guid(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        Url = c.String(),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId, cascadeDelete: true)
                .Index(t => t.SiteId)
                .Index(t => t.CaseId, unique: true);
            
            CreateTable(
                "dbo.Site",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Title = c.String(),
                        Pin = c.Int(nullable: false),
                        ProvisionedSiteId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProvisionedSite", t => t.ProvisionedSiteId, cascadeDelete: true)
                .Index(t => t.Pin, unique: true)
                .Index(t => t.ProvisionedSiteId);
            
            CreateTable(
                "dbo.ProvisionedSite",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsAllocated = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 36),
                        Url = c.String(),
                        ProvisionedSiteCollectionId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProvisionedSiteCollection", t => t.ProvisionedSiteCollectionId, cascadeDelete: true)
                .Index(t => t.ProvisionedSiteCollectionId);
            
            CreateTable(
                "dbo.ProvisionedSiteCollection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Parameter",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                        Description = c.String(nullable: false, maxLength: 250),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.HistoryLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(nullable: false),
                        EventType = c.String(),
                        EventDetail = c.String(),
                        CompletedDate = c.DateTime(nullable: false),
                        SiteId = c.Int(),
                        Escalated = c.Boolean(),
                        Handler = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        Frequency = c.String(nullable: false),
                        Pin = c.Int(),
                        ProjectId = c.Int(),
                        ProjectName = c.String(maxLength: 100),
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
            
            CreateTable(
                "dbo.Task",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompletedDate = c.DateTime(),
                        NextScheduledDate = c.DateTime(),
                        SiteId = c.Int(),
                        Handler = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        Frequency = c.String(nullable: false),
                        Pin = c.Int(),
                        ProjectId = c.Int(),
                        ProjectName = c.String(maxLength: 100),
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
            
            CreateTable(
                "dbo.TimerLock",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LockedInstance = c.Guid(nullable: false),
                        LockedPin = c.Int(nullable: false),
                        TaskId = c.Int(nullable: false),
                        InsertedDate = c.DateTime(nullable: false),
                        InsertedBy = c.String(nullable: false, maxLength: 150),
                        UpdatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Task", t => t.TaskId, cascadeDelete: true)
                .Index(t => t.TaskId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimerLock", "TaskId", "dbo.Task");
            DropForeignKey("dbo.Library", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Site", "ProvisionedSiteId", "dbo.ProvisionedSite");
            DropForeignKey("dbo.ProvisionedSite", "ProvisionedSiteCollectionId", "dbo.ProvisionedSiteCollection");
            DropIndex("dbo.TimerLock", new[] { "TaskId" });
            DropIndex("dbo.Parameter", new[] { "Name" });
            DropIndex("dbo.ProvisionedSiteCollection", new[] { "Name" });
            DropIndex("dbo.ProvisionedSite", new[] { "ProvisionedSiteCollectionId" });
            DropIndex("dbo.Site", new[] { "ProvisionedSiteId" });
            DropIndex("dbo.Site", new[] { "Pin" });
            DropIndex("dbo.Library", new[] { "CaseId" });
            DropIndex("dbo.Library", new[] { "SiteId" });
            DropTable("dbo.TimerLock");
            DropTable("dbo.Task");
            DropTable("dbo.HistoryLog");
            DropTable("dbo.Parameter");
            DropTable("dbo.ProvisionedSiteCollection");
            DropTable("dbo.ProvisionedSite");
            DropTable("dbo.Site");
            DropTable("dbo.Library");
        }
    }
}

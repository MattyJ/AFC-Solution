namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeadTask",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Handler = c.String(nullable: false, maxLength: 50),
                    Name = c.String(nullable: false, maxLength: 50),
                    Frequency = c.String(nullable: false),
                    EventDetail = c.String(),
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
                "dbo.SharePointGroupRefData",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 250),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);

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
                .Index(t => t.ProvisionedSiteId);

            CreateTable(
                "dbo.HistoryLog",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Handler = c.String(nullable: false, maxLength: 50),
                    Name = c.String(nullable: false, maxLength: 50),
                    Frequency = c.String(nullable: false),
                    CompletedDate = c.DateTime(nullable: false),
                    EventType = c.String(),
                    EventDetail = c.String(),
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
                    SiteId = c.Int(),
                    TaskId = c.Int(),
                    InsertedDate = c.DateTime(nullable: false),
                    InsertedBy = c.String(nullable: false, maxLength: 150),
                    UpdatedDate = c.DateTime(nullable: false),
                    UpdatedBy = c.String(nullable: false, maxLength: 150),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .Index(t => t.SiteId);

            CreateTable(
                "dbo.Task",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Handler = c.String(nullable: false, maxLength: 50),
                    Name = c.String(nullable: false, maxLength: 50),
                    Frequency = c.String(nullable: false),
                    CompletedDate = c.DateTime(),
                    NextScheduledDate = c.DateTime(),
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
                    SiteId = c.Int(),
                    InsertedDate = c.DateTime(nullable: false),
                    InsertedBy = c.String(nullable: false, maxLength: 150),
                    UpdatedDate = c.DateTime(nullable: false),
                    UpdatedBy = c.String(nullable: false, maxLength: 150),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Site", t => t.SiteId)
                .Index(t => t.SiteId);

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
            DropForeignKey("dbo.Task", "SiteId", "dbo.Site");
            DropForeignKey("dbo.HistoryLog", "SiteId", "dbo.Site");
            DropForeignKey("dbo.Site", "ProvisionedSiteId", "dbo.ProvisionedSite");
            DropForeignKey("dbo.ProvisionedSite", "ProvisionedSiteCollectionId", "dbo.ProvisionedSiteCollection");
            DropIndex("dbo.TimerLock", new[] { "TaskId" });
            DropIndex("dbo.Task", new[] { "SiteId" });
            DropIndex("dbo.HistoryLog", new[] { "SiteId" });
            DropIndex("dbo.Site", new[] { "ProvisionedSiteId" });
            DropIndex("dbo.SharePointGroupRefData", new[] { "Name" });
            DropIndex("dbo.ProvisionedSite", new[] { "ProvisionedSiteCollectionId" });
            DropIndex("dbo.ProvisionedSiteCollection", new[] { "Name" });
            DropIndex("dbo.Parameter", new[] { "Name" });
            DropTable("dbo.TimerLock");
            DropTable("dbo.Task");
            DropTable("dbo.HistoryLog");
            DropTable("dbo.Site");
            DropTable("dbo.SharePointGroupRefData");
            DropTable("dbo.ProvisionedSite");
            DropTable("dbo.ProvisionedSiteCollection");
            DropTable("dbo.Parameter");
            DropTable("dbo.DeadTask");
        }
    }
}

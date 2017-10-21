namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System.Data.Entity.Migrations;
    public partial class InsertMaximumFileSizeMbParameter : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT [dbo].[Parameter] ([Name], [Value], [Description], [InsertedDate], [InsertedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'MaximumFileSizeMb', N'20', N'Maximum file size in megabytes for any source MergePIN files.', GETDATE(), SYSTEM_USER, GETDATE(), SYSTEM_USER)");
        }
        public override void Down()
        {
        }
    }
}

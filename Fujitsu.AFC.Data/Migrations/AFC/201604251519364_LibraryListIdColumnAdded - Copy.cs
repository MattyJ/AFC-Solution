namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LibraryListIdColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Library", "ListId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Library", "ListId");
        }
    }
}

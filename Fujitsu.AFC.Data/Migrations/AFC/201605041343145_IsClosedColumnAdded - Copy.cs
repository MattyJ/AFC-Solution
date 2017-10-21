namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsClosedColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Library", "IsClosed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Library", "IsClosed");
        }
    }
}

namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestrictedUserColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Site", "RestrictedUser", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Site", "RestrictedUser");
        }
    }
}

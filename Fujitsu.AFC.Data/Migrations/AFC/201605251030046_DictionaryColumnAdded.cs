namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DictionaryColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Library", "Dictionary", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Library", "Dictionary");
        }
    }
}

namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System.Data.Entity.Migrations;

    public partial class SiteDictionaryColumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Site", "Dictionary", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Site", "Dictionary");
        }
    }
}

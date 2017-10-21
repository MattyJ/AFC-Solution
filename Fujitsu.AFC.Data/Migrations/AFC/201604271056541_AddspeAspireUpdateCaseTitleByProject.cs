namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System.Data.Entity.Migrations;

    public partial class AddspeAspireUpdateCaseTitleByProject : DbMigration
    {
        public override void Up()
        {
            Sql(DatabaseResources.Create_sp_eAspire_UpdateCaseTitleByProject);
        }

        public override void Down()
        {
            Sql(DatabaseResources.Drop_sp_eAspire_UpdateCaseTitleByProject);
        }
    }
}

namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddeAspireViews : DbMigration
    {
        public override void Up()
        {
            Sql(DatabaseResources.Create_vw_eAspireCase);
            Sql(DatabaseResources.Create_vw_eAspirePIN);
        }

        public override void Down()
        {
            Sql(DatabaseResources.Drop_vw_eAspireCase);
            Sql(DatabaseResources.Drop_vw_eAspirePIN);
        }
    }
}

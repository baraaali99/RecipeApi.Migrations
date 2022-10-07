using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(2)]
    public class _002_CreateCategoryTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(Tables.Category)
           .WithColumn("Id").AsInt32().PrimaryKey().Identity()
           .WithColumn("name").AsString()
           .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

        }
    }
}

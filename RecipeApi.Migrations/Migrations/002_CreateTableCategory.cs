using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(2)]
    public class _002_CreateTableCategory : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("Category")
           .WithColumn("name").AsString()
           .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

            Create.ForeignKey()
           .FromTable("Ingredient").ForeignColumn("name")
           .ToTable("Category");
        }
    }
}

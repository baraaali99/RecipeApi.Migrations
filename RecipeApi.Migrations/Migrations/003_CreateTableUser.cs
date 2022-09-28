using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(3)]
    public class _003_CreateTableUser : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("User")
            .WithColumn("Username").AsString().PrimaryKey()
            .WithColumn("Password").AsString().NotNullable()
            .WithColumn("RefreshToken").AsString().Unique();
        }
    }
}

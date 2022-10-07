using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
namespace RecipeApi.Migrations.Migrations
{
    [Migration(1)]
    public class _001_CreateUserTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(Tables.User)
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Username").AsString().Unique()
            .WithColumn("Password").AsString().NotNullable()
            .WithColumn("RefreshToken").AsString().Unique();
        }
    }
}

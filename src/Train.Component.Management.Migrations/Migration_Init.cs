using FluentMigrator;

namespace Train.Component.Management.Migrations;

[Migration(20250728, "init")]
public sealed class Migration_Init : Migration
{
    private const string SchemaName = "public";
    
    public override void Up()
    {
        Create.Table("items").InSchema(SchemaName)
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("unique_number").AsString(100).NotNullable()
            .WithColumn("can_assign_quantity").AsBoolean().WithDefaultValue(false).NotNullable();

        Create.Table("item_quantities").InSchema(SchemaName)
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("item_id").AsInt64().ForeignKey("FK_ItemQuantities_Items", "items", "id")
            .WithColumn("quantity").AsInt32().NotNullable();
        
        // used for faster work with large datasets 
        Create.Index("ix_items_unique_number").OnTable("items").OnColumn("unique_number");
        Create.Index("ix_items_name").OnTable("items").OnColumn("name");
        Create.Index("ix_item_quantities_item_id").OnTable("item_quantities").OnColumn("item_id");
    }

    public override void Down()
    {
        Delete.Table("item_quantities");
        Delete.Table("items");
    }
}
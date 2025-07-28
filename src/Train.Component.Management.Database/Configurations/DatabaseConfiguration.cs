namespace Train.Component.Management.Database.Configurations;

public sealed class DatabaseConfiguration
{
    public required string ConnectionString { get; set; }
    public required string SchemaName { get; set; }
}
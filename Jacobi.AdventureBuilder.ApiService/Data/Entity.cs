namespace Jacobi.AdventureBuilder.ApiService.Data;

internal interface ILogicalPartition
{
    string PartitionPath { get; }
    string PartitionKey { get; }
}

internal abstract record Entity(string id) : ILogicalPartition
{
    public virtual string PartitionPath { get; } = "id";
    public virtual string PartitionKey => id;
}
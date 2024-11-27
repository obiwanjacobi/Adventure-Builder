namespace Jacobi.AdventureBuilder.ApiService.Data;

internal interface ILogicalPartition
{
    static abstract string ContainerName { get; }
    static abstract string PartitionPath { get; }
    string PartitionKey { get; }
}

internal abstract record Entity(string id) //: ILogicalPartition
{
    //public abstract string ContainerName { get; }
    public static string PartitionPath { get; } = "id";
    public virtual string PartitionKey => id;
}
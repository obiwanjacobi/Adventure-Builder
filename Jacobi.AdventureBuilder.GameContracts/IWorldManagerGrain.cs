using System.Runtime.InteropServices;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

[Guid("060551BF-E482-4E71-9DA9-7DC6C0EA15F0")]
[Alias("AdventureBuilder.IWorldManagerGrain")]
public interface IWorldManagerGrain : IGrainWithGuidKey
{
    [Alias("CreateWorld")]
    Task<IWorldGrain> CreateWorld(string worldId);
    [Alias("FindWorld")]
    Task<Option<IWorldGrain>> FindWorld(string worldNameOrId);
}

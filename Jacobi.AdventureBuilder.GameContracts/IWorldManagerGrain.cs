using System.Runtime.InteropServices;
using LanguageExt;

namespace Jacobi.AdventureBuilder.GameContracts;

[Guid("060551BF-E482-4E71-9DA9-7DC6C0EA15F0")]
public interface IWorldManagerGrain : IGrainWithGuidKey
{
    Task<IAdventureWorld> CreateWorld(string adventureId);
    ValueTask<Option<IAdventureWorld>> FindWorld(string worldNameOrId);
}

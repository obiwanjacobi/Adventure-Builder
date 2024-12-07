using Microsoft.AspNetCore.Components;

namespace Jacobi.AdventureBuilder.Web.Features.Counter;

// code-behind experiment

public partial class Counter : ComponentBase
{
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}

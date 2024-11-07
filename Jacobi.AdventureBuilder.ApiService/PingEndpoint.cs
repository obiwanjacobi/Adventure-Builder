using FastEndpoints;

namespace Jacobi.AdventureBuilder.ApiService;

public sealed record TestResponse(string Status);

public sealed class PingEndpoint : EndpointWithoutRequest<TestResponse>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return SendOkAsync(new TestResponse("OK"));
    }
}

using FastEndpoints;
using FluentValidation;

namespace Jacobi.AdventureBuilder.ApiService;

public sealed record TestRequest(string Text);
public sealed record TestResponse(string Text);

public sealed class TestRequestValidator : Validator<TestRequest>
{
    public TestRequestValidator()
    {
        RuleFor(r => r.Text)
            .NotEmpty();
    }
}

public sealed class TestEndpoint : Endpoint<TestRequest, TestResponse>
{
    public override void Configure()
    {
        Post("test");
        AllowAnonymous();
    }

    public override Task HandleAsync(TestRequest req, CancellationToken ct)
    {
        return SendOkAsync(new TestResponse(req.Text));
    }
}

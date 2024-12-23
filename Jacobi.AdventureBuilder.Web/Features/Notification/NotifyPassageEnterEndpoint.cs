using FastEndpoints;
using FluentValidation;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

internal sealed record class NotifyPassageEnterRequest(string PassageKey, string CharacterKey);

internal sealed class NotifyPassageEnterValidator : Validator<NotifyPassageEnterRequest>
{
    public NotifyPassageEnterValidator()
    {
        RuleFor(r => r.PassageKey)
            .NotEmpty();
        RuleFor(r => r.CharacterKey)
            .NotEmpty();
    }
}

internal sealed class NotifyPassageEnterEndpoint(INotificationService notificationService)
    : Endpoint<NotifyPassageEnterRequest>
{
    private readonly INotificationService _notificationService = notificationService;

    public override void Configure()
    {
        Post("/notify/passage/enter");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(NotifyPassageEnterRequest req, CancellationToken ct)
    {
        await _notificationService.NotifyPassageEnterAsync(req.PassageKey, req.CharacterKey);
    }
}


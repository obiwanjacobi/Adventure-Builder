using FastEndpoints;
using FluentValidation;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

internal sealed record class NotifyPassageExitRequest(string PassageKey, string OccupantKey);

internal sealed class NotifyPassageExitValidator : Validator<NotifyPassageExitRequest>
{
    public NotifyPassageExitValidator()
    {
        RuleFor(r => r.PassageKey)
            .NotEmpty();
        RuleFor(r => r.OccupantKey)
            .NotEmpty();
    }
}

internal sealed class NotifyPassageExitEndpoint(INotificationService notificationService)
    : Endpoint<NotifyPassageExitRequest>
{
    private readonly INotificationService _notificationService = notificationService;

    public override void Configure()
    {
        Post("notify/passage/exit");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(NotifyPassageExitRequest req, CancellationToken ct)
    {
        await _notificationService.NotifyPassageExit(req.PassageKey, req.OccupantKey);
    }
}


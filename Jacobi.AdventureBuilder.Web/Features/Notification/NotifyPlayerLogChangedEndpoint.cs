using FastEndpoints;
using FluentValidation;

namespace Jacobi.AdventureBuilder.Web.Features.Notification;

internal sealed record class NotifyPlayerLogChangedRequest(string PlayerKey);

internal sealed class NotifyPlayerLogChangedValidator : Validator<NotifyPlayerLogChangedRequest>
{
    public NotifyPlayerLogChangedValidator()
    {
        RuleFor(r => r.PlayerKey)
            .NotEmpty();
    }
}

internal sealed class NotifyPlayerLogChangedEndpoint(INotificationService notificationService)
    : Endpoint<NotifyPlayerLogChangedRequest>
{
    private readonly INotificationService _notificationService = notificationService;

    public override void Configure()
    {
        Post("notify/player/logchanged");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(NotifyPlayerLogChangedRequest req, CancellationToken ct)
    {
        await _notificationService.NotifyPlayerLogChanged(req.PlayerKey);
    }
}

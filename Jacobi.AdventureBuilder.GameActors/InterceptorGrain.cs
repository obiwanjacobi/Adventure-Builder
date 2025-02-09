using Microsoft.Extensions.Logging;

namespace Jacobi.AdventureBuilder.GameActors;

public sealed class InterceptorGrain : IIncomingGrainCallFilter
{
    private readonly ILogger _logger;

    public InterceptorGrain(ILogger<InterceptorGrain> logger)
    {
        _logger = logger;
    }
    public Task Invoke(IIncomingGrainCallContext context)
    {
        var grainId = context.TargetId;
        if (context.InterfaceName.StartsWith("Jacobi"))
        {
            var arguments = "";
            for (int i = 0; i < context.Request.GetArgumentCount(); i++)
            {
                string? strArg = null;
                var arg = context.Request.GetArgument(i);
                if (arg is string)
                    strArg = arg.ToString();
                if (arg is IGrain grain)
                    strArg = grain.GetPrimaryKeyString();

                if (strArg is not null)
                {
                    if (arguments.Length > 0)
                        arguments += ", ";
                    arguments += $"\"{strArg}\"";
                }
            }

            _logger.LogInformation($"=> Calling: {context.InterfaceName}.{context.Request.GetMethodName()}({arguments}) [{grainId.Key}]");
        }
        return context.Invoke();
    }
}

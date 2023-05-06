using System.Threading;
using System.Threading.Tasks;
using Mediator.Behaviors.Authorization.Models;

namespace Mediator.Behaviors.Authorization.Interfaces;

public interface IAuthorizationHandler<TRequirement>
    where TRequirement : IAuthorizationRequirement
{
    Task<AuthorizationResult> Handle(TRequirement requirement, CancellationToken cancellationToken = default);
}
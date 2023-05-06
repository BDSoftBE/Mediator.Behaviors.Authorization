using System.Collections.Generic;
using Mediator.Behaviors.Authorization.Interfaces;

namespace Mediator.Behaviors.Authorization.Models;

public abstract class AbstractRequestAuthorizer<TRequest> : IAuthorizer<TRequest>
{
    private HashSet<IAuthorizationRequirement> _requirements { get; set; } = new HashSet<IAuthorizationRequirement>();

    public IEnumerable<IAuthorizationRequirement> Requirements => _requirements;

    protected void UseRequirement(IAuthorizationRequirement requirement)
    {
        if (requirement == null) return;
        _requirements.Add(requirement);
    }

    public virtual void ClearRequirements() => _requirements.Clear();

    public abstract void BuildPolicy(TRequest request);
}
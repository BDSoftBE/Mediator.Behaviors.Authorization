using Mediator.Behaviors.Authorization.Models;

namespace GlobalAuthorizerExample.Common.Authorization
{
    public class GlobalAuthorizer<TRequest> : AbstractRequestAuthorizer<TRequest>
    {
        public override void BuildPolicy(TRequest request) 
            => UseRequirement(new MustBeAuthenticatedRequirement());
    }
}

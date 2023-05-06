﻿using System.Collections.Generic;

namespace Mediator.Behaviors.Authorization.Interfaces;

public interface IAuthorizer<T>
{
    IEnumerable<IAuthorizationRequirement> Requirements { get; }
    void ClearRequirements();
    void BuildPolicy(T instance);
}
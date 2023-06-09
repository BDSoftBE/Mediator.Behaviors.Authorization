# Mediator.Behaviors.Authorization

[![NuGet](https://img.shields.io/nuget/v/Mediator.Behaviors.Authorization.svg)](https://www.nuget.org/packages/Mediator.Behaviors.Authorization/)

A simple request authorization package that allows you to build and run request specific authorization requirements before your request handler is called for [Mediator][mediator]. 

This library is inspired on the [MediatR.Behaviors.Authorization][mediatrv] library for MediatR.

## Installation

Using the [.NET Core command-line interface (CLI) tools][dotnet-core-cli-tools]:

```sh
dotnet add package Mediator.Behaviors.Authorization
```

Using the [NuGet Command Line Interface (CLI)][nuget-cli]:

```sh
nuget install Mediator.Behaviors.Authorization
```

Using the [Package Manager Console][package-manager-console]:

```powershell
Install-Package Mediator.Behaviors.Authorization
```

From within Visual Studio:

1. Open the Solution Explorer.
2. Right-click on a project within your solution.
3. Click on *Manage NuGet Packages...*
4. Click on the *Browse* tab and search for "Mediator.Behaviors.Authorization".
5. Click on the Mediator.Behaviors.Authorization package, select the latest version in the
   right-tab and click *Install*.


## Getting Started

### Dependency Injection

You will need to register the authorization pipeline along with all implementations of `IAuthorizer`:

```c#
using Mediator.Behaviors.Authorization.Extensions.DependencyInjection;

public class Startup
{
	//...
	public void ConfigureServices(IServiceCollection services)
	{
		// Adds the transient pipeline behavior and additionally registers all `IAuthorizationHandlers` for a given assembly
		services.AddMediatorAuthorization(Assembly.GetExecutingAssembly());
		// Register all `IAuthorizer` implementations for a given assembly
		services.AddAuthorizersFromAssembly(Assembly.GetExecutingAssembly())

	}
}
```
You can use the helper method to register 'IAuthorizer' implementations from an assembly or manually inject them using Microsoft's DI methods.

## Example Usage

Scenario: We need to get details about a specific video for a course on behalf of a user. However, this video course information is considered privileged information and we only want users with a subscription to that course to have access to the information about the video.

### Creating an Authorization Requirement `IAuthorizationRequirement`

Location: `~/Application/Authorization/MustHaveCourseSubscriptionRequirement.cs`

You can create custom, reusable authorization rules for your Mediator requests by implementing `IAuthorizationRequirement` and `IAuthorizationHandler<TAuthorizationRequirement>`:

```c#
public class MustHaveCourseSubscriptionRequirement : IAuthorizationRequirement
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }

        class MustHaveCourseSubscriptionRequirementHandler : IAuthorizationHandler<MustHaveCourseSubscriptionRequirement>
        {
            private readonly IApplicationDbContext _applicationDbContext;

            public MustHaveCourseSubscriptionRequirementHandler(IApplicationDbContext applicationDbContext)
            {
                _applicationDbContext = applicationDbContext;
            }

            public async Task<AuthorizationResult> Handle(MustHaveCourseSubscriptionRequirement request, CancellationToken cancellationToken)
            {
                var userId = request.UserId;
                var userCourseSubscription = await _applicationDbContext.UserCourseSubscriptions
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == request.CourseId, cancellationToken);

                if (userCourseSubscription != null)
                    return AuthorizationResult.Succeed();

                return AuthorizationResult.Fail("You don't have a subscription to this course.");
            }
        }
    }
```
In the preceding listing, you can see this is your standard Mediator Request/Request Handler usage; so you can treat the whole affair as you normally would. It is important to note you must return `AuthorizationResult` You can fail two ways: `AuthorizationResult.Fail()` or `AuthorizationResult.Fail("your message here")` and you can pass using `AuthorizationResult.Succeed()`

### Basic Mediator Request

Location: `~/Application/Courses/Queries/GetCourseVideoDetail/GetCourseVideoDetailQuery.cs`

```c#
public class GetCourseVideoDetailQuery : IRequest<CourseVideoDetailVm>
    {
        public int CourseId { get; set; }
        public int VideoId { get; set; }
        
        class GetCourseVideoDetailQueryHandler : IRequestHandler<GetCourseVideoDetailQuery>
        {
            private readonly IApplicationDbContext _applicationDbContext;

            public GetCourseVideoDetailQueryHandler(IApplicationDbContext applicationDbContext)
            {
                _applicationDbContext = applicationDbContext;
            }

            public async Task<CourseVideoDetailVm> Handle(GetCourseVideoDetailQuery request, CancellationToken cancellationToken)
            {
                var video = await _applicationDbContext.CourseVideos
                    .FirstOrDefaultAsync(x => x.CourseId == request.CourseId && x.VideoId == request.VideoId, cancellationToken);

                return new CourseVideoDetailVm(video);
            }
        }
    }
```

### Creating the `IAuthorizer`

Location: `~/Application/Courses/Queries/GetCourseVideoDetail/GetCourseVideoDetailAuthorizer.cs`

```c#
public class GetCourseVideoDetailAuthorizer : AbstractRequestAuthorizer<GetCourseVideoDetailQuery>
    {
        private readonly ICurrentUserService _currentUserService;

        public GetCourseVideoDetailAuthorizer(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override void BuildPolicy(GetCourseVideoDetailQuery request)
        {
            UseRequirement(new MustHaveCourseSubscriptionRequirement
            {
                CourseId = request.CourseId,
                UserId = _currentUserService.UserId
            });
        }
    }
```
The usage of `AbstractRequestAuthorizer<TRequest>` will usually be preferable; this abstract class does a couple things for us. It takes care of initializing and adding new requirements to the `Requirements` property through the `UseRequirement(IAuthorizationRequirement)`, finally, it still forces the class extending it to implement the `IAuthorizer.BuildPolicy()` method which is very important for passing the needed arguments to the authorization requirement that handles the authorization logic.

For any requests, bug or comments, please [open an issue][issues] or [submit a pull request][pulls].

[dotnet-core-cli-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/
[issues]: https://github.com/BDSoftBE/Mediator.Behaviors.Authorization/issues/new
[nuget-cli]: https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference
[package-manager-console]: https://docs.microsoft.com/en-us/nuget/tools/package-manager-console
[pulls]: https://github.com/BDSoftBE/Mediator.Behaviors.Authorization/pulls
[mediatrv]: https://github.com/AustinDavies/MediatR.Behaviors.Authorization
[mediator]: https://github.com/martinothamar/Mediator
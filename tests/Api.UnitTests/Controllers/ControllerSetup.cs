using System.Reflection;
using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     ControllerSetup class.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class ControllerSetup<T> where T : ApiControllerBase, new()
{
    /// <summary>
    ///     The mediator mock.
    /// </summary>
    protected readonly Mock<ISender> MediatorMock;

    /// <summary>
    ///     The service provider.
    /// </summary>
    protected readonly T Controller;

    /// <summary>
    ///     Initializes ControllerSetup.
    /// </summary>
    protected ControllerSetup()
    {
        MediatorMock = new Mock<ISender>();
        var services = new ServiceCollection();

        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient(_ => MediatorMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        Controller = new T
        {
            ControllerContext = new ControllerContext
                { HttpContext = new DefaultHttpContext { RequestServices = serviceProvider } }
        };
    }
}
using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Observa.Architecture.Tests;

public sealed class NamingConventionTests
{
    private static readonly Assembly s_applicationAssembly = typeof(Application.DependencyInjection).Assembly;
    private static readonly Assembly s_domainAssembly = typeof(Domain.Abstractions.Entity).Assembly;

    [Fact]
    public void CommandHandlers_ShouldHaveNameEndingWithCommandHandler()
    {
        var result = Types.InAssembly(s_applicationAssembly)
            .That()
            .ImplementInterface(typeof(Application.Abstractions.Messaging.ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(Application.Abstractions.Messaging.ICommandHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryHandlers_ShouldHaveNameEndingWithQueryHandler()
    {
        var result = Types.InAssembly(s_applicationAssembly)
            .That()
            .ImplementInterface(typeof(Application.Abstractions.Messaging.IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEntities_ShouldBeSealed()
    {
        var result = Types.InAssembly(s_domainAssembly)
            .That()
            .Inherit(typeof(Domain.Abstractions.Entity))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Repositories_ShouldBeInterfaces()
    {
        var result = Types.InAssembly(s_domainAssembly)
            .That()
            .ResideInNamespace("Observa.Domain.Repositories")
            .Should()
            .BeInterfaces()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}

using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Observa.Architecture.Tests;

public sealed class CleanArchitectureTests
{
    private static readonly Assembly s_domainAssembly = typeof(Domain.Abstractions.Entity).Assembly;
    private static readonly Assembly s_applicationAssembly = typeof(Application.DependencyInjection).Assembly;
    private static readonly Assembly s_infrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
    private static readonly Assembly s_apiAssembly = typeof(Api.Middleware.GlobalExceptionHandler).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOnApplication()
    {
        var result = Types.InAssembly(s_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Observa.Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(s_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Observa.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(s_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Observa.Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructure()
    {
        var result = Types.InAssembly(s_applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Observa.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(s_applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Observa.Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(s_infrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn("Observa.Api")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOnMediatR()
    {
        var result = Types.InAssembly(s_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("MediatR")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ShouldNotDependOnEntityFramework()
    {
        var result = Types.InAssembly(s_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}

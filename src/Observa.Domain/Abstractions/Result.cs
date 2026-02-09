using System;
using System.Diagnostics.CodeAnalysis;

namespace Observa.Domain.Abstractions;

/// <summary>
/// Representa el resultado de una operacion que puede fallar.
/// Encapsula exito o fallo sin usar excepciones para flujo de control.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }
}

/// <summary>
/// Resultado generico que contiene un valor en caso de exito.
/// Hereda de Result para permitir su uso como constraint generico en behaviors.
/// </summary>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("No se puede acceder al valor de un resultado fallido.");

#pragma warning disable CA1000
    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(value, true, Error.None);
    }

    public new static Result<TValue> Failure(Error error)
    {
        return new Result<TValue>(default, false, error);
    }
#pragma warning restore CA1000
}

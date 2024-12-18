using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Managers.Api.Extensions;

public static class RequestExtensions
{
    public static async Task<ValidationResult> ValidateAsync<TRequest>(this TRequest request)
        where TRequest : RequestBase
    {
        var requestType = request.GetType();
        var validatorFullName = $"{requestType.FullName}Validator";

        try
        {
            var validator = Activator.CreateInstance(requestType.Assembly.FullName, validatorFullName)!.Unwrap()!;
            var validatorType = validator.GetType();

            return await (Task<ValidationResult>)validatorType
                .GetMethod("ValidateAsync", [requestType, typeof(CancellationToken)])!
                .Invoke(validator, [request, CancellationToken.None])!;
        }
        catch (TypeLoadException ex)
        {
            throw new TypeLoadException(
                $"Validator '{requestType.Name}Validator' was not found for '{requestType.Name}'. " +
                $"You must place a '{requestType.Name}Validator' in the namespace of the request type ('{requestType.Namespace!}').",
                ex
            );
        }
    }
}
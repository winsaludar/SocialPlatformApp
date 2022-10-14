using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Extensions;

public static class ValidationResultExtensions
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        => result.Errors.ForEach(x => modelState.AddModelError(x.PropertyName, x.ErrorMessage));

    public static void ThrowHubException(this ValidationResult result)
    {
        IEnumerable<string> errors = result.Errors.Select(x => x.ErrorMessage);
        string errorMessage = string.Join(",", errors);

        throw new HubException(errorMessage);
    }
}

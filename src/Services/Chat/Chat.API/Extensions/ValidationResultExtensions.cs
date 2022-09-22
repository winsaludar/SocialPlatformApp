using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chat.API.Extensions;

public static class ValidationResultExtensions
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        => result.Errors.ForEach(x => modelState.AddModelError(x.PropertyName, x.ErrorMessage));
}

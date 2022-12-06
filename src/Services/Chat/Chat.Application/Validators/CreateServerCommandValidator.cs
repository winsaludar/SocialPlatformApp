using Chat.Application.Commands;
using Chat.Application.Extensions;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class CreateServerCommandValidator : AbstractValidator<CreateServerCommand>
{
    public CreateServerCommandValidator(IRepositoryManager repositoryManager)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MustNotBeExistingServerName(repositoryManager);
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LongDescription).NotEmpty();
        RuleFor(x => x.CreatorEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CreatedById).NotEmpty();

        RuleFor(x => x.Categories).Must(categories =>
        {
            // Allow empty categories
            if (!categories.Any())
                return true;

            // Both id and name must be valid
            categories.ForEach(item =>
            {
                Category? category;
                try
                {
                    category = Enumeration.FromValue<Category>(item.Id);
                }
                catch (Exception)
                {
                    throw new InvalidCategoryException(item.Name, item.Id);
                }

                if (category.Name.ToLower() != item.Name.ToLower())
                    throw new InvalidCategoryException(item.Name, item.Id);
            });

            return true;
        });
    }
}

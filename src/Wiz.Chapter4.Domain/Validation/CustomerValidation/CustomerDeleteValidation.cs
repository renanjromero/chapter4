using FluentValidation;
using Wiz.Chapter4.Domain.Models;

namespace Wiz.Chapter4.Domain.Validation.CustomerValidation
{
    public class CustomerDeleteValidation : AbstractValidator<Customer>
    {
        public CustomerDeleteValidation()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("Id não pode ser nulo");
        }
    }
}

using System.ComponentModel.DataAnnotations;
using RefactorThis.Controllers;

namespace RefactorThis.Validators
{
    public class CreateOrUpdateOptionRequestValidator: ICreateOrUpdateOptionRequestValidator
    {
        public void ValidateRequest(CreateOrUpdateOptionRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ValidationException("Option name must be provided");
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;
using RefactorThis.Controllers;

namespace RefactorThis.Validators
{
    public class CreateOrUpdateProductRequestValidator: ICreateOrUpdateProductRequestValidator
    {
        public void ValidateRequest(CreateOrUpdateProductRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ValidationException("Product name must be provided");
            }
        }
    }
}
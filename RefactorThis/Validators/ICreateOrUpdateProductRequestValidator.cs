using RefactorThis.Controllers;

namespace RefactorThis.Validators
{
    public interface ICreateOrUpdateProductRequestValidator
    {
        void ValidateRequest(CreateOrUpdateProductRequest request);
    }
}
using RefactorThis.Controllers;

namespace RefactorThis.Validators
{
    public interface ICreateOrUpdateOptionRequestValidator
    {
        void ValidateRequest(CreateOrUpdateOptionRequest request);
    }
}

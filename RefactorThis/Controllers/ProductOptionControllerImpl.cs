using System;
using System.Threading.Tasks;
using RefactorThis.Models;

namespace RefactorThis.Controllers
{
    public class ProductOptionControllerImpl: IOptionController
    {
        public Task<OptionsRetrievedResponse> GetOptionsAsync(Guid id)
        {
            // return new ProductOptions(productId);
            throw new NotImplementedException();
        }

        public async Task<OptionAddedResponse> AddOptionAsync(Guid id, CreateOrUpdateOptionRequest body)
        {
            var optionId = Guid.NewGuid();
            var option = new ProductOption{Id = optionId, Name = body.Name, Description = body.Description};
            option.ProductId = id;
            option.Save();
            return new OptionAddedResponse { Id = optionId };
        }

        public async Task<Option> GetOptionAsync(Guid id, Guid optionId)
        {
            var option = new ProductOption(id);
            if (option.IsNew)
                throw new Exception();

            return new Option{Id = option.Id, Name = option.Name, Description = option.Description};
        }

        public async Task UpdateOptionAsync(Guid id, Guid optionId, CreateOrUpdateOptionRequest body)
        {
            var orig = new ProductOption(id)
            {
                Name = body.Name,
                Description = body.Description
            };

            if (!orig.IsNew)
                orig.Save();
        }

        public async Task DeleteOptionAsync(Guid id, Guid optionId)
        {
            var opt = new ProductOption(id);
            opt.Delete();
        }
    }
}
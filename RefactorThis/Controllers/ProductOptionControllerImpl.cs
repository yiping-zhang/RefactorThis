using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Exceptions;
using RefactorThis.Models;
using RefactorThis.Repositories;
using RefactorThis.Validators;

namespace RefactorThis.Controllers
{
    public class ProductOptionControllerImpl: IOptionController
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductOptionRepository _optionRepository;
        private readonly ICreateOrUpdateOptionRequestValidator _validator;

        public ProductOptionControllerImpl(IProductRepository productRepository,
            IProductOptionRepository optionRepository,
            ICreateOrUpdateOptionRequestValidator validator)
        {
            _optionRepository = optionRepository;
            _validator = validator;
            _productRepository = productRepository;
        }

        public async Task<OptionsRetrievedResponse> GetOptionsAsync(Guid productId)
        {
            var dbOptions = await _optionRepository.RetrieveOptions(productId);
            var dtoOptions = dbOptions.Select(Map).ToList();
            return new OptionsRetrievedResponse {Items = dtoOptions};
        }

        public async Task<OptionAddedResponse> AddOptionAsync(Guid productId, CreateOrUpdateOptionRequest body)
        {
            _validator.ValidateRequest(body);

            var product = await _productRepository.RetrieveProduct(productId);
            if (product == null)
            {
                // TODO add foreign key constrain between Products and ProductOptions tables
                // https://stackoverflow.com/questions/1884818/how-do-i-add-a-foreign-key-to-an-existing-sqlite-table?answertab=active#tab-top
                throw new NotFoundException($"Product with Id: {productId} not found");
            }

            var dbOption = Map(body);
            dbOption.ProductId = productId;
            var optionId = await _optionRepository.AddOption(productId, dbOption);
            return new OptionAddedResponse { Id = optionId };
        }

        public async Task<Option> GetOptionAsync(Guid productId, Guid optionId)
        {
            var dbOption = await _optionRepository.GetOption(optionId);
            return Map(dbOption);
        }

        public async Task UpdateOptionAsync(Guid productId, Guid optionId, CreateOrUpdateOptionRequest body)
        {
            _validator.ValidateRequest(body);
            var dbOption = Map(body);
            var numberOfRowsAffected = await _optionRepository.UpdateOption(productId, optionId, dbOption);
            if (numberOfRowsAffected == 0)
            {
                throw new NotFoundException($"No option found with Id: {optionId} belongs to product with Id: {productId}");
            }
        }

        public async Task DeleteOptionAsync(Guid productId, Guid optionId)
        {
            await _optionRepository.DeleteOption(productId, optionId);
        }

        private static Option Map(ProductOption dbOption)
        {
            return new Option
            {
                Id = dbOption.Id, Name = dbOption.Name, Description = dbOption.Description
            };
        }

        private static ProductOption Map(CreateOrUpdateOptionRequest dtoOption)
        {
            return new ProductOption {Name = dtoOption.Name, Description = dtoOption.Description};
        }
    }
}

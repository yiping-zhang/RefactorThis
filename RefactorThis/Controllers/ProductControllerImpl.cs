using System;
using System.Linq;
using System.Threading.Tasks;
using RefactorThis.Exceptions;
using RefactorThis.Repositories;
using RefactorThis.Validators;

namespace RefactorThis.Controllers
{
    public class ProductControllerImpl: IProductController
    {
        private readonly IProductRepository _repository;
        private readonly ICreateOrUpdateProductRequestValidator _validator;

        public ProductControllerImpl(IProductRepository repository, ICreateOrUpdateProductRequestValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<ProductsRetrievedResponse> GetProductsAsync(string name, int? limit, int? offset)
        {
            var dbResults = await _repository.RetrieveProducts(name, limit, offset);
            var dtoProducts = dbResults.Select(Map).ToList();
            return new ProductsRetrievedResponse {Items = dtoProducts};
        }

        public async Task<ProductCreatedResponse> CreateProductAsync(CreateOrUpdateProductRequest body)
        {
            _validator.ValidateRequest(body);
            var dbProduct = Map(body);
            var productId = await _repository.CreateProduct(dbProduct);
            return new ProductCreatedResponse{Id = productId};
        }

        public async Task<Product> GetProductAsync(Guid id)
        {
            var dbProduct = await _repository.RetrieveProduct(id);
            return dbProduct == null ? null : Map(dbProduct);
        }

        public async Task UpdateProductAsync(Guid id, CreateOrUpdateProductRequest body)
        {
            _validator.ValidateRequest(body);
            var dbProduct = Map(body);
            var numberOfRowsAffected = await _repository.UpdateProduct(id, dbProduct);
            if (numberOfRowsAffected == 0)
            {
                throw new NotFoundException($"No product found with Id: {id}");
            }
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _repository.DeleteProduct(id);
        }

        private static RefactorThis.Models.Product Map(CreateOrUpdateProductRequest dtoProduct)
        {
            return new RefactorThis.Models.Product
            {
                Name = dtoProduct.Name,
                Description = dtoProduct.Description,
                Price = dtoProduct.Price,
                DeliveryPrice = dtoProduct.DeliveryPrice
            };
        }

        private static Product Map(RefactorThis.Models.Product dbProduct)
        {
            return new Product
            {
                Id = dbProduct.Id, Name = dbProduct.Name, Description = dbProduct.Description,
                Price = dbProduct.Price, DeliveryPrice = dbProduct.DeliveryPrice
            };
        }
    }
}

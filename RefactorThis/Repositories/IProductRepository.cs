using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RefactorThis.Models;

namespace RefactorThis.Repositories
{
    public interface IProductRepository
    {
        Task<Product> RetrieveProduct(Guid id);
        Task DeleteProduct(Guid id);
        Task<Guid> CreateProduct(Product product);
        Task<IEnumerable<Product>> RetrieveProducts(string name, int? offset, int? limit);
        Task<int> UpdateProduct(Guid id, Product product);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RefactorThis.Models;

namespace RefactorThis.Repositories
{
    public interface IProductOptionRepository
    {
        Task<IEnumerable<ProductOption>> RetrieveOptions(Guid productId);
        Task<Guid> AddOption(Guid productId, ProductOption option);
        Task<ProductOption> GetOption(Guid optionId);
        Task<int> UpdateOption(Guid productId, Guid optionId, ProductOption option);
        Task DeleteOption(Guid productId, Guid optionId);
    }
}

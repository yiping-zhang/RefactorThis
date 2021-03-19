using NUnit.Framework;
using RefactorThis.Controllers;
using RefactorThis.Repositories;
using RefactorThis.Validators;

namespace RefactorThis.Tests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
    {
        private IProductRepository _productRepository;
        private ICreateOrUpdateProductRequestValidator _validator;
        
        private ProductControllerImpl _subject;

        [SetUp]
        public void SetUp()
        {
            
        }
    }
}
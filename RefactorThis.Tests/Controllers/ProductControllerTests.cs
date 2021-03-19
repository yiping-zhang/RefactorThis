using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RefactorThis.Controllers;
using RefactorThis.Exceptions;
using RefactorThis.Repositories;
using RefactorThis.Validators;
using Shouldly;
using TestStack.BDDfy;
using Product = RefactorThis.Models.Product;

namespace RefactorThis.Tests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
    {
        private readonly Guid _productId = new Guid("0307fae5-37ff-45c4-95d0-517436d6c0b5");
        private CreateOrUpdateProductRequest _request;

        private IProductRepository _productRepository;
        private ICreateOrUpdateProductRequestValidator _validator;

        private ProductControllerImpl _subject;

        private Exception _actualException;

        [SetUp]
        public void SetUp()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _validator = new CreateOrUpdateProductRequestValidator();

            _subject = new ProductControllerImpl(_productRepository, _validator);
        }

        [Test]
        public void ItShouldThrowExceptionIfNoProductCanBeFoundToUpdate()
        {
            this.Given(x => GivenNoProductInRepositoryCanBeFoundWithId(_productId))
                .And(x => GivenAnUpdateProductRequest(_productId))
                .When(x => WhenPerformUpdateOperation())
                .Then(x => ThenNotFoundExceptionShouldBeThrownWithMessage($"No product found with Id: {_productId}"))
                .BDDfy();
        }

        private void GivenNoProductInRepositoryCanBeFoundWithId(Guid productId)
        {
            _productRepository.UpdateProduct(productId, Arg.Any<Product>()).Returns(0);
        }

        private void GivenAnUpdateProductRequest(Guid productId)
        {
            _request = new CreateOrUpdateProductRequest{Name = "LG"};
        }

        private async Task WhenPerformUpdateOperation()
        {
            try
            {
                await _subject.UpdateProductAsync(_productId, _request);
            }
            catch (NotFoundException exception)
            {
                _actualException = exception;
            }
        }

        private void ThenNotFoundExceptionShouldBeThrownWithMessage(string expectedExceptionMessage)
        {
            _actualException.Message.ShouldBe(expectedExceptionMessage);
        }
    }
}

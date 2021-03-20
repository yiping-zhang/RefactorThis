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
    public class ProductOptionControllerTests
    {
        private readonly Guid _productId = new Guid("0307fae5-37ff-45c4-95d0-517436d6c0b5");
        private readonly Guid _optionId = new Guid("de1287c0-4b15-4a7b-9d8a-dd21b3cafec3");
        private CreateOrUpdateOptionRequest _request;

        private IProductRepository _productRepository;
        private IProductOptionRepository _optionRepository;
        private ICreateOrUpdateOptionRequestValidator _validator;

        private ProductOptionControllerImpl _subject;

        private Exception _actualException;

        [SetUp]
        public void SetUp()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _optionRepository = Substitute.For<IProductOptionRepository>();
            _validator = new CreateOrUpdateOptionRequestValidator();

            _subject = new ProductOptionControllerImpl(_productRepository, _optionRepository, _validator);
        }

        [Test]
        public void ItShouldThrowExceptionIfAnOptionIsAddedToNonExistingProduct()
        {
            this.Given(x => GivenAddOptionRequest())
                .And(x => GivenNoProductExistsWithId(_productId))
                .When(x => WhenAddOptionToProduct())
                .Then(x => ThenNotFoundExceptionShouldBeThrownWithMessage($"Product with Id: {_productId} not found"))
                .BDDfy();
        }

        private void GivenAddOptionRequest()
        {
            _request = new CreateOrUpdateOptionRequest{Name = "White", Description = "White LG"};
        }

        private void GivenNoProductExistsWithId(Guid productId)
        {
            _productRepository.RetrieveProduct(productId).Returns((Product)null);
        }

        private async Task WhenAddOptionToProduct()
        {
            try
            {
                await _subject.AddOptionAsync(_productId, _request);
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

using System;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using RefactorThis.Controllers;
using RefactorThis.Validators;
using Shouldly;
using TestStack.BDDfy;

namespace RefactorThis.Tests.Validators
{
    [TestFixture]
    public class CreateOrUpdateProductValidatorTests
    {
        private CreateOrUpdateProductRequest _requestPayload;
        
        private ICreateOrUpdateProductRequestValidator _subject;
        
        private Exception _actualException;

        [SetUp]
        public void SetUp()
        {
            _subject = new CreateOrUpdateProductRequestValidator();
        }

        [Test]
        public void ItShouldNotThrowExceptionIfRequestIsValid()
        {
            this.Given(x => GivenAValidRequest())
                .When(x => WhenPerformValidation())
                .Then(x => ThenNoExceptionShouldBeThrown())
                .BDDfy();
        }

        [Test]
        public void ItShouldThrowExceptionIfProductNameIsMissing()
        {
            this.Given(x => GivenAnInvalidRequest())
                .When(x => WhenPerformValidation())
                .Then(x => ThenAValidationExceptionShouldBeThrownWithMessage("Product name must be provided"))
                .BDDfy();
        }

        private void GivenAValidRequest()
        {
            _requestPayload = new CreateOrUpdateProductRequest
            {
                Name = "Samsung S7",
                Description = ""
            };
        }
        
        private void GivenAnInvalidRequest()
        {
            _requestPayload = new CreateOrUpdateProductRequest
            {
                Description = ""
            };
        }

        private void WhenPerformValidation()
        {
            try
            {
                _subject.ValidateRequest(_requestPayload);
            }
            catch (ValidationException exception)
            {
                _actualException = exception;
            }
        }

        private void ThenNoExceptionShouldBeThrown()
        {
            _actualException.ShouldBe(null);
        }
        
        private void ThenAValidationExceptionShouldBeThrownWithMessage(string expectedExceptionMessage)
        {
            _actualException.Message.ShouldBe(expectedExceptionMessage);
        }
    }
}
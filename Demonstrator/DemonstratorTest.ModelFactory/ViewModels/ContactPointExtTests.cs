using Demonstrator.Models.ViewModels.Factories;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class ContactPointExtTests
    {
        [Fact]
        public void ContactPointViewModel_Returns_ValidViewModel_Full()
        {
            var model = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Use = ContactPoint.ContactPointUse.Work,
                Value = "EmailAddress"
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);
            Assert.Equal("Email", viewModel.System);
            Assert.Equal("Work", viewModel.Use);
            Assert.Equal("EmailAddress", viewModel.Value);
        }

        [Fact]
        public void ContactPointViewModel_Returns_ValidViewModel_NullUse()
        {
            var model = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Use = null,
                Value = "EmailAddress"
            };

            var viewModel = model.ToViewModel();

            Assert.Null(viewModel.Use);
        }

        [Fact]
        public void ContactPointViewModel_Null_ThrowsException()
        {
            ContactPoint model = null;

            Assert.Throws<NullReferenceException>(() => model.ToViewModel());
        }

    }
}

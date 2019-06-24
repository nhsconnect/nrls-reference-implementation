using Demonstrator.Models.ViewModels.Factories;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class CodeableConceptExtTests
    {
        [Fact]
        public void CodeableConceptViewModel_Returns_ValidViewModel_Full()
        {
            var model = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        Code = "Code1",
                        Display = "Display1",
                        System = "System1"
                    }
                }
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);
            Assert.NotNull(viewModel.Coding);
            Assert.Single(viewModel.Coding);
        }

        [Fact]
        public void CodeableConceptViewModel_Returns_ValidViewModel_Null()
        {
            var model = new CodeableConcept
            {
                Coding = null
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);
            Assert.Empty(viewModel.Coding);
        }

        [Fact]
        public void CodeableConceptViewModel_Returns_ValidViewModel_Empty()
        {
            var model = new CodeableConcept
            {
                Coding = new List<Coding>()
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);
            Assert.NotNull(viewModel.Coding);
            Assert.Empty(viewModel.Coding);
        }

        [Fact]
        public void CodeableConceptViewModel_Null_ThrowsException()
        {
            CodeableConcept model = null;
        
            Assert.Throws<NullReferenceException>(() => model.ToViewModel());
        }

        [Fact]
        public void CodingViewModel_Returns_ValidViewModel_Full()
        {
            var model = new Coding
            {
                Code = "Code1",
                Display = "Display1",
                System = "System1"
            };

            var viewModel = model.ToCodingViewModel();

            Assert.NotNull(viewModel);

            Assert.Equal("Code1", viewModel.Code);
            Assert.Equal("Display1", viewModel.Display);
            Assert.Equal("System1", viewModel.System);
        }

        [Fact]
        public void CodingViewModel_Returns_ValidViewModel_NUllDisplay()
        {
            var model = new Coding
            {
                Code = "Code1",
                Display = null,
                System = "System1"
            };

            var viewModel = model.ToCodingViewModel();

            Assert.Null(viewModel.Display);
        }

    }
}

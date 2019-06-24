using Demonstrator.Models.ViewModels.Factories;
using Demonstrator.Models.ViewModels.Fhir;
using DemonstratorTest.Data.FhirModels;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class DocumentReferenceExtTests
    {
        [Fact]
        public void PointerViewModel_Returns_ValidViewModel_Full()
        {
            var model = DocumentReferences.ValidFull_DocumentReference;

            var viewModel = model.ToViewModel("baseUrl", "pointerUrl");

            Assert.NotNull(viewModel);

            //Content - separate test
            Assert.Single(viewModel.Content);
            Assert.IsType<ContentViewModel>(viewModel.Content.First());

            Assert.Null(viewModel.Created);

            //Custodian - separate test for just reference
            Assert.NotNull(viewModel.Custodian);
            Assert.IsType<ReferenceViewModel>(viewModel.Custodian);

            Assert.Equal("abc", viewModel.Id);

            Assert.Equal("Current", viewModel.Status);

            //Identifier - separate test for just identifiers
            Assert.NotNull(viewModel.Identifier);
            Assert.NotEmpty(viewModel.Identifier);
            Assert.IsType<IdentifierViewModel>(viewModel.Identifier.First());

            //Subject - separate test for just reference
            Assert.NotNull(viewModel.Subject);
            Assert.IsType<ReferenceViewModel>(viewModel.Subject);

            //Type - separate test for codeableconcept
            Assert.NotNull(viewModel.Type);
            Assert.NotEmpty(viewModel.Type.Coding);
            Assert.IsType<CodingViewModel>(viewModel.Type.Coding.First());

            //Author - separate test for just reference
            Assert.NotNull(viewModel.Author);
            Assert.IsType<ReferenceViewModel>(viewModel.Author);

            Assert.NotNull(viewModel.Indexed);
            Assert.Equal(new DateTimeOffset(new DateTime(2019, 01, 01, 12, 33, 14), new TimeSpan(0, 0, 0)), viewModel.Indexed);

        }

        [Fact]
        public void PointerViewModel_Returns_InvalidViewModel_NullReferenceList()
        {
            var model = DocumentReferences.InvalidFull_DocumentReference_NullReferenceList;

            var viewModel = model.ToViewModel("baseUrl", "pointerUrl");

            Assert.NotNull(viewModel);
            
            //Author
            Assert.Null(viewModel.Author);

        }

        [Fact]
        public void PointerViewModel_Returns_InvalidViewModel_NullReference()
        {
            var model = DocumentReferences.InvalidFull_DocumentReference_NullReference;

            //Custodian
            Assert.Throws<NullReferenceException>(() => model.ToViewModel("baseUrl", "pointerUrl"));

        }

        [Fact]
        public void PointerViewModel_Returns_InvalidViewModel_NullCodeableConcept()
        {
            var model = DocumentReferences.InvalidFull_DocumentReference_NullCodeableConcept;

            //Type
            Assert.Throws<NullReferenceException>(() => model.ToViewModel("baseUrl", "pointerUrl"));

        }

        [Fact]
        public void PointerViewModel_Returns_InvalidViewModel_NullContent()
        {
            var model = DocumentReferences.InvalidFull_DocumentReference_NullContent;

            var viewModel = model.ToViewModel("baseUrl", "pointerUrl");

            //Type
            Assert.NotNull(viewModel.Content);
            Assert.Empty(viewModel.Content);
        }

        [Fact]
        public void ContentViewModel_Returns_ValidViewModel_FullContent()
        {
            var model = DocumentReferences.ValidFull_DocumentReference;

            var viewModel = model.ToViewModel("baseUrl", "pointerUrl");

            Assert.NotNull(viewModel);

            //Content - separate test
            Assert.Single(viewModel.Content);

            var contentViewModel = viewModel.Content.First();

            Assert.IsType<ContentViewModel>(contentViewModel);

            Assert.NotNull(contentViewModel.Attachment);
            Assert.Equal("application/pdf", contentViewModel.Attachment.ContentType);
            Assert.Equal(new DateTimeOffset(new DateTime(2019, 01, 01, 10, 13, 53), new TimeSpan(0, 0, 0)), contentViewModel.Attachment.Creation);
            Assert.Equal("My Attachement", contentViewModel.Attachment.Title);
            Assert.Equal("pointerUrl/link-to-attachment", contentViewModel.Attachment.Url);
        }

    }
}

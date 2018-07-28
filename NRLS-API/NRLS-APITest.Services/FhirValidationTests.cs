using Hl7.Fhir.Model;
using Moq;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Services;
using NRLS_APITest.Comparer;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NRLS_APITest.Services
{
    public class FhirValidationTests : IDisposable
    {

        private IValidationHelper _iValidationHelper;

        public FhirValidationTests()
        {
            var mock = new Mock<IValidationHelper>();
            mock.Setup(op => op.ValidReference(It.IsAny<ResourceReference>(), It.IsAny<string>())).Returns(true);
            mock.Setup(op => op.ValidReferenceParameter(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mock.Setup(op => op.ValidCodableConcept(It.Is<CodeableConcept>(q => q.Coding != null && q.Coding.FirstOrDefault() != null && q.Coding.FirstOrDefault().Code == null), It.IsAny<string>(), false, true, true, true, It.IsAny<string>())).Returns(false);
            mock.Setup(op => op.ValidCodableConcept(It.Is<CodeableConcept>(q => q.Coding != null && q.Coding.FirstOrDefault() != null && q.Coding.FirstOrDefault().Code != null), It.IsAny<string>(), false, true, true, true, It.IsAny<string>())).Returns(true);
            mock.Setup(op => op.ValidCodableConcept(It.Is<CodeableConcept>(q => q.Coding != null && q.Coding.FirstOrDefault() != null && q.Coding.FirstOrDefault().Code != null), It.IsAny<string>(), true, true, true, true, It.IsAny<string>())).Returns(true);

            mock.Setup(op => op.ValidNhsNumber(It.Is<string>(q => string.IsNullOrEmpty(q)))).Returns(false);
            mock.Setup(op => op.ValidNhsNumber(It.Is<string>(q => !string.IsNullOrEmpty(q)))).Returns(true);

            mock.Setup(op => op.ValidTokenParameter(It.Is<string>(q => q == "valid|valid"), null, false)).Returns(true);
            mock.Setup(op => op.ValidTokenParameter(It.Is<string>(q => q == "invalid|invalid"), null, false)).Returns(false);

            _iValidationHelper = mock.Object;
        }

        public void Dispose()
        {
            _iValidationHelper = null;
        }

        //[Fact]
        //public void ValidProfile_Valid()
        //{
        //    //add test
        //}

        //this is semi integration...
        [Fact]
        public void ValidPointer_Valid()
        {

            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Valid;

            var validPoiner = validationService.ValidPointer(pointer);

            var expected = OperationOutcomes.Ok;

            Assert.Equal(expected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_Status()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Status;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_Type()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Type;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Missing_Type()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Missing_Type;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_Subject()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Subject;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Missing_Subject()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Missing_Subject;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_Indexed()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Indexed;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_Author()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Author;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Missing_Author()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Missing_Author;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_TooMany_Author()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.TooMany_Author;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_Custodian()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Custodian;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Missing_Custodian()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Missing_Custodian;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Missing_Content()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Missing_Content;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }


        //[Fact]
        //public void ValidatePatientReference_Valid()
        //{
        //    //add test
        //}

        //[Fact]
        //public void ValidatePatientParameter_Valid()
        //{
        //    //add test
        //}

        [Fact]
        public void ValidateCustodianParameter_Valid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var reference = "https://directory.spineservices.nhs.uk/STU3/Organization/test";
            var validPoiner = validationService.ValidateCustodianParameter(reference);

            Assert.Null(validPoiner);
        }

        [Fact]
        public void ValidateCustodianParameter_Invalid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var reference = "https://directory.spineservices.nhs.uk/STU3/Organization/";
            var validPoiner = validationService.ValidateCustodianParameter(reference);

            Assert.NotNull(validPoiner);
        }

        //[Fact]
        //public void ValidateOrganisationReference_Valid()
        //{
        //    //add test
        //}

        [Fact]
        public void ValidateContent_Valid()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var content = FhirResources.Valid_Content;
            var validPoiner = validationService.ValidateContent(content);

            Assert.Null(validPoiner);
        }

        [Fact]
        public void ValidateContent_Invalid_Empty()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var emptyContent = new List<DocumentReference.ContentComponent>();

            var validPoiner = validationService.ValidateContent(emptyContent);

            Assert.IsType<OperationOutcome>(validPoiner);
        }

        [Fact]
        public void ValidateContent_Missing_Attachment()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var content = FhirResources.Missing_Attachment;

            var validPoiner = validationService.ValidateContent(content);

            Assert.IsType<OperationOutcome>(validPoiner);
        }

        [Fact]
        public void ValidateContent_Invalid_Creation()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var content = FhirResources.Invalid_Creation;

            var validPoiner = validationService.ValidateContent(content);

            Assert.IsType<OperationOutcome>(validPoiner);
        }


        [Fact]
        public void ValidateContent_Invalid_Url()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var content = FhirResources.Invalid_Url;

            var validPoiner = validationService.ValidateContent(content);

            Assert.IsType<OperationOutcome>(validPoiner);
        }

        [Fact]
        public void ValidateContent_Missing_Url()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var content = FhirResources.Missing_Url;

            var validPoiner = validationService.ValidateContent(content);

            Assert.IsType<OperationOutcome>(validPoiner);
        }

        [Fact]
        public void ValidateContent_Invalid_ContentType()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var content = FhirResources.Invalid_ContentType;

            var validPoiner = validationService.ValidateContent(content);

            Assert.IsType<OperationOutcome>(validPoiner);
        }

        [Fact]
        public void ValidateIdentifierParameter_Valid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var validParam = validationService.ValidateIdentifierParameter("identifier", "valid|valid");

            Assert.Null(validParam);
        }

        [Fact]
        public void ValidateIdentifierParameter_Invalid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var validParam = validationService.ValidateIdentifierParameter("identifier", "invalid|invalid");

            Assert.IsType<OperationOutcome>(validParam);
        }
    }
}

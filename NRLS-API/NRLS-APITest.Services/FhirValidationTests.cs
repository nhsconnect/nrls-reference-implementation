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
            mock.Setup(op => op.ValidateResource(It.IsAny<Resource>(), It.IsAny<string>())).Returns(OperationOutcomes.Ok);
            //mock.Setup(op => op.ValidReference(It.Is<ResourceReference>(r => r.Reference == "https://directory.spineservices.nhs.uk/STU3/Organization/"), It.IsAny<string>())).Returns(false);
            mock.Setup(op => op.ValidReference(It.IsAny<ResourceReference>(), It.IsAny<string>())).Returns(true);
            mock.Setup(op => op.ValidReferenceParameter(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mock.Setup(op => op.ValidCodableConcept(It.Is<CodeableConcept>(q => q.Coding != null && q.Coding.FirstOrDefault() != null && q.Coding.FirstOrDefault().Code == null), It.IsAny<int>(), It.IsAny<string>(), false, true, true, true, It.IsAny<string>())).Returns(false);
            mock.Setup(op => op.ValidCodableConcept(It.Is<CodeableConcept>(q => q.Coding != null && q.Coding.FirstOrDefault() != null && q.Coding.FirstOrDefault().Code != null), It.IsAny<int>(), It.IsAny<string>(), false, true, true, true, It.IsAny<string>())).Returns(true);
            mock.Setup(op => op.ValidCodableConcept(It.Is<CodeableConcept>(q => q.Coding != null && q.Coding.FirstOrDefault() != null && q.Coding.FirstOrDefault().Code != null), It.IsAny<int>(), It.IsAny<string>(), true, true, true, true, It.IsAny<string>())).Returns(true);

            mock.Setup(op => op.ValidNhsNumber(It.Is<string>(q => string.IsNullOrEmpty(q)))).Returns(false);
            mock.Setup(op => op.ValidNhsNumber(It.Is<string>(q => !string.IsNullOrEmpty(q)))).Returns(true);

            mock.Setup(op => op.ValidTokenParameter(It.Is<string>(q => q == "valid|valid"), null, false)).Returns(true);
            mock.Setup(op => op.ValidTokenParameter(It.Is<string>(q => q == "invalid|invalid"), null, false)).Returns(false);
            mock.Setup(op => op.ValidTokenParameter(It.Is<string>(q => q == "https://fhir.nhs.uk/Id/ods-organization-code|test"), It.Is<string>(q => q == "https://fhir.nhs.uk/Id/ods-organization-code"), false)).Returns(true);

            mock.Setup(op => op.ValidIdentifier(It.IsAny<Identifier>(), It.IsAny<string>())).Returns((true, null));
            mock.Setup(op => op.ValidIdentifier(It.Is<Identifier>(i => string.IsNullOrEmpty(i.System)), It.IsAny<string>())).Returns((false, "test"));
            mock.Setup(op => op.ValidIdentifier(It.Is<Identifier>(i => string.IsNullOrEmpty(i.Value)), It.IsAny<string>())).Returns((false, "test"));

            mock.Setup(op => op.GetResourceReferenceId(It.IsAny<ResourceReference>(), It.IsAny<string>())).Returns("resourceRefId");
            mock.Setup(op => op.GetResourceReferenceId(It.Is<ResourceReference>(r => r.Reference == "InvalidAuthorhttps://directory.spineservices.nhs.uk/STU3/Organization/"), It.IsAny<string>())).Returns(delegate { return null; });
            mock.Setup(op => op.GetResourceReferenceId(It.Is<ResourceReference>(r => r.Reference == "InvalidCustodianhttps://directory.spineservices.nhs.uk/STU3/Organization/"), It.IsAny<string>())).Returns(delegate { return null; });

            mock.Setup(op => op.GetOrganisationParameterIdentifierId(It.Is<string>(q => q == "https://fhir.nhs.uk/Id/ods-organization-code|test"))).Returns("test");

            mock.Setup(op => op.GetOrganisationParameterId(It.Is<string>(q => q == "https://directory.spineservices.nhs.uk/STU3/Organization/test"))).Returns("test");

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

            Assert.IsType<OperationOutcome>(validPoiner);
            Assert.True(validPoiner.Success);
        }

        [Fact]
        public void ValidPointer_Valid_with_MasterId()
        {

            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Valid_With_MasterId;

            var validPoiner = validationService.ValidPointer(pointer);

            Assert.IsType<OperationOutcome>(validPoiner);
            Assert.True(validPoiner.Success);
        }

        [Fact]
        public void ValidPointer_Invalid_MasterId_System()
        {

            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_MasterId_System;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_MasterId_Value()
        {

            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_MasterId_Value;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
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
        public void ValidPointer_Invalid_Subject()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Subject;

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
        public void ValidPointer_Invalid_Custodian()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Invalid_Custodian;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_RelatesTo_System()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Missing_RelatesTo_System;

            var validPoiner = validationService.ValidPointer(pointer);

            var notexpected = OperationOutcomes.Ok;

            Assert.NotEqual(notexpected, validPoiner, Comparers.ModelComparer<OperationOutcome>());
        }

        [Fact]
        public void ValidPointer_Invalid_RelatesTo_Value()
        {
            var validationService = new FhirValidation(_iValidationHelper);
            var pointer = NrlsPointers.Missing_RelatesTo_Value;

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

        [Fact]
        public void ValidateCustodianIdentifierParameter_Valid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var reference = "https://fhir.nhs.uk/Id/ods-organization-code|test";
            var validPoiner = validationService.ValidateCustodianIdentifierParameter(reference);

            Assert.Null(validPoiner);
        }

        [Fact]
        public void ValidateCustodianIdentifierParameter_Invalid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var reference = "https://fhir.nhs.uk/Id/ods-organization-code|";
            var validPoiner = validationService.ValidateCustodianIdentifierParameter(reference);

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

        [Fact]
        public void ValidateSummaryParameter_Valid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var validParam = validationService.ValidSummaryParameter("count");

            Assert.Null(validParam);
        }

        [Fact]
        public void ValidateSummaryParameter_Invalid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var validParam = validationService.ValidSummaryParameter("notcount");

            Assert.IsType<OperationOutcome>(validParam);
        }

        [Fact]
        public void GetValidRelatesTo_Valid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var relatesToList = new List<DocumentReference.RelatesToComponent>
            {
                FhirResources.Valid_Single_RelatesTo
            };

            var actual = validationService.GetValidRelatesTo(relatesToList);

            Assert.IsType<DocumentReference.RelatesToComponent>(actual.element);

            Assert.NotNull(actual.element.Target);
            Assert.NotNull(actual.element.Target.Identifier);

            Assert.Equal("urn:ietf:rfc:4151", actual.element.Target.Identifier.System);
            Assert.Equal("urn:tag:humber.nhs.uk,2004:cdc:600009612669", actual.element.Target.Identifier.Value);
        }

        [Fact]
        public void GetValidRelatesTo_Handles_Null()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var actual = validationService.GetValidRelatesTo(null);

            Assert.Null(actual.element);
        }

        [Fact]
        public void GetValidRelatesTo_Invalid_BadTarget()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var relatesToList = new List<DocumentReference.RelatesToComponent>
            {
                FhirResources.Invalid_Single_RelatesTo_BadTarget
            };

            var actual = validationService.GetValidRelatesTo(relatesToList);

            Assert.Null(actual.element);

        }

        [Fact]
        public void GetValidRelatesTo_Invalid_BadTargetValue()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var relatesToList = new List<DocumentReference.RelatesToComponent>
            {
                FhirResources.Invalid_Single_RelatesTo_BadTargetValue
            };

            var actual = validationService.GetValidRelatesTo(relatesToList);

            Assert.Null(actual.element);

        }

        [Fact]
        public void GetValidRelatesTo_Invalid_BadTargetSystem()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var relatesToList = new List<DocumentReference.RelatesToComponent>
            {
                FhirResources.Invalid_Single_RelatesTo_BadTargetSystem
            };

            var actual = validationService.GetValidRelatesTo(relatesToList);

            Assert.Null(actual.element);

        }

        [Fact]
        public void GetValidStatus_Valid()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var actual = validationService.GetValidStatus(DocumentReferenceStatus.Current);

            Assert.NotNull(actual);

            Assert.Equal(DocumentReferenceStatus.Current, actual);
        }

        [Fact]
        public void GetValidStatus_Handles_Null()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var actual = validationService.GetValidStatus(null);

            Assert.Null(actual);

        }

        [Fact]
        public void GetValidStatus_Handles_EnteredInError()
        {
            var validationService = new FhirValidation(_iValidationHelper);

            var actual = validationService.GetValidStatus(DocumentReferenceStatus.EnteredInError);

            Assert.Null(actual);

        }
    }
}

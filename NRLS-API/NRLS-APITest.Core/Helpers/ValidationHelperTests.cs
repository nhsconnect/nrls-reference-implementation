using Hl7.Fhir.Model;
using Moq;
using NRLS_API.Core.Helpers;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Resources;
using NRLS_APITest.Data;
using NRLS_APITest.StubClasses;
using System;
using System.Collections.Generic;
using Xunit;

namespace NRLS_APITest.Core.Helpers
{
    public class ValidationHelperTests : IDisposable
    {
        private IFhirCacheHelper _fhirCacheHelper;

        public ValidationHelperTests()
        {
            var mockCacheHelper = new Mock<IFhirCacheHelper>();
            mockCacheHelper.Setup(op => op.GetSource()).Returns(ResourceResolverStub.MockResourceResolver.GetSource());
            mockCacheHelper.Setup(op => op.GetValueSet(It.Is<string>(s => s.Equals(FhirConstants.VsRecordType)))).Returns(FhirResources.ValueSet_NrlsType);

            _fhirCacheHelper = mockCacheHelper.Object;
        }

        public void Dispose()
        {
            _fhirCacheHelper = null;
        }

        [Fact]
        public void Validation_ValidCodableConcept_InvalidNoConcept()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(null, 1, null, false, true, false, false, null);

            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_InvalidNoCoding()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>()
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, null, false, true, false, false, null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_InvalidTooManyCoding()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "TestSystem",
                        Code = "TestCode",
                        Display = "TestDisplay"
                    },
                    new Coding
                    {
                        System = "TestSystem_2",
                        Code = "TestCode_2",
                        Display = "TestDisplay_2"
                    }
                }
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, null, false, true, false, false, null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_ValidWithSystem()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "TestSystem",
                        Code = "TestCode",
                        Display = "TestDisplay"
                    }
                }
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, "TestSystem", false, true, false, false, null);


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_InvalidWithoutSystem()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = null,
                        Code = "TestCode",
                        Display = "TestDisplay"
                    }
                }
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, null, false, true, false, false, null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_ValidWithCode()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "TestSystem",
                        Code = "TestCode",
                        Display = "TestDisplay"
                    }
                }
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, null, false, false, true, false, null);


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_InvalidWithoutCode()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "TestSystem",
                        Code = null,
                        Display = "TestDisplay"
                    }
                }
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, null, false, false, true, false, null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_ValidWithDisplay()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "TestSystem",
                        Code = "TestCode",
                        Display = "TestDisplay"
                    }
                }
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, null, false, false, false, true, null);


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_InvalidWithoutDisplay()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding
                    {
                        System = "TestSystem",
                        Code = "TestCode",
                        Display = null
                    }
                }
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidCodableConcept(concept, 1, null, false, false, false, true, null);


            Assert.False(actual);
        }


        [Fact]
        public void Validation_GetResourceReferenceId_HandlesNull()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetResourceReferenceId(null, null);


            Assert.Null(actual);
        }

        [Fact]
        public void Validation_GetResourceReferenceId_ValidId()
        {
            var reference = new ResourceReference
            {
                Reference = "testsystem/id"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetResourceReferenceId(reference, "testsystem/");


            Assert.Equal("id", actual);
        }

        [Fact]
        public void Validation_GetResourceReferenceId_ValidIdWrongSystem()
        {
            var reference = new ResourceReference
            {
                Reference = "testsystem/id"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetResourceReferenceId(reference, "testsystem_blaa/");


            Assert.Equal("testsystem/id", actual);
        }

        [Fact]
        public void Validation_GetResourceReferenceId_ValidIdNoSystem()
        {
            var reference = new ResourceReference
            {
                Reference = "id"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetResourceReferenceId(reference, "testsystem_blaa/");


            Assert.Equal("id", actual);
        }

        [Fact]
        public void Validation_GetResourceReferenceId_ValidIdNullSystem()
        {
            var reference = new ResourceReference
            {
                Reference = "testsystem/id"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetResourceReferenceId(reference, null);


            Assert.Equal("testsystem/id", actual);
        }

        [Fact]
        public void Validation_ValidIdentifier_Valid()
        {
            var identifier = new Identifier
            {
                System = "testsystem.com",
                Value = "testValue"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidIdentifier(identifier, "masterIdentifier");


            Assert.True(actual.valid);
        }

        [Fact]
        public void Validation_ValidIdentifier_HandlesNull()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidIdentifier(null, "masterIdentifier");


            Assert.False(actual.valid);
        }

        [Fact]
        public void Validation_ValidIdentifier_Invalid_System()
        {
            var identifier = new Identifier
            {
                System = null,
                Value = "testValue"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidIdentifier(identifier, "masterIdentifier");

            Assert.False(actual.valid);
        }

        [Fact]
        public void Validation_ValidIdentifier_Invalid_Value()
        {
            var identifier = new Identifier
            {
                System = "testsystem.com",
                Value = null
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidIdentifier(identifier, "masterIdentifier");

            Assert.False(actual.valid);
        }

        [Fact]
        public void Validation_ValidReference_Valid()
        {
            var reference = new ResourceReference
            {
                Reference = "testsystem/id"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidReference(reference, "testsystem");


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidReference_Invalid()
        {
            var reference = new ResourceReference
            {
                Reference = "blaa_testsystem/id"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidReference(reference, "testsystem");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidReference_HandlesNull()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidReference(null, null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidReference_HandlesEmptyString()
        {
            var reference = new ResourceReference
            {
                Reference = "testsystem/id"
            };

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidReference(reference, "");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidNhsNumber("2686033207");


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_InvalidTooShort()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidNhsNumber("268603320");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_InvalidNumbers()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidNhsNumber("2686033201");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_InvalidNotNumbers()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidNhsNumber("268a6_0301");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_HandlesNull()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidNhsNumber(null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_HandlesEmptyString()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidNhsNumber("");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidReferenceParameter_HandlesEmptyParameterVal()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidReferenceParameter(null, "test");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidReferenceParameter_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidReferenceParameter("testprefix/testparam", "testprefix");


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidReferenceParameter_Invalid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidReferenceParameter("otherprefix/testparam", "testprefix");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_OptionalVal_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("tokenVal", null);


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_OptionalExpSys_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("sysVal|", "sysVal");


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_OptionalEmpty_Invalid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("", null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_OptionalNull_Invalid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter(null, null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_OptionalExpSys_Invalid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("sysVal", "AltSysVal");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_OptionalValPipe_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("|tokenVal", null);


            Assert.True(actual);
        }


        [Fact]
        public void Validation_ValidTokenParameter_OptionalSysPipe_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("sysVal|", null);


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_SysVal_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("sysVal|tokenVal", null, false);


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_SysValExp_Valid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("sysVal|tokenVal", "sysVal", false);


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_SysVal_Invalid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("|tokenVal", null, false);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidTokenParameter_SysValExp_Invalid()
        {

            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.ValidTokenParameter("sysVal|tokenVal", "AltSysVal", false);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterIdentifierId_Valid()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterIdentifierId($"{FhirConstants.SystemOrgCode}|testorgCode");

            Assert.Equal("testorgCode", actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterIdentifierId_Invalid_BadUri()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterIdentifierId($"{FhirConstants.SystemOrgCode}testorgCode");

            Assert.NotEqual("testorgCode", actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterIdentifierId_InvalidNoUrl()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterIdentifierId($"testorgCode");

            Assert.NotEqual("testorgCode", actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterIdentifierId_ValidNoCode()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterIdentifierId($"{FhirConstants.SystemOrgCode}|");

            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterId_Valid()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterId($"{FhirConstants.SystemODS}testorgCode");

            Assert.Equal("testorgCode", actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterId_Invalid_BadUri()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterId($"blassurltestorgCode");

            Assert.NotEqual("testorgCode", actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterId_InvalidNoUrl()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterId($"testorgCode");

            Assert.NotEqual("testorgCode", actual);
        }

        [Fact]
        public void Validation_GetOrganisationParameterId_ValidNoCode()
        {
            var helper = new ValidationHelper(_fhirCacheHelper);

            var actual = helper.GetOrganisationParameterId($"{FhirConstants.SystemODS}");

            Assert.Equal(string.Empty, actual);
        }

    }
}

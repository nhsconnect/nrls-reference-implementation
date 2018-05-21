using Hl7.Fhir.Model;
using NRLS_API.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NRLS_APITest.Core.Helpers
{
    public class ValidationHelperTests
    {
        [Fact]
        public void Validation_ValidCodableConcept_InvalidNoConcept()
        {
            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(null, null, false, true, false, false);

            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidCodableConcept_InvalidNoCoding()
        {
            var concept = new CodeableConcept
            {
                Coding = new List<Coding>()
            };

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, true, false, false);


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

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, true, false, false);


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

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, true, false, false);


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

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, true, false, false);


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

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, false, true, false);


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

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, false, true, false);


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

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, false, false, true);


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

            var helper = new ValidationHelper();

            var actual = helper.ValidCodableConcept(concept, null, false, false, false, true);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_GetResourceReferenceId_HandlesNull()
        {
            var helper = new ValidationHelper();

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

            var helper = new ValidationHelper();

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

            var helper = new ValidationHelper();

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

            var helper = new ValidationHelper();

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

            var helper = new ValidationHelper();

            var actual = helper.GetResourceReferenceId(reference, null);


            Assert.Equal("testsystem/id", actual);
        }

        [Fact]
        public void Validation_ValidReference_Valid()
        {
            var reference = new ResourceReference
            {
                Reference = "testsystem/id"
            };

            var helper = new ValidationHelper();

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

            var helper = new ValidationHelper();

            var actual = helper.ValidReference(reference, "testsystem");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidReference_HandlesNull()
        {
            var helper = new ValidationHelper();

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

            var helper = new ValidationHelper();

            var actual = helper.ValidReference(reference, "");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_Valid()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidNhsNumber("2686033207");


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_InvalidTooShort()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidNhsNumber("268603320");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_InvalidNumbers()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidNhsNumber("2686033201");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_InvalidNotNumbers()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidNhsNumber("268a6_0301");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_HandlesNull()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidNhsNumber(null);


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidNhsNumber_HandlesEmptyString()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidNhsNumber("");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidReferenceParameter_HandlesEmptyParameterVal()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidReferenceParameter(null, "test");


            Assert.False(actual);
        }

        [Fact]
        public void Validation_ValidReferenceParameter_Valid()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidReferenceParameter("testprefix/testparam", "testprefix");


            Assert.True(actual);
        }

        [Fact]
        public void Validation_ValidReferenceParameter_Invalid()
        {

            var helper = new ValidationHelper();

            var actual = helper.ValidReferenceParameter("otherprefix/testparam", "testprefix");


            Assert.False(actual);
        }

    }
}

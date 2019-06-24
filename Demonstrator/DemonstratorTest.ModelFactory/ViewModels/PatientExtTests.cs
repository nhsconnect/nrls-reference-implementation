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
    public class PatientExtTests
    {
        [Fact]
        public void PatientViewModel_Returns_ValidViewModel_Full()
        {
            var model = new Patient
            {
                Id = "abc",
                Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Family = "Jones",
                        Given = new List<string> { "Mark", "Andrew" },
                        Use = HumanName.NameUse.Official,
                        Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
                    }
                },
                Active = true,
                Gender = AdministrativeGender.Female,
                BirthDate = "2019/01/01T15:23:45",
                Address = new List<Address>
                {
                    new Address
                    {
                        City = "City",
                        District = "District",
                        Line = new List<string> { "Line1", "Line2" },
                        PostalCode = "PostalCode"
                    }
                },
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        System = "pas",
                        Value = "001"
                    }
                },
                ManagingOrganization = new ResourceReference
                {
                    Reference = "org-ref/org1"
                },
                Telecom = new List<ContactPoint>
                {
                    new ContactPoint
                    {
                        System = ContactPoint.ContactPointSystem.Email,
                        Use = null,
                        Value = "EmailAddress"
                    }
                }
            };

            var viewModel = model.ToViewModel("system");

            Assert.NotNull(viewModel);

            Assert.Equal("abc", viewModel.Id);

            //Name - seperate name tests
            Assert.NotNull(viewModel.Name);
            Assert.Single(viewModel.Name);
            Assert.IsType<NameViewModel>(viewModel.Name.First());

            Assert.True(viewModel.Active);

            Assert.Equal("Female", viewModel.Gender);

            Assert.NotNull(viewModel.BirthDate);
            Assert.Equal(new DateTime(2019, 01, 01, 00, 00, 00), viewModel.BirthDate);

            //Address - seperate address tests
            Assert.NotNull(viewModel.Address);
            Assert.Single(viewModel.Address);
            Assert.IsType<AddressViewModel>(viewModel.Address.First());

            //Identifier - seperate identifier tests
            Assert.NotNull(viewModel.Identifier);
            Assert.Single(viewModel.Identifier);
            Assert.IsType<IdentifierViewModel>(viewModel.Identifier.First());

            //ManagingOrganization - seperate reference tests
            Assert.NotNull(viewModel.ManagingOrganization);
            Assert.IsType<ReferenceViewModel>(viewModel.ManagingOrganization);

            //Telecom - seperate contact point tests
            Assert.NotNull(viewModel.Telecom);
            Assert.IsType<ContactPointViewModel>(viewModel.Telecom);

        }

        [Fact]
        public void PatientViewModel_Returns_InvalidViewModel_Null()
        {
            Patient model = null;

            Assert.Throws<NullReferenceException>(() => model.ToViewModel("system"));
        }

    }
}

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
    public class OrganizationExtTests
    {
        [Fact]
        public void OrganizationViewModel_Returns_ValidViewModel_Full()
        {
            var model = new Organization
            {
                Id = "abc",
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
                Name = "OrgName",
                Telecom = new List<ContactPoint>
                {
                    new ContactPoint
                    {
                        System = ContactPoint.ContactPointSystem.Email,
                        Use = null,
                        Value = "EmailAddress"
                    }
                },
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        System = "system",
                        Value = "org1"
                    }
                }
            };

            var viewModel = model.ToViewModel("system");

            Assert.NotNull(viewModel);

            Assert.Equal("abc", viewModel.Id);


            //Address - seperate address tests
            Assert.NotNull(viewModel.Address);
            Assert.Single(viewModel.Address);
            Assert.IsType<AddressViewModel>(viewModel.Address.First());

            Assert.Equal("OrgName", viewModel.Name);

            //Telecom - seperate contact point tests
            Assert.NotNull(viewModel.Telecom);
            Assert.Single(viewModel.Telecom);
            Assert.IsType<ContactPointViewModel>(viewModel.Telecom.First());


            //Identifier - seperate identifier tests
            Assert.NotNull(viewModel.Identifier);
            Assert.Single(viewModel.Identifier);
            Assert.IsType<IdentifierViewModel>(viewModel.Identifier.First());
        }

        [Fact]
        public void OrganizationViewModel_Returns_ValidViewModel_NullTelecom()
        {
            var model = new Organization
            {
                Id = "abc",
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
                Name = "OrgName",
                Telecom = null,
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        System = "system",
                        Value = "org1"
                    }
                }
            };

            var viewModel = model.ToViewModel("system");

            Assert.NotNull(viewModel);

            Assert.Empty(viewModel.Telecom);

        }

        [Fact]
        public void OrganizationViewModel_Returns_InvalidViewModel_Null()
        {
            Organization model = null;

            Assert.Throws<NullReferenceException>(() => model.ToViewModel("system"));
        }

    }
}

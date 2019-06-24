using Demonstrator.Models.ViewModels.Factories;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class AddressExtTests
    {
        [Fact]
        public void AddressViewModel_Returns_ValidViewModel_Full()
        {
            var model = new Address
            {
                City = "City",
                District = "District",
                Line = new List<string> { "Line1", "Line2" },
                PostalCode = "PostalCode",
                Type = Address.AddressType.Postal,
                Use = Address.AddressUse.Home,
                Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();

            Assert.NotNull(viewModel);
            Assert.Equal("City", viewModel.City);
            Assert.Equal("District", viewModel.District);
            Assert.Collection(viewModel.Line, item => Assert.Equal("Line1", item),
                                              item => Assert.Equal("Line2", item));
            Assert.Equal("PostalCode", viewModel.PostalCode);
            Assert.Equal("Postal", viewModel.Type);
            Assert.Equal("Home", viewModel.Use);

            Assert.Equal(new DateTime(2019, 01, 01), viewModel.Period.Start);
            Assert.Equal(new DateTime(2019, 12, 31), viewModel.Period.End);
        }

        [Fact]
        public void AddressViewModel_Returns_InvalidViewModel_NoLines()
        {
            var model = new Address
            {
                City = "City",
                District = "District",
                Line = null,
                PostalCode = "PostalCode",
                Type = Address.AddressType.Postal,
                Use = Address.AddressUse.Home,
                Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();

            Assert.Empty(viewModel.Line);

        }

        [Fact]
        public void AddressViewModel_Returns_InvalidViewModel_NoType()
        {
            var model = new Address
            {
                City = "City",
                District = "District",
                Line = null,
                PostalCode = "PostalCode",
                Type = null,
                Use = Address.AddressUse.Home,
                Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();

            Assert.Null(viewModel.Type);

        }

        [Fact]
        public void AddressViewModel_Returns_InvalidViewModel_NoUse()
        {
            var model = new Address
            {
                City = "City",
                District = "District",
                Line = null,
                PostalCode = "PostalCode",
                Type = null,
                Use = null,
                Period = new Period(new FhirDateTime(2019, 01, 01), new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();

            Assert.Null(viewModel.Use);

        }

        [Fact]
        public void AddressViewModel_Returns_InvalidViewModel_NoPeriodStart()
        {
            var model = new Address
            {
                City = "City",
                District = "District",
                Line = null,
                PostalCode = "PostalCode",
                Type = null,
                Use = Address.AddressUse.Home,
                Period = new Period(null, new FhirDateTime(2019, 12, 31))
            };

            var viewModel = model.ToViewModel();

            Assert.Null(viewModel.Period.Start);

        }

        [Fact]
        public void AddressViewModel_Returns_InvalidViewModel_NoPeriodEnd()
        {
            var model = new Address
            {
                City = "City",
                District = "District",
                Line = null,
                PostalCode = "PostalCode",
                Type = null,
                Use = Address.AddressUse.Home,
                Period = new Period(null, null)
            };

            var viewModel = model.ToViewModel();

            Assert.Null(viewModel.Period.End);

        }

        [Fact]
        public void AddressViewModel_Returns_InvalidViewModel_NoPeriod()
        {
            var model = new Address
            {
                City = "City",
                District = "District",
                Line = null,
                PostalCode = "PostalCode",
                Type = null,
                Use = Address.AddressUse.Home,
                Period = null
            };

            var viewModel = model.ToViewModel();

            Assert.Null(viewModel.Period);

        }
    }
}

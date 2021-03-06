using Demonstrator.Models.Core.Models;
using Demonstrator.Models.DataModels.Flows;
using Demonstrator.Models.ViewModels.Flows;
using DemonstratorTest.Comparer;
using DemonstratorTest.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class PersonnelTests
    {
        [Fact]
        public void Personnel_Returns_ValidViewModel()
        {
            var models = MongoPersonnel.Personnel;

            var viewModel = models.Select(Personnel.ToViewModel).First();

            var expectedViewModel = new PersonnelViewModel
            {
                Id = "5a8417f68317338c8e080a62",
                Name = "999 Call Handler",
                ImageUrl = "....",
                Context = new List<ContentView>()
                {
                    new ContentView
                    {
                        Title = "Title Text",
                        Content = new List<string>{"Content Text" },
                        CssClass = "CssClass Text",
                        Order = 1
                    }
                },
                UsesNrls = true,
                ActorOrganisationId = "5a82f9ffcb969daa58d33377",
                CModule = "CModule-Type",
                SystemIds = new List<string> { "5a8417338317338c8e0809e5" },
                Benefits = new List<string> { "benefitid" }
            };

            Assert.Equal(expectedViewModel, viewModel, Comparers.ModelComparer<PersonnelViewModel>());
        }

        [Fact]
        public void Personnel_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Personnel.ToViewModel(null));
        }
    }
}

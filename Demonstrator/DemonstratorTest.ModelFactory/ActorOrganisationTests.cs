using Demonstrator.Models.Core.Enums;
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
    public class ActorOrganisationTests
    {
        [Fact]
        public void ActorOrganisation_Returns_ValidViewModel()
        {
            var orgs = MongoActorOrganisations.ActorOrganisations;

            var orgViewModel = orgs.Select(ActorOrganisation.ToViewModel).First();

            var expectedViewModel = new ActorOrganisationViewModel
            {
                Id = "5a82f9ffcb969daa58d33377",
                Type = ActorType.Consumer,
                Name = "Ambulance Service",
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
                OrgCode = "AMSR01",
                Benefits = new List<string> { "benefitid" },
                PersonnelLinkId = "testlink"
            };

            Assert.Equal(expectedViewModel, orgViewModel, Comparers.ModelComparer<ActorOrganisationViewModel>());
        }

        [Fact]
        public void ActorOrganisation_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => ActorOrganisation.ToViewModel(null));
        }
    }
}

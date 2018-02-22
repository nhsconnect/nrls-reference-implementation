using Demonstrator.Models.Core.Enums;
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
    public class GenericSystemTests
    {
        [Fact]
        public void GenericSystem_Returns_ValidViewModel()
        {
            var models = MongoGenericSystems.GenericSystems;

            var viewModel = models.Select(GenericSystem.ToViewModel).First();

            var expectedViewModel = new GenericSystemViewModel
            {
                Id = "5a8417338317338c8e0809e5",
                Name = "Ambulance Service Call Handler",
                FModule = "Ambulance_Service_Call_Handler",
                Asid = "200000000115",
                Context = "Some context...",
                ActionTypes = new List<ActorType> { ActorType.Consumer }
            };

            Assert.Equal(expectedViewModel, viewModel, Comparers.ModelComparer<GenericSystemViewModel>());
        }

        [Fact]
        public void GenericSystem_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => GenericSystem.ToViewModel(null));
        }
    }
}

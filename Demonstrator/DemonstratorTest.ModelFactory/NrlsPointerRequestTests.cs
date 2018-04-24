using Demonstrator.Models.Nrls;
using DemonstratorTest.Comparer;
using Xunit;

namespace DemonstratorTest.ModelFactory
{
    public class NrlsPointerRequestTests
    {
        [Fact]
        public void NrlsPointerRequest_Create_Valid()
        {

            var viewModel = NrlsPointerRequest.Create("Test", "0001", "https://test.com/test/pdf", "test/test", "0001", "test", "0001", "test");

            var expectedViewModel = new NrlsPointerRequest
            {
                Asid = "0001",
                Interaction = "test",
                NhsNumber = "0001",
                OrgCode = "Test",
                PointerId = null,
                RecordContentType = "test/test",
                RecordUrl = "https://test.com/test/pdf",
                TypeCode = "0001",
                TypeDisplay = "test"
            };

            Assert.Equal(expectedViewModel, viewModel, Comparers.ModelComparer<NrlsPointerRequest>());
        }

        [Fact]
        public void NrlsPointerRequest_Delete_Valid()
        {

            var viewModel = NrlsPointerRequest.Delete("id001", "0001", "Test");

            var expectedViewModel = new NrlsPointerRequest
            {
                Asid = "0001",
                Interaction = "Test",
                PointerId = "id001"
            };

            Assert.Equal(expectedViewModel, viewModel, Comparers.ModelComparer<NrlsPointerRequest>());
        }

        [Fact]
        public void NrlsPointerRequest_Read_Valid()
        {

            var viewModel = NrlsPointerRequest.Read("id001", "0001", "Test");

            var expectedViewModel = new NrlsPointerRequest
            {
                Asid = "0001",
                Interaction = "Test",
                PointerId = "id001"
            };

            Assert.Equal(expectedViewModel, viewModel, Comparers.ModelComparer<NrlsPointerRequest>());
        }

        [Fact]
        public void NrlsPointerRequest_Search_Valid()
        {

            var viewModel = NrlsPointerRequest.Search("Test", "0001", "0001", "Test");

            var expectedViewModel = new NrlsPointerRequest
            {
                Asid = "0001",
                Interaction = "Test",
                OrgCode = "Test",
                NhsNumber = "0001"
            };

            Assert.Equal(expectedViewModel, viewModel, Comparers.ModelComparer<NrlsPointerRequest>());
        }

    }
}

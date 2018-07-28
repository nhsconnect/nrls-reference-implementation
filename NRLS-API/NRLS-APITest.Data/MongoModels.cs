using NRLS_APITest.TestModels;

namespace NRLS_APITest.Data
{
    public static class MongoModels
    {
        public static MockDeleteResult Valid_Delete
        {
            get
            {
                return new MockDeleteResult(1, true);
            }
        }

        public static MockDeleteResult Invalid_Delete
        {
            get
            {
                return new MockDeleteResult(0, false);
            }
        }
    }
}

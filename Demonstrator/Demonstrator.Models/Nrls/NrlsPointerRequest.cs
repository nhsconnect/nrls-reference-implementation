namespace Demonstrator.Models.Nrls
{
    public class NrlsPointerRequest
    {
        public string PointerId { get; set; }

        //This can be for search or create (both custodian and author)
        public string OrgCode { get; set; }

        public string NhsNumber { get; set; }

        public string RecordUrl { get; set; }

        public string RecordContentType { get; set; }

        public string TypeCode { get; set; }

        public string TypeDisplay { get; set; }

        public string Asid { get; set; }

        public static NrlsPointerRequest Read(string pointerId, string asid)
        {
            return Generate(pointerId, null, null, null, null, null, null, asid);
        }

        public static NrlsPointerRequest Search(string orgCode, string nhsNumber, string typeCode, string asid)
        {
            return Generate(null, orgCode, nhsNumber, null, null, typeCode, null, asid);
        }

        public static NrlsPointerRequest Create(string orgCode, string nhsNumber, string recordUrl, string recordContentType, string typeCode, string typeDisplay, string asid)
        {
            return Generate(null, orgCode, nhsNumber, recordUrl, recordContentType, typeCode, typeDisplay, asid);
        }

        public static NrlsPointerRequest Delete(string pointerId, string asid)
        {
            return Generate(pointerId, null, null, null, null, null, null, asid);
        }

        private static NrlsPointerRequest Generate(string pointerId, string orgCode, string nhsNumber, string recordUrl, string recordContentType, string typeCode, string typeDisplay, string asid)
        {
            return new NrlsPointerRequest
            {
                Asid = asid,
                NhsNumber = nhsNumber,
                OrgCode = orgCode,
                PointerId = pointerId,
                RecordContentType = recordContentType,
                RecordUrl = recordUrl,
                TypeCode = typeCode,
                TypeDisplay = typeDisplay
            };
        }
    }
}

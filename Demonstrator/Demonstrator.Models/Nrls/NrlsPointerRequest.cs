namespace Demonstrator.Models.Nrls
{
    public class NrlsPointerRequest
    {
        public string PointerId { get; set; }

        public string JwtOrgCode { get; set; }

        public string CustodianOrgCode { get; set; }

        public string NhsNumber { get; set; }

        public string RecordUrl { get; set; }

        public string RecordContentType { get; set; }

        public string TypeCode { get; set; }

        public string TypeDisplay { get; set; }

        public string Asid { get; set; }

        public static NrlsPointerRequest Read(string pointerId, string asid, string jwtOrgCode)
        {
            return Generate(pointerId, jwtOrgCode, null, null, null, null, null, asid, null);
        }

        public static NrlsPointerRequest Search(string jwtOrgCode, string nhsNumber, string typeCode, string asid, string custodianOrgCode)
        {
            return Generate(null, jwtOrgCode, nhsNumber, null, null, typeCode, null, asid, custodianOrgCode);
        }

        public static NrlsPointerRequest Create(string jwtOrgCode, string custodianOrgCode, string nhsNumber, string recordUrl, string recordContentType, string typeCode, string typeDisplay, string asid)
        {
            return Generate(null, jwtOrgCode, nhsNumber, recordUrl, recordContentType, typeCode, typeDisplay, asid, custodianOrgCode);
        }

        public static NrlsPointerRequest Delete(string pointerId, string asid, string jwtOrgCode)
        {
            return Generate(pointerId, jwtOrgCode, null, null, null, null, null, asid, null);
        }

        private static NrlsPointerRequest Generate(string pointerId, string jwtOrgCode, string nhsNumber, string recordUrl, string recordContentType, string typeCode, string typeDisplay, string asid, string custodianOrgCode)
        {
            return new NrlsPointerRequest
            {
                Asid = asid,
                NhsNumber = nhsNumber,
                JwtOrgCode = jwtOrgCode,
                PointerId = pointerId,
                RecordContentType = recordContentType,
                RecordUrl = recordUrl,
                TypeCode = typeCode,
                TypeDisplay = typeDisplay,
                CustodianOrgCode = custodianOrgCode
            };
        }
    }
}

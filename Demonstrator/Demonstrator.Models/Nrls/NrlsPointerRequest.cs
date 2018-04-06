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

        public string Interaction { get; set; }

        public static NrlsPointerRequest Read(string pointerId, string asid, string interaction)
        {
            return Generate(pointerId, null, null, null, null, null, null, asid, interaction);
        }

        public static NrlsPointerRequest Search(string orgCode, string nhsNumber, string asid, string interaction)
        {
            return Generate(null, orgCode, nhsNumber, null, null, null, null, asid, interaction);
        }

        public static NrlsPointerRequest Create(string orgCode, string nhsNumber, string recordUrl, string recordContentType, string typeCode, string typeDisplay, string asid, string interaction)
        {
            return Generate(null, orgCode, nhsNumber, recordUrl, recordContentType, typeCode, typeDisplay, asid, interaction);
        }

        public static NrlsPointerRequest Delete(string pointerId, string asid, string interaction)
        {
            return Generate(pointerId, null, null, null, null, null, null, asid, interaction);
        }

        private static NrlsPointerRequest Generate(string pointerId, string orgCode, string nhsNumber, string recordUrl, string recordContentType, string typeCode, string typeDisplay, string asid, string interaction)
        {
            return new NrlsPointerRequest
            {
                Asid = asid,
                Interaction = interaction,
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

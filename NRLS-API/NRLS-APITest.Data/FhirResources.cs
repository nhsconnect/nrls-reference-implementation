using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_APITest.Data
{
    public class FhirResources
    {
        public static List<DocumentReference.ContentComponent> Valid_Content
        {
            get
            {
                return new List<DocumentReference.ContentComponent>()
                {
                    new DocumentReference.ContentComponent
                    {
                        Attachment = new Attachment
                        {
                            ContentType = "application/pdf",
                            Url = "http://example.org/xds/mhd/Binary/07a6483f-732b-461e-86b6-edb665c45510.pdf",
                            Title = "Mental health Care Plan Report",
                            Creation = "2016-03-08T15:26:00+00:00"
                        }
                    }
                };
            }
        }

        public static List<DocumentReference.ContentComponent> Missing_Attachment
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment = null;

                return content;
            }
        }

        public static List<DocumentReference.ContentComponent> Invalid_Creation
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.Creation = "invalid date";

                return content;
            }
        }

        public static List<DocumentReference.ContentComponent> Missing_Creation
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.Creation = null;

                return content;
            }
        }

        public static List<DocumentReference.ContentComponent> Invalid_Url
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.Url = "http://.";

                return content;
            }
        }

        public static List<DocumentReference.ContentComponent> Missing_Url
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.Url = null;

                return content;
            }
        }

        public static List<DocumentReference.ContentComponent> Invalid_ContentType
        {
            get
            {
                var content = Valid_Content;
                content.First().Attachment.ContentType = null;

                return content;
            }
        }
    }
}

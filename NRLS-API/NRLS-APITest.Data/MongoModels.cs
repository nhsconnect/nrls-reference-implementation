using MongoDB.Bson;
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

        public static MockDUpdateResult Valid_Update
        {
            get
            {
                return new MockDUpdateResult(1, true);
            }
        }

        public static MockDUpdateResult Invalid_Update
        {
            get
            {
                return new MockDUpdateResult(0, false);
            }
        }

        public static BsonDocument BsonDocumentReferenceA
        {
            get
            {
                var json = "{\"_id\":{\"$oid\":\"5ab13f4a957d0ad5d93a133a\"},\"resourceType\":\"DocumentReference\",\"meta\":{\"versionId\":\"1\",\"profile\":[\"https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1\"]},\"status\":\"current\",\"type\":{\"coding\":[{\"system\":\"http://snomed.info/sct\",\"code\":\"736252007\",\"display\":\"Cancer care plan (record artifact)\"}]},\"subject\":{\"reference\":\"https://demographics.spineservices.nhs.uk/STU3/Patient/1020103620\"},\"indexed\":\"2011-07-19T11:27:16+00:00\",\"author\":[{\"reference\":\"https://directory.spineservices.nhs.uk/STU3/Organization/1XR\"}],\"custodian\":{\"reference\":\"https://directory.spineservices.nhs.uk/STU3/Organization/1XR\"},\"content\":[{\"attachment\":{\"contentType\":\"text/html\",\"url\":\"http://localhost:55448/provider/1XR/fhir/STU3/careconnect/binary/5ab13f4a957d0ad5d93a133a\",\"title\":\"Cancer Care Plan\",\"creation\":\"2011-07-19T11:27:16+00:00\"}}]}";
                return BsonDocument.Parse(json);
            }
        }

        public static BsonDocument BsonDocumentReferenceB
        {
            get
            {
                var json = "{\"_id\":{\"$oid\":\"5ab13f41957d0ad5d93a1339\"},\"resourceType\":\"DocumentReference\",\"meta\":{\"versionId\":\"1\",\"profile\":[\"https://fhir.nhs.uk/STU3/StructureDefinition/NRLS-DocumentReference-1\"]},\"status\":\"current\",\"type\":{\"coding\":[{\"system\":\"http://snomed.info/sct\",\"code\":\"736253002\",\"display\":\"Mental health crisis plan (record artifact)\"}]},\"subject\":{\"reference\":\"https://demographics.spineservices.nhs.uk/STU3/Patient/1020103620\"},\"indexed\":\"2005-12-24T09:43:41+00:00\",\"author\":[{\"reference\":\"https://directory.spineservices.nhs.uk/STU3/Organization/MHT01\"}],\"custodian\":{\"reference\":\"https://directory.spineservices.nhs.uk/STU3/Organization/MHT01\"},\"content\":[{\"attachment\":{\"contentType\":\"application/pdf\",\"url\":\"http://localhost:55448/provider/1XR/fhir/STU3/careconnect/binary/5ab13f41957d0ad5d93a1339\",\"title\":\"Mental Health Crisis Plan\",\"creation\":\"2016-03-08T15:26:00+00:00\"}}]}";

                return BsonDocument.Parse(json);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;

namespace DemonstratorTest.Data
{
    public class HttpContexts
    {
        public static HttpContext Valid_Search
        {
            get
            {
                var context = new DefaultHttpContext();
                context.Request.Scheme = "http";
                context.Request.Host = new HostString("www.testhost.com");
                context.Request.Path = new PathString("/testpath");
                context.Request.Method = HttpMethods.Get;
                context.Request.Headers["fromASID"] = "000";
                context.Request.Headers["toASID"] = "toASID";

                return context;
            }
        }

        public static HttpContext Valid_DocumentFhir
        {
            get
            {
                var context = new DefaultHttpContext();
                context.Request.Scheme = "http";
                context.Request.Host = new HostString("www.testhost.com");
                context.Request.Path = new PathString("/testpath");
                context.Request.Method = HttpMethods.Get;
                context.Request.Headers["fromASID"] = "000";
                context.Request.Headers["toASID"] = "toASID";
                context.Request.Headers["Accept"] = "application/xml+fhir";

                return context;
            }
        }

        public static HttpContext Valid_CustomHeaders
        {
            get
            {
                var context = new DefaultHttpContext();
                context.Request.Scheme = "http";
                context.Request.Host = new HostString("www.testhost.com");
                context.Request.Path = new PathString("/testpath");
                context.Request.Method = HttpMethods.Get;
                context.Request.Headers["Asid"] = "000";
                context.Request.Headers["OrgCode"] = "org001";

                return context;
            }
        }

        //public static HttpContext NotFound_Search
        //{
        //    get
        //    {
        //        var context = Valid_Search;
        //        context.Request.Headers["fromASID"] = "notfound";

        //        return context;
        //    }
        //}

        //public static HttpContext Valid_Create_Pointer
        //{
        //    get
        //    {
        //        var context = new DefaultHttpContext();
        //        context.Request.Scheme = "http";
        //        context.Request.Host = new HostString("www.testhost.com");
        //        context.Request.Path = new PathString("/testpath");
        //        context.Request.Method = HttpMethods.Post;
        //        context.Request.Headers["fromASID"] = "fromASID";
        //        context.Request.Headers["toASID"] = "toASID";

        //        return context;
        //    }
        //}

        //public static HttpContext Valid_Delete_Pointer
        //{
        //    get
        //    {
        //        var context = new DefaultHttpContext();
        //        context.Request.Scheme = "http";
        //        context.Request.Host = new HostString("www.testhost.com");
        //        context.Request.Path = new PathString("/testpath");
        //        context.Request.Method = HttpMethods.Delete;
        //        context.Request.Headers["fromASID"] = "fromASID";
        //        context.Request.Headers["toASID"] = "toASID";
        //        context.Request.QueryString = QueryString.Create(new List<KeyValuePair<string, string>> {
        //            new KeyValuePair<string, string>("_id", "testId")
        //        });

        //        return context;
        //    }
        //}

        //public static HttpContext Valid_ConditionalDelete
        //{
        //    get
        //    {
        //        var context = new DefaultHttpContext();
        //        context.Request.Scheme = "http";
        //        context.Request.Host = new HostString("www.testhost.com");
        //        context.Request.Path = new PathString("/testpath");
        //        context.Request.Method = HttpMethods.Delete;
        //        context.Request.Headers["fromASID"] = "fromASID";
        //        context.Request.Headers["toASID"] = "toASID";
        //        context.Request.QueryString = QueryString.Create(new List<KeyValuePair<string, string>> {
        //            new KeyValuePair<string, string>("identifier", "testSystem|testValue"),
        //            new KeyValuePair<string, string>("subject", "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207")
        //        });

        //        return context;
        //    }
        //}

        //public static HttpContext Invalid_Delete_Pointer_BadRequest
        //{
        //    get
        //    {
        //        var context = new DefaultHttpContext();
        //        context.Request.Scheme = "http";
        //        context.Request.Host = new HostString("www.testhost.com");
        //        context.Request.Path = new PathString("/testpath");
        //        context.Request.Method = HttpMethods.Delete;
        //        context.Request.Headers["fromASID"] = "badrequest";
        //        context.Request.Headers["toASID"] = "toASID";
        //        context.Request.QueryString = QueryString.Create(new List<KeyValuePair<string, string>> {
        //            new KeyValuePair<string, string>("_id", "testId")
        //        });

        //        return context;
        //    }
        //}

        //public static HttpContext Invalid_Delete_Pointer_NotFound
        //{
        //    get
        //    {
        //        var context = new DefaultHttpContext();
        //        context.Request.Scheme = "http";
        //        context.Request.Host = new HostString("www.testhost.com");
        //        context.Request.Path = new PathString("/testpath");
        //        context.Request.Method = HttpMethods.Delete;
        //        context.Request.Headers["fromASID"] = "notfound";
        //        context.Request.Headers["toASID"] = "toASID";
        //        context.Request.QueryString = QueryString.Create(new List<KeyValuePair<string, string>> {
        //            new KeyValuePair<string, string>("_id", "testId")
        //        });

        //        return context;
        //    }
        //}

        //public static HttpContext Invalid_ConditionalDelete_NoSearchValues
        //{
        //    get
        //    {
        //        var context = new DefaultHttpContext();
        //        context.Request.Scheme = "http";
        //        context.Request.Host = new HostString("www.testhost.com");
        //        context.Request.Path = new PathString("/testpath");
        //        context.Request.Method = HttpMethods.Delete;
        //        context.Request.Headers["fromASID"] = "fromASID";
        //        context.Request.Headers["toASID"] = "toASID";
        //        context.Request.QueryString = QueryString.Create(new List<KeyValuePair<string, string>> {
        //            new KeyValuePair<string, string>("identifier", ""),
        //            new KeyValuePair<string, string>("subject", "")
        //        });

        //        return context;
        //    }
        //}
    }
}

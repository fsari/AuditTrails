using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace AuditTrails.Models
{
    public class AuditAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        { 
            var request = filterContext.HttpContext.Request;

            var sessionIdentifier = HttpContext.Current.Session.SessionID;
           
            var audit = new Audit()
            {
                UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous", 
                IpAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress, 
                UrlAccessed = request.RawUrl, 
                Timestamp = DateTime.UtcNow,
                SessionId =  sessionIdentifier,
                Data =  SerializeRequest(request)
            };
             
            var context = new ApplicationDbContext();
            context.AuditRecords.Add(audit);
            context.SaveChanges();
             
            base.OnActionExecuting(filterContext);
        }

        public int AuditingLevel { get; set; }
        
        private string SerializeRequest(HttpRequestBase request)
        {
            switch (AuditingLevel)
            {
                //No Request Data will be serialized
                case 0:
                default:
                    return "";
                //Basic Request Serialization - just stores Data
                case 1:
                    return Json.Encode(new { request.Cookies, request.Headers, request.Files });
                //Middle Level - Customize to your Preferences
                case 2:
                    return Json.Encode(new { request.Cookies, request.Headers, request.Files, request.Form, request.QueryString, request.Params });
                //Highest Level - Serialize the entire Request object
                case 3:
                    //We can't simply just Encode the entire request string due to circular references as well
                    //as objects that cannot "simply" be serialized such as Streams, References etc.
                    //return Json.Encode(request);
                    return Json.Encode(new { request.Cookies, request.Headers, request.Files, request.Form, request.QueryString, request.Params });
            }
        }
    }
}
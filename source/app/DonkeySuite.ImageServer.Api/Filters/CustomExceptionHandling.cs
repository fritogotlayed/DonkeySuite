using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using log4net;

namespace DonkeySuite.ImageServer.Api.Filters
{
    public class CustomExceptionHandling : ExceptionFilterAttribute
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext context)
        {
            /*
        if (context.Exception is BusinessException)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(context.Exception.Message),
                ReasonPhrase = "Exception"
            });

        }

        //Log Critical errors
        Debug.WriteLine(context.Exception);
         */
            Log.Error("An error has occurred while processing a request.", context.Exception);

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("An error occurred, please try again or contact the administrator."),
                ReasonPhrase = "Critical Exception"
            });
        }
    }
}
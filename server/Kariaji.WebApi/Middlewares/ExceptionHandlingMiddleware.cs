using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kariaji.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;

namespace Kariaji.WebApi.Middlewares
{
    public class KariajiException : Exception
    {
        public static KariajiException NotAuthorized =>
            KariajiException.NewPublic("Nincs jogosultságod ehhez a művelethez");
        public static KariajiException BadParamters =>
            KariajiException.NewPublic("Hibás kérés");

        public string LogMessage { get; set; }
        public string PublicMessage { get; set; }

        public static KariajiException NewPublic(string publicMessage, string logMessage = null) => new KariajiException
        {
            LogMessage = logMessage,
            PublicMessage = publicMessage
        };

        public static KariajiException NewLog(string logMessage, string  publicMessage= null) => new KariajiException
        {
            LogMessage = logMessage,
            PublicMessage = publicMessage
        };



    }

    public class ErrorHandlingMiddleware
    {
        private static ILogger _Logger;

        private static ILogger Logger => _Logger ?? (_Logger =
                                      NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger());

        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception is KariajiException)
            {
                var kariajiExc = exception as KariajiException;
                Logger.Error(kariajiExc, kariajiExc.LogMessage ?? kariajiExc.PublicMessage);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(JsonConvert.SerializeObject(
                    CommonResult.NewError(kariajiExc.PublicMessage), new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
            }


            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            //if      (exception is MyNotFoundException)     code = HttpStatusCode.NotFound;
            //else if (exception is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (exception is MyException)             code = HttpStatusCode.BadRequest;

            Logger.Error(exception, exception.Message);
            var result = JsonConvert.SerializeObject(CommonResult.NewError());
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);


        }
    }
}

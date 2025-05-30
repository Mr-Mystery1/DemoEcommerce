using System.Net;
using System.Text.Json;
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare Default variables 
            string message = "sorry,internal server error occurred. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";
            try
            {
                await next(context);
                //Check if Response Too many requests -429 status code
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // Check if Response is UnAuthorized -401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access.";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }
                // Check if Response is Forbidden -403 status code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out Of Access";
                    message = "You are not allowed/required to access.";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }







            }
            catch (Exception ex) 
            {
                //Log Original Exception /File, Debugger, Console
                LogException.LogExceptions(ex);

                //Check if Exception is TimeOut -408 request timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out Of Time.";
                    message = "Request Timeout... Try again..";
                    statusCode=(int)StatusCodes.Status408RequestTimeout;
                }

                //If none of the error then do the default...
                await ModifyHeader(context, title, message, statusCode);

            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
           context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {   
                Detail = message,
                Status = statusCode,
                Title = title,

            }),CancellationToken.None);
            return;
        }
    }
}

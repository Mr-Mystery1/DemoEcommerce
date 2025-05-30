using Microsoft.AspNetCore.Http;
namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenOnlyApiGateway(RequestDelegate next)
    {
       
        public async Task InvokeAsync(HttpContext context)
        {
            //Extract specific request headers
            var signedHeader = context.Request.Headers["Api-Gateway"];

            //Null means, the request is not coming from the API Gateway //Service Unavailable
            if (signedHeader.FirstOrDefault()==null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, Service Unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}

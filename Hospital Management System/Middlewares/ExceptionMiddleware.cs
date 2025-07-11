using Hospital_Management_System.Errors;
using System.Net;
using System.Text.Json;

namespace Hospital_Management_System.Middlewares
{
    public class ExceptionMiddleware
    {

     
            private readonly RequestDelegate next;
            private readonly ILogger<ExceptionMiddleware> logger;
            private readonly IHostEnvironment env;

            public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
            {
                this.next = next;
                this.logger = logger;
                this.env = env;
            }
            public async Task Invoke(HttpContext context)
            {
                try
                {
                    await next.Invoke(context);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    //for frontend
                    context.Response.ContentType = "Application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var response = env.IsDevelopment() ?
                        new ApiExceptionError((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                        :
                        new ApiExceptionError((int)HttpStatusCode.InternalServerError);
                    var json = JsonSerializer.Serialize(response);
                    await context.Response.WriteAsync(json);

                }
            }
        


    }
}

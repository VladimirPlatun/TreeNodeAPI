using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TreeNodesApi.Controllers;
using TreeNodesApi.Data;
using TreeNodesApi.Models;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        var requestBodyText = string.Empty;

        using (var newBodyStream = new MemoryStream())
        {
            context.Response.Body = newBodyStream;

            var originalRequestBodyStream = context.Request.Body;
            using (var requestBodyStream = new MemoryStream())
            {

                await context.Request.Body.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);

                requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
                requestBodyStream.Seek(0, SeekOrigin.Begin);

                context.Request.Body = requestBodyStream;
            }

            try
            {
                await _next(context);
            }
            catch (SecureException ex)
            {
                var eventId = Guid.NewGuid();
                await LogException(context, ex, "Secure", eventId, requestBodyText);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = new
                {
                    type = "Secure",
                    id = eventId,
                    data = new { message = ex.Message }
                };
                var responseJson = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(responseJson);
            }
            catch (Exception ex)
            {
                var eventId = Guid.NewGuid();
                await LogException(context, ex, "Exception", eventId, requestBodyText);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = new
                {
                    type = "Exception",
                    id = eventId,
                    data = new { message = $"Internal server error ID = {eventId}" }
                };
                var responseJson = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(responseJson);
            }
            finally
            {
                newBodyStream.Seek(0, SeekOrigin.Begin);
                await newBodyStream.CopyToAsync(originalBodyStream);

                context.Request.Body = originalRequestBodyStream;
            }
        }
    }

    private async Task LogException(HttpContext context, Exception ex, string exceptionType, Guid eventId, string requestBodyText)
    {
        // Debug 
        Console.WriteLine($"Request Body: {requestBodyText}");
        Console.WriteLine($"Query Params: {context.Request.QueryString.Value}");

        var exceptionLog = new ExceptionLog
        {
            EventId = eventId,
            Timestamp = DateTime.UtcNow,
            QueryParams = context.Request.QueryString.Value,
            BodyParams = requestBodyText,
            StackTrace = ex.StackTrace,
            ExceptionType = exceptionType,
            Message = ex.Message
        };

        var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
        dbContext.ExceptionLogs.Add(exceptionLog);
        await dbContext.SaveChangesAsync();
    }
}

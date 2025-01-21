using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using trabahuso_api.DTOs.Responses;

namespace trabahuso_api.Middlewares
{
    public class GlobalErrorHandler : IMiddleware
    {

        public GlobalErrorHandler()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new ErrorResponse()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An error occured while processing your request",
                Path = context.Request.Path
            };

            var result = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(result);
        }


    }
}
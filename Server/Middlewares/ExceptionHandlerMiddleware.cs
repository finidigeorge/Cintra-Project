﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shared.Dto;

namespace Server.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
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
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            
            if (exception is AuthenticationException)
                code = HttpStatusCode.Unauthorized;            

            var result = JsonConvert.SerializeObject(new ErrorMessageDto { ErrorMessage = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;            
            return context.Response.WriteAsync(result);
        }
        
    }
}

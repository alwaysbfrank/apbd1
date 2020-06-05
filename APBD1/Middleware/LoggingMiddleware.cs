using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using APBD1.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace APBD1.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var method = request.Method;
            var path = request.Path.ToString();

            string body;

            var log = $"{method} @ {path}";
            
            
            using (var streamReader = new StreamReader(request.Body))
            {
                body = await streamReader.ReadToEndAsync();
                
                if (!string.IsNullOrWhiteSpace(body))
                {
                    log += $", body {body}";
                }
                
                var injectedRequestStream = new MemoryStream();
                var bytesToWrite = Encoding.UTF8.GetBytes(body);
                injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                injectedRequestStream.Seek(0, SeekOrigin.Begin);
                request.Body = injectedRequestStream;
            }
            
            var query = request.QueryString.ToString();

            if (!string.IsNullOrWhiteSpace(query))
            {
                log += $", query {query}";
            }
            
            Console.WriteLine(log);
            await _next.Invoke(httpContext);
        }
    }
}
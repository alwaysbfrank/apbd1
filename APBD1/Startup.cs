using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD1.DAL;
using APBD1.Middleware;
using APBD1.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace APBD1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<InMemoryDbAccessor>();
            services.AddSingleton<IStudentsService, StudentsService>();
            services.AddSingleton<EnrollmentsService>();
            services.AddSingleton<StudiesService>();
            services.AddSingleton<FullStudentService>();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeParsing.Converter());
            });
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentsService studentsService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseMyMiddleware();
            
            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    await FailedValidation(context);
                }

                string index = context.Request.Headers["Index"].ToString();

                try
                {
                    studentsService.GetStudent(index);
                }
                catch
                {
                    await FailedValidation(context);
                }

                await next();
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }

        private async Task FailedValidation(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsync("Nie podałeś indeksu");
        }
    }
}
using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movies.Models;
using Movies.Repository;

namespace Movies
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CloudDbConnection")));
            }
            else
            {
                services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));
            }
            services.BuildServiceProvider().GetService<DataContext>().Database.Migrate();
            services.AddTransient<IDataRepository, DataRepository>();
            //services.AddMvc(options =>
            //{
            //    options.RespectBrowserAcceptHeader = true;
            //    options.ReturnHttpNotAcceptable = true;  // Return HTTP 406
            //});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    if (feature != null)
                    {
                        Exception exception = feature.Error;

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = exception.Message
                        }.ToString());
                    }
                });
            });
            app.UseMvc();
        }
    }
}

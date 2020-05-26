using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoFilterer.Swagger;
using MarkdownDocumenting.Elements;
using MarkdownDocumenting.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WebApplication.API.Models;
using WebApplication.API.Repository;

namespace WebApplication.API
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
            services.AddControllersWithViews()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSingleton<BooksRepository>();

            services.AddDocumentation();

            services.AddSwaggerGen(c =>
            {
                c.UseAutoFiltererParameters();

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sample book",
                    Version = "v1",
                });
                c.DescribeAllEnumsAsStrings();

                var docFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                string filePath = Path.Combine(AppContext.BaseDirectory, docFile);
                if (File.Exists(filePath))
                    c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseDocumentation(cfg =>
            {
                cfg.RootPathHandling = HandlingType.Redirect;
                cfg.GetMdlStyle = "https://code.getmdl.io/1.3.0/material.deep_purple-light_blue.min.css";
                var swashbuckleLink = new CustomLink("Swagger UI", "/swagger", false);
                cfg
                    .AddCustomLink(swashbuckleLink)
                    .AddFooterLink(swashbuckleLink)
                    .AddFooterLink(new CustomLink("Swagger.json", "/swagger/v1/swagger.json", true))
                    .AddFooterLink(new CustomLink("See on Gitub", "https://github.com/enisn/AutoFilterer", true));
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DefaultModelExpandDepth(3);
                options.EnableDeepLinking();
                options.DisplayRequestDuration();
                options.ShowExtensions();

                options.RoutePrefix = "swagger";

                options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");

            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

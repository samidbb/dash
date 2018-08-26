using System.Collections.Generic;
using Dash.Domain;
using Dash.Infrastructure;
using Dash.Infrastructure.Configuration;
using Dash.Infrastructure.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;

namespace Dash
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
            var settings = new Settings();

            Configuration.Bind("DASH", settings);

            services.AddSingleton(settings);

            services.AddTransient<FileSystem>();
            services.AddTransient<DashboardService>();
            services.AddTransient<FileVersionRepository>();
            services.AddTransient<DashboardConfigurationRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            app.UseSwaggerDocumentation(env);

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    public class Settings
    {
        public string Root { get; set; }
    }

    public static class SwaggerDocumentationExtensions
    {
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwaggerUi(typeof(Startup).Assembly, settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Description = "Seabook Terms and Conditions API V1";
                    document.Security.Add(new SwaggerSecurityRequirement {{"Bearer", new string[] { }}});
                    document.Schemes = new List<SwaggerSchema> {env.IsDevelopment() ? SwaggerSchema.Http : SwaggerSchema.Https};
                };

                settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;
                settings.GeneratorSettings.Title = "Seabook Terms and Conditions API Swagger documentation";

                settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("Bearer",
                    new SwaggerSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "X-Api-Key",
                        In = SwaggerSecurityApiKeyLocation.Header
                    }));
            });

            return app;
        }
    }
}
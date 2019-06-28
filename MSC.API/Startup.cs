using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSC.API.Context;
using MSC.API.Helpers;
using MSC.API.Repositories;
using MSC.API.Services;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace MSC.API
{
    public class Startup
    {

        #region Variables

        private readonly string ApiTitle = "Making Sense API";
        private string ApiVersion => !Configuration["VERSION_NUMBER"].IsNullOrEmpty() 
            ? $"v{Configuration["VERSION_NUMBER"]}" 
            : "v1";

        public IConfiguration Configuration { get; }
        private readonly ILoggerFactory _loggerFactory;

        #endregion

        public Startup(IHostingEnvironment env, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddEntityFramework(services);

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Unspecified;
                });

            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddScoped<IPostService, PostService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = ApiTitle,
                    Version = ApiVersion
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApiTitle} {ApiVersion}");

                c.DocExpansion(DocExpansion.None);
            });

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseMvc();
        }

        /// <summary>
        /// Configures EntityFrameworkCore.
        /// </summary>
        /// <param name="services">The collection of services to add.</param>
        public virtual void AddEntityFramework(IServiceCollection services)
        {
            var connection = Configuration["DEFAULT_CONNECTION"] ?? Configuration["Data:DefaultConnection:DefaultConnection"];
            services.AddDbContext<AppContext>(options => options.UseSqlServer(connection).ConfigureWarnings(warnings =>
            {
                //  "Some LINQ cannot be converted to SQL". We don't care. Ignore.
                warnings.Ignore(RelationalEventId.QueryClientEvaluationWarning);
            }));
        }
    }
}

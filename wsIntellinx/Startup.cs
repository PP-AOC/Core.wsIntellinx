using AOC.Data.AppSettings;
using Core.Api.Utils.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using wsIntellinx.BLL;

namespace wsIntellinx
{
    /// <summary>
    /// Standard startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="config"></param>
        public Startup(IConfiguration config)
        {
            var traceConverter = new Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config)
                                                   .WriteTo.ApplicationInsights(telemetryConverter: traceConverter,
                                                                               restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                                                  .Enrich.WithThreadId()
                                                  .CreateLogger();

            // Now logging can be done.
            Configuration = config;
            Log.Logger.Information("Exiting Startup ctor.");
        }

        /// <summary> /// IConfiguration property /// </summary>
        public IConfiguration Configuration { get; }

        #region ConfigureServices
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger.Information("Entering  Startup::ConfigureServices.");

            // Register Exception handler for Controllers
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    });

            // Binding the class AppSettings to the AppSettings tage in the file: appsettings.json
            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);

            // setup so that Dependency Injection can use AppSettings instead of IOptionsSnapshot<AppSettings>
            services.AddSingleton<AppSettings>(appSettings);

            // Application Insights
            services.AddApplicationInsightsTelemetry();

            //Adding CORS Policies           
            services.AddCors(options =>
            {
                //Policy used for Development
                options.AddPolicy("AllowAnyOrigin",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
                //Policy used for Test,QA, and Prod
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins(appSettings.AllowedOrigins)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            #region registerDependencyInjection
            // register dependency injection for the controller
            services.AddScoped<IIntellinxLogic, IntellinxLogic>();

            // registers IHttpContextAccessor for the Controller
            services.AddHttpContextAccessor();

            // register dependency injection
            // for the Service Layer
            services.AddScoped<IAppSettingsFactory, AppSettingsFactory>();

            // DI Logger
            services.AddSingleton(Log.Logger);
            services.AddScoped<Core.Api.Utils.Log.ILogger, Core.Api.Utils.Log.LoggerSeri>();
            #endregion

            #region Swagger
            // Add OpenAPI and Swagger DI services and configure documents

            // Register an OpenAPI 3.0 document generator
            services.AddSwaggerGen(document =>
            {
                document.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Intellinx",
                    Version = GetType().Assembly.GetName().Version.ToString()
                });

                document.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
                {
                    Description = "Api key needed to access the endpoints. ApiKey-v1 My_API_Key",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });

                document.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Authorization",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Authorization"
                            },
                         },
                         new string[] {}
                     }
                });

                document.UseInlineDefinitionsForEnums();
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                document.IncludeXmlComments(xmlPath);
                document.CustomSchemaIds(x => x.FullName);
            });
            #endregion

            // Log Configuration settings
            Log.Logger.Information("Exit Startup::ConfigureServices.");
        }
        #endregion

        #region Configure
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Hook Global Exception Handler 1st  -- will catch any issue in Middle-ware
            app.UseMiddleware<ExceptionHandler>();

            // Hook in LogRequest and LogResponse into the middleWare pipe.
            app.UseMiddleware<LogRequest>();
            app.UseMiddleware<LogResponse>();
            Log.Logger.Information("Entering Startup::Configure.");
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            //Configuring CORS policy for all controller endpoints based on environments
            if (env.IsDevelopment())
            {
                app.UseCors("AllowAnyOrigin");
            }
            else
            {
                app.UseCors("AllowSpecificOrigins");
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region swagger
            app.UseSwagger();
            // When the application is hosted as an IIS virtual application, the request path to swagger.json 
            // is missing the path of the virtual application. 
            // For example, https://example.com/myapi/swagger will make a request 
            // to https://example.com/swagger/v1/swagger.json
            // To work around this, the request can be intercepted to modify the path
            app.UseSwaggerUI(config =>
            {
                config.ConfigObject = new ConfigObject
                {
                    ShowCommonExtensions = true,
                    ShowExtensions = true,

                };

                config.SwaggerEndpoint("./swagger/v1/swagger.json", "wsIntellinx");
                config.DocumentTitle = string.Format("{0} - {1}",
                                       Assembly.GetExecutingAssembly().GetName().Name,
                                       Configuration.GetSection("AppSettings").GetSection("DeploymentEnvironment").Value);
                config.RoutePrefix = string.Empty;
            });
            #endregion

            // Log Configuration settings
            Log.Logger.Information("Exit Startup::Configure.");
        }
        #endregion
    }
}

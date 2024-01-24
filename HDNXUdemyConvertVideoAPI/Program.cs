using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Redis.StackExchange;
using HDNXUdemyConvertVideoAPI.Middlewares;
using HDNXUdemyConvertVideoAPI.ModelHelp;
using HDNXUdemyConvertVideoAPI.ProjectExtensisons;
using HDNXUdemyData.EntitiesContext;
using HDNXUdemyModel.Base;
using HDNXUdemyServices.IServices;
using HDNXUdemyServices.RecuringJob;
using HDNXUdemyServices.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using static HDNXUdemyConvertVideoAPI.ProjectExtensisons.ApplicationSwaggerExtension;

namespace HDNXUdemyConvertVideoAPI
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        public ILogServices<UploadFileVideoToServer> _logServices;

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.

            // Add services to the container.
            HelperConstantsModel.LoadConfig(configuration);
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
            builder.Services.AddDbContext<ProjectContext>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(xmlFilePath);
            });

            builder.Services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'VVV";
                config.SubstituteApiVersionInUrl = true;
            });
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });
            builder.Services.AddSwashbuckleSwagger();
            builder.Services.AddApplicationServicesExtension();
            builder.Services.CustomerApplicationJWTExtension();

            var redisConfig = GetSettings(configuration);
            // Add Hangfire services.
            builder.Services.AddHangfire(configuration =>
            {
                configuration.UseRedisStorage($"{redisConfig.Host}:{redisConfig.Port},abortConnect=false,ssl=false,password={redisConfig.Password}");
                // configuration.UseRedisStorage($"{redisConfig.Host}:{redisConfig.Port}");
            });

            builder.Services.AddHangfireServer(options =>
            {
                options.WorkerCount = 30;
            });

            builder.Services.AddCors(x =>
            {
                x.AddDefaultPolicy(polocy =>
                {
                    polocy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            builder.Services.AddDirectoryBrowser();
            builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            var app = builder.Build();

            var apiVersionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            // Configure the HTTP request pipeline.
            var env = app.Environment;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwashbuckleSwagger(apiVersionProvider);
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseExceptionMiddleware();
            app.UseCors();
            app.UseRouting().UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
            });

            app.UseAuthorization();

            var dashboardOptions = new DashboardOptions()
            {
                Authorization = new[] { new AllowAllConnectionsFilter(configuration) },
            };

            // Using Dashboard for hangfire
            // app.UseHangfireDashboard("/hangfirejobs", dashboardOptions);
            // RunRecuringJob.RunAutoRecuringJob(redisConfig);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath)),
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=600");
                },
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".m3u8"] = "application/x-mpegURL", [".ts"] = "video/mp2t" }
                }
            });
            app.UseDirectoryBrowser();
            app.Run();
        }

        private static RedisConnect GetSettings(IConfiguration configuration)
        {
            var jobSetting = configuration.GetSection(nameof(RedisConfig.RedisConnect)).Get<RedisConnect>();
            return jobSetting ?? new RedisConnect();
        }

        /// <summary>
        /// AllowAllConnectionsFilter
        /// </summary>
        public class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
        {
            /// <summary>
            /// AllowAllConnectionsFilter
            /// </summary>
            /// <param name="configuration"></param>
            public AllowAllConnectionsFilter(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            /// <summary>
            /// Configuration
            /// </summary>
            public IConfiguration Configuration { get; }

            /// <summary>
            /// Authorize
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public bool Authorize(DashboardContext context)
            {
                var jobSetting = Configuration.GetSection(nameof(RedisConfig.RedisConnect)).Get<RedisConnect>() ?? new RedisConnect();
                return jobSetting.IsEnableDashboard;
            }
        }
    }
}
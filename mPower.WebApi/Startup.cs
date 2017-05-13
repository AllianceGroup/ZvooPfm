using System;
using mPower.WebApi.Authorization;
using mPower.WebApi.Extensions;
using mPower.WebApi.Tenants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using StructureMap;

namespace mPower.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            #region CORS
            services.AddCors(options => options.AddPolicy("Default", builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials();
            }));
            #endregion

            services.AddTokenAuthentication();
            services.AddMemoryCache();
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Never", new CacheProfile
                {
                    Location = ResponseCacheLocation.None,
                    NoStore = true
                });
            }).AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            #region Multi tenancy
            var container = new Container();
            services.AddMultiTenancy(container);           
            container.Populate(services);
            #endregion

            services.AddScoped<IAuthorizationHandler, LedgerAuthorizationHandler>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IAuthorizationHandler, PermissionsAuthorizationHandler>();

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                app.UseApplicationInsightsRequestTelemetry();
                app.UseApplicationInsightsExceptionTelemetry();
            }

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseCors("Default");
            app.UseTokenAuthentication();
            app.UseStaticFiles();
            app.UseSignalR2();
            app.UseMvc();
        }
    }
}

using AutoMapper;
using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Logging;
using Microsoft.Net.Http.Headers;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using Wiz.Chapter4.API.Extensions;
using Wiz.Chapter4.API.Filters;
using Wiz.Chapter4.API.Middlewares;
using Wiz.Chapter4.API.Settings;
using Wiz.Chapter4.Domain.Interfaces.Identity;
using Wiz.Chapter4.Domain.Interfaces.Notifications;
using Wiz.Chapter4.Domain.Interfaces.UoW;
using Wiz.Chapter4.Domain.Notifications;
using Wiz.Chapter4.Infra.Context;
using Wiz.Chapter4.Infra.Identity;
using Wiz.Chapter4.Infra.UoW;

namespace Wiz.Chapter4.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;            
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc(options =>
            {
                options.Filters.Add<DomainNotificationFilter>();
                options.EnableEndpointRouting = false;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = Configuration["WizID:Authority"];
                options.Audience = Configuration["WizID:Audience"];
                options.RequireHttpsMetadata = false;

                if (PlatformServices.Default.Application.ApplicationName == "testhost")
                {
                    options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration();
                }

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        var jwtClaimScope = ctx.Principal.Claims.FirstOrDefault(x => x.Type == "scope")?.Value;

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.System, jwtClaimScope),
                            new Claim(ClaimTypes.Authentication, ((JwtSecurityToken)ctx.SecurityToken).RawData)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims);
                        ctx.Principal.AddIdentity(claimsIdentity);
                        ctx.Success();
                    }
                };
            });

            services.Configure<GzipCompressionProviderOptions>(x => x.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(x =>
            {
                x.Providers.Add<GzipCompressionProvider>();
            });

            this.RegisterHttpClient(services);

            if (PlatformServices.Default.Application.ApplicationName != "testhost")
            {
                var healthCheck = services.AddHealthChecksUI(setupSettings: setup =>
                {
                    setup.DisableDatabaseMigrations();
                    setup.MaximumHistoryEntriesPerEndpoint(6);
                    setup.AddWebhookNotification("Teams", Configuration["Webhook:Teams"],
                        payload: File.ReadAllText(Path.Combine(".", "MessageCard", "ServiceDown.json")),
                        restorePayload: File.ReadAllText(Path.Combine(".", "MessageCard", "ServiceRestore.json")),
                        customMessageFunc: report =>
                            {
                                var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                                return $"{AppDomain.CurrentDomain.FriendlyName}: {failing.Count()} healthchecks are failing";
                            }
                        );
                }).AddInMemoryStorage();

                var builder = healthCheck.Services.AddHealthChecks();

                //500 mb
                builder.AddProcessAllocatedMemoryHealthCheck(500 * 1024 * 1024, "Process Memory", tags: new[] { "self" });
                //500 mb
                builder.AddPrivateMemoryHealthCheck(1500 * 1024 * 1024, "Private memory", tags: new[] { "self" });

                builder.AddSqlServer(Configuration["ConnectionStrings:CustomerDB"], tags: new[] { "services" });

                //dotnet add <Project> package AspNetCore.HealthChecks.OpenIdConnectServer
                builder.AddIdentityServer(new Uri(Configuration["WizID:Authority"]), "SSO Wiz", tags: new[] { "services" });

                builder.AddApplicationInsightsPublisher();
            }

            if (!WebHostEnvironment.IsProduction())
            {
                services.AddOpenApiDocument(document =>
                {
                    document.DocumentName = "v1";
                    document.Version = "v1";
                    document.Title = "Chapter4 API";
                    document.Description = "API de Chapter4";
                    document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
                    document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = HeaderNames.Authorization,
                        Description = "Token de autenticação via SSO",
                        In = OpenApiSecurityApiKeyLocation.Header
                    });

                    document.PostProcess = (configure) =>
                    {
                        configure.Info.TermsOfService = "None";
                        configure.Info.Contact = new OpenApiContact()
                        {
                            Name = "Squad",
                            Email = "squad@xyz.com",
                            Url = "exemplo.xyz.com"
                        };
                        configure.Info.License = new OpenApiLicense()
                        {
                            Name = "Exemplo",
                            Url = "exemplo.xyz.com"
                        };
                    };


                });
            }

            services.AddAutoMapper(typeof(Startup));
            services.AddHttpContextAccessor();
            services.AddApplicationInsightsTelemetry();

            this.RegisterServices(services);
            this.RegisterDatabaseServices(services);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApplicationInsightsSettings> options)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseResponseCompression();

            if (!env.IsProduction())
            {
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseLogMiddleware();

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new ErrorHandlerMiddleware(options, env).Invoke
            });

            app.UseEndpoints(endpoints =>
            {
                if (PlatformServices.Default.Application.ApplicationName != "testhost")
                {
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions
                    {
                        Predicate = r => r.Tags.Contains("self"),
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                    endpoints.MapHealthChecks("/ready", new HealthCheckOptions
                    {
                        Predicate = r => r.Tags.Contains("services"),
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                    endpoints.MapHealthChecksUI(setup =>
                    {
                        setup.UIPath = "/health-ui";
                    });
                }

                endpoints.MapControllers();
            });
        }


        private void RegisterHttpClient(IServiceCollection services)
        {
        }

        protected virtual void RegisterServices(IServiceCollection services)
        {
            services.Configure<ApplicationInsightsSettings>(Configuration.GetSection("ApplicationInsights"));

            #region Service

            #endregion

            #region Domain

            services.AddScoped<IDomainNotification, DomainNotification>();

            #endregion

            #region Infra


            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IIdentityService, IdentityService>();

            #endregion
        }

        protected virtual void RegisterDatabaseServices(IServiceCollection services)
        {
            // if (PlatformServices.Default.Application.ApplicationName != "testhost")
            // {
                services.AddDbContext<EntityContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("CustomerDB")));
                services.AddSingleton<DbConnection>(conn => new SqlConnection(Configuration.GetConnectionString("CustomerDB")));
                services.AddScoped<DapperContext>();
            // }
        }
    }
}

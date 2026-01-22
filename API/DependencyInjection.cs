using Api.Extensions;
using API.Extensions;
using API.Extensions.HealthCheck;
using API.Extensions.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace API;
[ExcludeFromCodeCoverage]

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddAuthentication(configuration);
        services.AddAuthorization();
        services.AddSerilog();

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        services.AddControllersWithViews();

        //services.AddOptionsPattern(configuration);
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddTransient<UnauthorizedTokenMiddleware>();
        services.AddEndpoints(Assembly.GetExecutingAssembly());

        services.AddHttpContextAccessor();

        services.AddSwaggerGen(x =>
        {

            x.CustomSchemaIds(n => n.FullName);
            x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Tech Challenge - Alexandre Alencar RM364893", Version = "v1", Description = "API o projeto da Pós-Tech em Arquitetura de Software na FIAP, hamburg" });
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Autorização efetuada via JWT token (Digite 'Bearer {seu_token}' para autenticar).",
                Name = "Authorization",
                In = ParameterLocation.Header,
                BearerFormat = "JWT",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            x.AddSecurityDefinition("Bearer", securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
                        {
                            { securitySchema, new[] { "Bearer" } }
                        };

            x.AddSecurityRequirement(securityRequirement);
        });


        services.AddTransient<ApiHealthCheck>();

        return services;
    }

    public static IHealthChecksBuilder AddHealthApi(this IHealthChecksBuilder services)
    {
        services.AddCheck<ApiHealthCheck>("API")
            .AddPrivateMemoryHealthCheck(
                maximumMemoryBytes: 2_000_000_000, // Define o limite máximo de 2 GB de memória RAM
                name: "Uso de Memória",
                failureStatus: HealthStatus.Degraded); // Reporta como "Degraded" em vez de "Unhealthy"
        return services;                                                // Isso é útil para saber que há um problema, mas a API ainda funciona
    }

    public static void RegisterPipeline(this WebApplication app)
    {

        app.UseSerilogRequestLogging();
        app.UseExceptionHandler(o => { });
        app.UseMiddleware<UnauthorizedTokenMiddleware>();

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            app.AddApiDocumentation();

        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });

        app.UseAuthentication(); //1
        app.UseAuthorization(); //2
        app.UseAntiforgery();

        app.MapEndpoints();

        app.MapFallbackToFile("index.html");

    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(
            options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };
            });

        return services;
    }

    public static void AddApiDocumentation(this WebApplication app)
    {
        app.AddSwagger();
        app.AddScalar();
    }

    public static void AddSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public static void AddScalar(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Tech Challenge - Alexandre Alencar RM364893")
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", options =>
            {
                options.Token = "teste";
            });
        });
    }

    //public static IServiceCollection AddOptionsPattern(this IServiceCollection services, IConfiguration configuration)
    //{
    //    //services.Configure<MicroOption>(configuration.GetSection(MicroOption.BaseConfig));Para configurar microservicos

    //    return services;
    //}
}

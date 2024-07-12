using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using System.Text.Json.Serialization;
using static NOS.Engineering.Challenge.Database.MongoDbContext;

namespace NOS.Engineering.Challenge.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder webApplicationBuilder)
    {
        var serviceCollection = webApplicationBuilder.Services;
        var configuration = webApplicationBuilder.Configuration;

        serviceCollection.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = null;
        });
        serviceCollection.AddControllers();
        serviceCollection
            .AddEndpointsApiExplorer();
        
        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nos Challenge Api", Version = "v1" });
        });

        serviceCollection
            .RegisterSlowDatabase(configuration)
            .RegisterContentsManager();
        return webApplicationBuilder;
    }

    private static IServiceCollection RegisterSlowDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDatabase<Content, ContentDto>, SlowDatabase<Content, ContentDto>>();
        services.AddSingleton<IMapper<Content, ContentDto>, ContentMapper>();
        services.AddSingleton<IMockData<Content>, MockData>();
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<MongoDbContext>();
        //services.AddScoped<IContentsManager, MongoDbContentManager>();
        return services;
    }

    private static IServiceCollection RegisterContentsManager(this IServiceCollection services)
    {
        services.AddSingleton<IContentsManager, MongoDbContentManager>();

        return services;
    }

    public static WebApplicationBuilder ConfigureWebHost(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder
            .WebHost
            .ConfigureLogging(logging => { logging.ClearProviders(); });

        return webApplicationBuilder;
    }
}
using NOS.Engineering.Challenge.API.Extensions;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();
var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach(var desc in descriptions)
        {
            string url = $"/swagger/{desc.GroupName}/swagger.json";
            string name = desc.GroupName.ToLowerInvariant();

            options.SwaggerEndpoint(url, name);
        }
    });

app.Run();
using Infrastructure;
using WebApi.Configuration;
using WebApi.Configuration.ApiVersioning;
using WebApi.Configuration.Observability;
using WebApi.Configuration.Swagger;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddAppOptions(configuration);
builder.Services.AddInfrastructure(configuration);
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddObservability(configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
    {
        var apiVersionDescriptions = app.DescribeApiVersions();
        foreach (var apiVersionDescription in apiVersionDescriptions)
            options.SwaggerEndpoint($"/swagger/{apiVersionDescription.GroupName}/swagger.json", apiVersionDescription.GroupName.ToUpperInvariant());

    });

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseObservability();

app.Run();

public partial class Program { }
using Application.Validators.Currency;
using FluentValidation;
using Infrastructure;
using Infrastructure.Currency.Constants;
using WebApi.Configuration;
using WebApi.Configuration.ApiVersioning;
using WebApi.Configuration.HttpCommunication;
using WebApi.Configuration.Observability;
using WebApi.Configuration.Swagger;
using WebApi.Filters;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddAppOptions(configuration);
builder.Services.AddInfrastructure(configuration);
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddObservability(configuration);

builder.Services.AddValidatorsFromAssemblyContaining<HistoricalRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ConvertRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LatestRequestValidator>();

builder.Services.AddHttpCommunication
(
    CurrencyRateConstants.ClientNames.Api,
    configuration.GetValue<string>("CurrencyProviderConfiguration:Url")!
);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
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
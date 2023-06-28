using System.Reflection;

using Examples.Etag.WebApi;
using Examples.Etag.WebApi.Application.Services;
using Examples.Etag.WebApi.Application.Translators;
using Examples.Etag.WebApi.Common.Abstractions;
using Examples.Etag.WebApi.Domain.Abstractions;
using Examples.Etag.WebApi.Domain;
using Examples.Etag.WebApi.Infrastructure;

using Microsoft.Azure.Cosmos;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<CosmosOptions>(
	builder.Configuration.GetRequiredSection(CosmosOptions.Cosmos));

// Add services to the container.

builder.Services.AddControllers()
	.AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
	options =>
	{
		var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
	});

builder.Services
	.AddSingleton<ITranslator<AppointmentRequest, AppointmentRequestEntity>, AppointmentRequestEntityTranslator>();
builder.Services.AddSingleton(serviceProvider =>
{
	var o = serviceProvider.GetRequiredService<IOptions<CosmosOptions>>().Value;
	return new CosmosClient(o.EndpointUri, o.PrimaryKey, new CosmosClientOptions
	{
		ApplicationName = o.ApplicationName
	});
});
builder.Services.AddSingleton<Container>(serviceProvider =>
{
	var client = serviceProvider.GetRequiredService<CosmosClient>();
	var o = serviceProvider.GetRequiredService<IOptions<CosmosOptions>>().Value;
	Database database = client.CreateDatabaseIfNotExistsAsync(o.DatabaseName).Result;
	return database.CreateContainerIfNotExistsAsync(o.ContainerName, o.PrimaryKeyPath, o.ThroughputRequestUnits).Result;
});
builder.Services.AddSingleton<IOptimisticallyConcurrentRepository<AppointmentRequest>, CosmosAppointmentRequestRepository>();
builder.Services.AddSingleton<AppointmentRequestDtoTranslator>();
builder.Services.AddSingleton<AppointmentRequestService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { } 

using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Reflection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Add configuration for accessing appsettings.json
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

// Retrieve MongoDB connection string and database name
string connectionString = configuration["DatabaseConfigurations:ConnectionString"];
string databaseName = configuration["DatabaseConfigurations:CmsDatabaseName"];

MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
IMongoClient mongoClient = new MongoClient(settings);

// Register custom serializers for ObjectId and DateTime
//BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
//BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));
//BsonSerializer.RegisterSerializer(new StringSerializer(BsonType.ObjectId));

builder.Services.AddSingleton<IMongoClient>(mongoClient);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    // Configure Swagger settings
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "API Key",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    //lock
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API V1", Version = "v1" });
    c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API V2", Version = "v2" });

    // Include XML comments for Swagger documentation
    var filename = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
    var filepath = Path.Combine(AppContext.BaseDirectory, filename);
    c.IncludeXmlComments(filepath); // Corrected from "filename" to "filepath"
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
    });
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

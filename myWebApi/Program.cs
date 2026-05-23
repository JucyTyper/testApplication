using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");
var containerName = Environment.GetEnvironmentVariable("AzureStorag__ContainerName");


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(
options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("*") // Define trusted origins
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();



    app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("MyAllowSpecificOrigins");


app.UseHttpsRedirection();

app.MapGet("/ping", () =>
{
    return new
    {
        Message = "Application deployed successfully to Azure",
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    };
})
.WithName("ping")
.WithOpenApi();

app.MapPost("/uploads", (Data data) =>
{
    var containerClient = new BlobContainerClient(connectionString, containerName);

    containerClient.CreateIfNotExists(PublicAccessType.None);

        // Unique blob name to avoid collisions
        var blobName   = $"{Guid.NewGuid()}/{Path.GetFileName(data.fileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);

    var sasBuilder = new BlobSasBuilder
    {
        BlobContainerName = containerName,
        BlobName = blobName,
        Resource = "b",
        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
    };


    sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

    var sasUri = blobClient.GenerateSasUri(sasBuilder);

    return new { };
})
.WithName("upload")
.WithOpenApi();




app.Run();

public record Data(string fileName, string contentType);

public record SasUploadToken(
    string SasUrl,
    string BlobUrl,
    string BlobName,
    DateTimeOffset ExpiresOn
);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


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

app.Run();

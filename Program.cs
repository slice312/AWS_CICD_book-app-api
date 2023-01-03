using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using book_app_api.Services;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestMethod
                            | HttpLoggingFields.RequestPath
                            | HttpLoggingFields.RequestQuery
                            | HttpLoggingFields.RequestScheme
                            | HttpLoggingFields.RequestProtocol;
});


var corss = Environment.GetEnvironmentVariable("FRONTEND_HOST");
Console.Out.WriteLine("-----------__CORS " + corss);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                corss
            )
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// Get the AWS profile information from configuration providers
AWSOptions awsOptions = builder.Configuration.GetAWSOptions();
// Configure AWS service clients to use these credentials
builder.Services.AddDefaultAWSOptions(awsOptions);

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();


builder.Services.AddScoped<IBooksService, BooksService>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
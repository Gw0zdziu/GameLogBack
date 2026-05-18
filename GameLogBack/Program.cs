using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using GameLogBack.Configurations;
using GameLogBack.Controllers;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using GameLogBack.Interfaces;
using GameLogBack.Middlewares;
using GameLogBack.Services;
using GameLogBack.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
string connectionString;
var gameBrainApiSettings = new GameBrainApiSettings();
var authenticationSettings = new AuthenticationSettings();
BasicAWSCredentials awsCredentials;
AmazonS3Config s3Config;
BucketS3 bucketS3;
if (builder.Environment.IsDevelopment())
{
    bucketS3 = new BucketS3(builder.Configuration.GetSection("BucketS3").Value);
    awsCredentials = new BasicAWSCredentials(
        builder.Configuration["BasicAWSCredentials:AccessKey"],
        builder.Configuration["BasicAWSCredentials:SecretKey"]
    );
    s3Config = new AmazonS3Config()
    {
        ServiceURL     = builder.Configuration["AmazonS3Config:ServiceURL"],
        ForcePathStyle =  bool.Parse(builder.Configuration["AmazonS3Config:ForcePathStyle"]),
    };
    connectionString = builder.Configuration.GetConnectionString("Postgres");
    builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
    builder.Configuration.GetSection("GameBrainSettings").Bind(gameBrainApiSettings);
}
else
{
    bucketS3 = new BucketS3(Environment.GetEnvironmentVariable("BUCKET_NAME"));
    awsCredentials = new BasicAWSCredentials(
        Environment.GetEnvironmentVariable("BASIC_AWS_ACCESS_TOKEN"),
        Environment.GetEnvironmentVariable("BASIC_AWS_SECRET_KEY")
    );
    s3Config = new AmazonS3Config()
    {
        ServiceURL     = Environment.GetEnvironmentVariable("BASIC_AWS_SERVICE_URL"),
        ForcePathStyle =bool.Parse(Environment.GetEnvironmentVariable("AWS_FORCE_PATH_STYLE"))
    };
    connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    authenticationSettings = new AuthenticationSettings()
    {
        JwtKey = Environment.GetEnvironmentVariable("JWT_KEY"),
        JwtTokenExpireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_TOKEN_EXPIRE_MINUTES") ?? "15"),
        JwtAccessTokenExpireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRE_DAYS") ?? "15"),
        JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")

    };
    gameBrainApiSettings = new GameBrainApiSettings()
    {
        ApiUrl = Environment.GetEnvironmentVariable("GAME_BRAIN_API_URL"),
        ApiKey = Environment.GetEnvironmentVariable("GAME_BRAIN_API_KEY"),
        GenerateFilterOptions = Environment.GetEnvironmentVariable("GENERATE_FILTER_OPTIONS"),

    };
}
builder.Services.AddSingleton(bucketS3);
builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsCredentials, s3Config));
builder.Services.AddDbContext<GameLogDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddSingleton(gameBrainApiSettings);
builder.Services.AddHttpClient<GameBrainApiService>((client) =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json");

});
builder.Services.AddSingleton(authenticationSettings);
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("GameLogProd", policy =>
        policy.WithOrigins("https://gamelogfront.up.railway.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
        options.AddPolicy("GameLogDev", policy =>
            policy.WithOrigins("http://localhost:4300")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
        );
});
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
    option.DefaultScheme = "Bearer";
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.UseSecurityTokenValidators = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidateAudience = true,
        ValidAudience = authenticationSettings.JwtIssuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<UserLogins>, PasswordHasher<UserLogins>>();
builder.Services.AddScoped<IUtilsService, UtilsService>();
builder.Services.AddScoped<IEmailSenderHelper, EmailSenderHelper>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameBrainApiService, GameBrainApiService>();
builder.Services.AddScoped<IRailwayBucketService, RailwayBucketService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors(builder.Environment.IsDevelopment() ? "GameLogDev" : "GameLogProd");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();

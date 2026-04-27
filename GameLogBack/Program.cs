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
if (builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("Postgres");
}
else
{
    connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
}
builder.Services.AddDbContext<GameLogDbContext>(options =>
    options.UseNpgsql(connectionString));
var authenticationSettings = new AuthenticationSettings();
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
}
else
{
    authenticationSettings = new AuthenticationSettings()
    {
        JwtKey = Environment.GetEnvironmentVariable("JWT_KEY"),
        JwtTokenExpireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_TOKEN_EXPIRE_MINUTES") ?? "15"),
        JwtAccessTokenExpireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRE_DAYS") ?? "15"),
        JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")

    };
}

var gameBrainApiSettings = new GameBrainApiSettings()
{
    ApiUrl = Environment.GetEnvironmentVariable("GAME_BRAIN_API_URL"),
    ApiKey = Environment.GetEnvironmentVariable("GAME_BRAIN_API_KEY"),
    GenerateFilterOptions = Environment.GetEnvironmentVariable("GENERATE_FILTER_OPTIONS"),

};
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
var awsCredentials = new BasicAWSCredentials(
    "tid_wNGAzAYWdXBagSiOtVCprPaGXXnPhmlrRUcHnkLTAnIuMaZXOJ",
    "tsec_kaClCL4iMvRS4pUw50PgGsZ7R_IJ3WjN1pyhKBgif3j1jVO9X9sPC4xKFgPZhrHXmaRz4f"
);

var s3Config = new AmazonS3Config
{
    ServiceURL     = builder.Configuration["AWS:ServiceURL"],
    ForcePathStyle = true,
    RegionEndpoint = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"] ?? "us-east-1")
};

builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(awsCredentials, s3Config));
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

using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using FluentValidation;
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DataAccess.Repositories;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Services;
using GameLogBack.Settings;
using GameLogBack.Validators.Auth;
using GameLogBack.Validators.Category;
using GameLogBack.Validators.Game;
using GameLogBack.Validators.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Resend;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions<ResendSettings>()
    .BindConfiguration(ResendSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<BucketSettings>()
    .BindConfiguration(BucketSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<FrontendSettings>()
    .BindConfiguration(FrontendSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<AuthenticationSettings>()
    .BindConfiguration(AuthenticationSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<GameBrainApiSettings>()
    .BindConfiguration(GameBrainApiSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<PostgresSqlSettings>()
    .BindConfiguration(PostgresSqlSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();


    /*else
{
    builder.Services.AddResend(o =>
    {
        o.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_KEY")!;
    });
    bucketS3 = new BucketS3(Environment.GetEnvironmentVariable("BUCKET_NAME"));
    awsCredentials = new BasicAWSCredentials(
        Environment.GetEnvironmentVariable("BASIC_AWS_ACCESS_TOKEN"),
        Environment.GetEnvironmentVariable("BASIC_AWS_SECRET_KEY")
    );
    s3Config = new AmazonS3Config()
    {
        ServiceURL     = Environment.GetEnvironmentVariable("BASIC_AWS_SERVICE_URL"),
        ForcePathStyle =bool.Parse(Environment.GetEnvironmentVariable("AWS_FORCE_PATH_STYLE") ?? "true")
    };
    connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    authenticationSettings = new AuthenticationSettings()
    {
        JwtKey = Environment.GetEnvironmentVariable("JWT_KEY"),
        JwtAccessTokenExpireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRE_MINUTES") ?? "15"),
        JwtRefreshTokenExpireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRE_DAYS") ?? "15"),
        JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")

    };
    gameBrainApiSettings = new GameBrainApiSettings()
    {
        ApiUrl = Environment.GetEnvironmentVariable("GAME_BRAIN_API_URL"),
        ApiKey = Environment.GetEnvironmentVariable("GAME_BRAIN_API_KEY"),
        GenerateFilterOptions = Environment.GetEnvironmentVariable("GENERATE_FILTER_OPTIONS"),

    };
    frontendSettings = new FrontendSettings()
    {
        FrontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL"),
        GamesImageDirectoryName = Environment.GetEnvironmentVariable("GAMES_IMAGE_DIRECTORY_NAME"),
        RecoveryPasswordEndpoint = Environment.GetEnvironmentVariable("RECOVERY_PASSWORD_ENDPOINT"),
    };
}*/
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblyContaining<LoginUserDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryPostDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryPutDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GamePostDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GamePutDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterNewUserDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PaginatedQuery>();
builder.Services.AddControllers();

builder.Services.AddScoped<ICodeConfirmUsersRepository, CodeConfirmUsersRepository>();
builder.Services.AddScoped<ICodeRecoveryPasswordsRepository, CodeRecoveryPasswordsRepository>();
builder.Services.AddScoped<IRefreshTokenInfoRepository, RefreshTokenInfoRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserLoginsRepository, UserLoginsRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<UserLogins>, PasswordHasher<UserLogins>>();
builder.Services.AddScoped<IUtilsService, UtilsService>();
builder.Services.AddScoped<IEmailSenderHelper, EmailSenderHelper>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameBrainApiService, GameBrainApiService>();
builder.Services.AddScoped<IRailwayBucketService, RailwayBucketService>();
builder.Services.AddSingleton<IAmazonS3>(serviceProvider =>
{
    var bucketSettingsSection = serviceProvider
        .GetRequiredService<IOptions<BucketSettings>>()
        .Value;
    var awsCredentials = new BasicAWSCredentials(
        bucketSettingsSection.AccessKey,
        bucketSettingsSection.SecretKey);
    var s3Config = new AmazonS3Config()
    {
        ServiceURL  = bucketSettingsSection.ServiceUrl,
        ForcePathStyle =  bucketSettingsSection.ForcePathStyle,
    };
    return new AmazonS3Client(awsCredentials, s3Config);
});


builder.Services.AddDbContext<GameLogDbContext>((serviceProvider, options) =>
{
    var postgresSqlSettings = serviceProvider
        .GetRequiredService<IOptions<PostgresSqlSettings>>()
        .Value;
    options.UseNpgsql(postgresSqlSettings.ConnectionString);
});
builder.Services.AddResend(_ => { });
builder.Services
    .AddOptions<ResendClientOptions>()
    .Configure<IOptions<ResendSettings>>((clientOptions, settings) =>
    {
        clientOptions.ApiToken = settings.Value.ApiToken;
        clientOptions.ThrowExceptions = settings.Value.ThrowExceptions;
    });



builder.Services.AddHttpClient();
builder.Services
    .AddOptions<HttpClient>()
    .Configure<IOptions<GameBrainApiSettings>>((clientOptions, settings) =>
    {
        clientOptions.BaseAddress = new Uri(settings.Value.ApiUrl);
        clientOptions.DefaultRequestHeaders.Add("Accept", "application/json");
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("GameLogProd", policy =>
        policy.WithOrigins("https://gamelogfront.up.railway.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
    options.AddPolicy("GameLogDev", policy =>
        policy.WithOrigins("http://localhost:4300", "http://localhost:80")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
});

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<AuthenticationSettings>>((clientOptions, settings) =>
    {
        clientOptions.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        clientOptions.SaveToken = true;
        clientOptions.UseSecurityTokenValidators = true;
        clientOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = settings.Value.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = settings.Value.JwtIssuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Value.JwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, configs) =>
{
    configs
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseSerilogRequestLogging();
app.UseCors(builder.Environment.IsDevelopment() ? "GameLogDev" : "GameLogProd");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


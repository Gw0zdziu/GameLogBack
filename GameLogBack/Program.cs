using System.Net;
using System.Text;
using System.Threading.RateLimiting;
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
using Microsoft.AspNetCore.RateLimiting;
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

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddSlidingWindowLimiter("auth-user", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6; 
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

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
builder.Services.AddScoped<IInvitationCodesRepository, InvitationCodesRepository>();
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
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


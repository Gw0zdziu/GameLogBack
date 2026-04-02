using System.Text;
using GameLogBack.Authentication;
using GameLogBack.Configurations;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using GameLogBack.Interfaces;
using GameLogBack.Middlewares;
using GameLogBack.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
//var connectionString = builder.Configuration.GetConnectionString("Postgres");
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<GameLogDbContext>(options =>
    options.UseNpgsql(connectionString));
var authenticationSettings = new AuthenticationSettings()
{
    JwtKey = Environment.GetEnvironmentVariable("JWT_KEY"),
    JwtTokenExpireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_TOKEN_EXPIRE_MINUTES") ?? ""),
    JwtAccessTokenExpireDays = int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRE_DAYS") ?? ""),
    JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")

};
/*builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);*/
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
;
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
        policy.WithOrigins("http://gamelogfront", "http://localhost:4300", "http://localhost:4300",
                "https://gamelogfront.up.railway.app")
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
/*builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5; // Max requests
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueLimit = 0;
    });
});*/
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("AngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameLogDbContext>();
    dbContext.Database.Migrate();
}*/
/*app.UseRateLimiter();*/

app.Run();

using API.ApiDbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Logs dans la console
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Logs dans des fichiers
    .Enrich.FromLogContext() // Enrichissement avec des informations contextuelles
    .CreateLogger();

builder.Host.UseSerilog(); // Utilisation de Serilog comme fournisseur de logs

// Configuration de la chaîne de connexion SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 33)) // Remplacez la version par celle de votre serveur MySQL
    );
});

/**
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.LogTo(Log.Information, LogLevel.Information); // Log chaque requête SQL
});
**/
// Configuration de la sérialisation JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// Configuration de l'authentification JWT
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configuration de l'autorisation avec des rôles spécifiques
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

// Configuration de Swagger avec support pour JWT Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Veuillez entrer votre token Bearer",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

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
            new string[] { }
        }
    });
});

// Enregistrement des services
builder.Services.AddScoped<TokenService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseCors("AllowAllOrigins");

if (!app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        try
        {
            dbContext.Database.Migrate();  // Applique les migrations au démarrage
        }
        catch (Exception ex)
        {
            Log.Error($"Error applying migrations: {ex.Message}");
        }
    }
}

// Utilisation des logs dans les middlewares (affichage d'informations sur chaque requête)
app.UseSerilogRequestLogging(); // Intégration des logs des requêtes HTTP

// Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

// Utilisation des middlewares pour l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();

// Routage des contrôleurs
app.MapControllers();

app.Run();

// Program.cs
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using FacturasIA.Platform.Iam.Application.CommandServices;
using FacturasIA.Platform.Iam.Application.Internal.CommandServices;
using FacturasIA.Platform.Iam.Application.Internal.OutboundServices;
using FacturasIA.Platform.Iam.Application.QueryServices;
using FacturasIA.Platform.Iam.Application.Internal.QueryServices;
using FacturasIA.Platform.Iam.Domain.Repositories;
using FacturasIA.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;
using FacturasIA.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using FacturasIA.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;
using FacturasIA.Platform.Iam.Infrastructure.Tokens.Jwt.Services;

using FacturasIA.Platform.Invoicing.Application.CommandServices;
using FacturasIA.Platform.Invoicing.Application.Internal.CommandServices;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Application.Internal.QueryServices;
using FacturasIA.Platform.Invoicing.Domain.Repositories;
using FacturasIA.Platform.Invoicing.Infrastructure.Ia.Gemini.Configuration;
using FacturasIA.Platform.Invoicing.Infrastructure.Ia.Gemini.Services;
using FacturasIA.Platform.Invoicing.Infrastructure.Pdf.PdfPig.Services;
using FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using FacturasIA.Platform.Invoicing.Infrastructure.Storage.Oracle.Configuration;
using FacturasIA.Platform.Invoicing.Infrastructure.Storage.Oracle.Services;
using FacturasIA.Platform.Invoicing.Infrastructure.Sunat.Services;
using FacturasIA.Platform.Invoicing.Infrastructure.Sunat.Configuration;

using FacturasIA.Platform.Shared.Domain.Repositories;
using FacturasIA.Platform.Shared.Infrastructure.OpenApi;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuración (options) ----
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("Gemini"));
builder.Services.Configure<OracleObjectStorageSettings>(builder.Configuration.GetSection("OracleObjectStorage"));
builder.Services.Configure<DecolectaSettings>(builder.Configuration.GetSection("Decolecta"));

// ---- Base de datos ----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---- Shared ----
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ---- Iam: repositorios, servicios de aplicación y outbound services ----
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioCommandService, UsuarioCommandService>();
builder.Services.AddScoped<IUsuarioQueryService, UsuarioQueryService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ---- Invoicing: repositorios, servicios de aplicación y outbound services ----
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IFacturaCommandService, FacturaCommandService>();
builder.Services.AddScoped<IFacturaQueryService, FacturaQueryService>();
builder.Services.AddScoped<IResumenGastosQueryService, ResumenGastosQueryService>();
builder.Services.AddScoped<IProveedorQueryService, ProveedorQueryService>();
builder.Services.AddScoped<ICategoriaQueryService, CategoriaQueryService>();
builder.Services.AddScoped<IPdfTextExtractorService, PdfTextExtractorService>();
builder.Services.AddScoped<IAlmacenamientoService, OracleObjectStorageService>();
builder.Services.AddHttpClient<IConsultaRucService, ConsultaRucService>();
builder.Services.AddHttpClient<IOcrIaService, GeminiOcrIaService>();

// ---- Controllers ----
builder.Services.AddControllers();

// ---- Autenticación JWT ----
// Nota: esto registra el middleware estándar de ASP.NET Core (Bearer). Nuestro propio
// RequestAuthorizationMiddleware (Iam) es el que realmente resuelve el usuario en
// HttpContext.Items["User"] para los [Authorize] custom; ambos conviven sin conflicto porque
// el AuthorizeAttribute custom es el que efectivamente bloquea, no el nativo.
var tokenSecret = builder.Configuration["TokenSettings:Secret"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// ---- CORS ----
// Ajusta el origin cuando despliegues el frontend en Vercel (mismo patrón que hiciste en sistema-hostal).
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://front-clcl.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ---- Swagger ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FacturasIA API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token con el formato: Bearer {tu token}"
    });
    options.OperationFilter<AuthorizeCheckOperationFilter>();
});

var app = builder.Build();

// Program.cs — agrega esta línea, justo antes del bloque de Swagger que ya tienes
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseRequestAuthorization(); // Iam: resuelve HttpContext.Items["User"] antes de los controllers
app.UseAuthorization();
app.MapControllers();
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Add($"http://+:{port}");

app.Run();
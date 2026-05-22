using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using IntercoopAPI.Data;
using IntercoopAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la Base de Datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ¡ESTAS SON LAS LÍNEAS QUE FALTABAN PARA SWAGGER!
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configuración de JWT (Seguridad)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaveSuperSecretaParaIntercoop2026ExamenPractico!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false, 
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

var app = builder.Build();

// ¡AQUÍ ENCENDEMOS LA INTERFAZ GRÁFICA!
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(opciones => opciones.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

app.Run();
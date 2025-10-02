using Microsoft.Data.SqlClient;
using ClubesApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System;

var jwtSecret = "01234567890123456789012345678901";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ClubesDBConnection");

var finalConnectionString = connectionString ?? string.Empty;

builder.Services.AddScoped<ClubRepository>(serviceProvider =>
    new ClubRepository(finalConnectionString)
);

builder.Services.AddScoped<DirigenteRepository>(serviceProvider =>
    new DirigenteRepository(finalConnectionString)
);

builder.Services.AddScoped<SocioRepository>(serviceProvider =>
    new SocioRepository(finalConnectionString)
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "TP_WebAPI_Clubes",
            ValidAudience = "TP_WebAPI_Clubes_Clients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
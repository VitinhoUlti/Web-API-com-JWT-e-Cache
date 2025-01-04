using Testezin.Contexto;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Testezin.Servicos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<HobbiesContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConexaoPadrao")));
builder.Services.AddDbContext<UsuariosContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConexaoPadrao")));

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =  JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Chave.chaveSecreta)),
        
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

public partial class Program { }
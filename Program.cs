using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minimal_API.Dominio.interfaces;
using Minimal_API.Dominio.Servicos;
using MinimalAPI.Dominio.ModelsViews;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("ConexaoString"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConexaoString")));
});

var app = builder.Build();


app.MapGet("/", () => Results.Json(new Home()));


app.MapPost("/login",([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico)=> 
{
    	if(administradorServico.Login(loginDTO) != null) 
            return Results.Ok("Login com sucesso");
        else 
            return Results.Unauthorized();
});



app.UseSwagger();
app.UseSwaggerUI(); 

app.Run();


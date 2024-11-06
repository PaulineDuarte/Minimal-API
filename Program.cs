using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("ConexaoString"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConexaoString")));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapPost("/login",(LoginDTO loginDTO)=> 
{
    	if(loginDTO.Email=="adm@teste.com" && loginDTO.Senha=="123456") 
            return Results.Ok("Login com sucesso");
        else 
            return Results.Unauthorized();
});


app.Run();


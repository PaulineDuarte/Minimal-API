using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.interfaces;
using Minimal_API.Dominio.Servicos;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.Dominio.ModelsViews;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestrutura.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("ConexaoString"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConexaoString")));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion 


#region Login Administrador

app.MapPost("/administradores/login",([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico)=> 
{
    	if(administradorServico.Login(loginDTO) != null) 
            return Results.Ok("Login com sucesso");
        else 
            return Results.Unauthorized();
}).WithTags("Administrador");
#endregion

#region Veiculos 
app.MapPost("/veiculos",([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)=> 
{       //nova instancia de veiculos
        
    	var veiculo = new Veiculo
        {
            Nome = veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };
        veiculoServico.Incluir(veiculo);
        return Results.Created($"/veiculo/{veiculo.Id}",veiculo); 
}).WithTags("Veiculos");

app.MapGet("/veiculos",([FromQuery] int pagina, IVeiculoServico veiculoServico)=> 
{    
            var veiculos = veiculoServico.Todos(pagina);
            
            return Results.Ok(veiculos); 
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI(); 

app.Run();
#endregion
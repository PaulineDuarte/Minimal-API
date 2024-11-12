using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.interfaces;
using Minimal_API.Dominio.Servicos;
using MinimalAPI.Dominio.Entidades;
using Minimal_API.Dominio.ModelViews;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestrutura.Db;
using MinimalAPI.Dominio.Enuns;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key="123456"; 

builder.Services.AddAuthentication(option => {
    option.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});


builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui : "
    }); 

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    { 
        {
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference 
            {
                Type = ReferenceType.SecurityScheme,
                Id="Bearer"
            }
        },
         new string[] {}
    
        }
    });
});



builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(builder.Configuration.GetConnectionString("ConexaoString"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConexaoString")));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");




#endregion 


#region Administrador

string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var claims = new List<Claim>() 
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };

    var securityKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims : claims,
        expires : DateTime.Now.AddDays(1),
        signingCredentials : credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores/login",([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico)=> 
{
        var adm = administradorServico.Login(loginDTO);
    	if(adm != null) {
            string token = GerarTokenJwt(adm);
            return Results.Ok(new AdministradorLogado
            {
                Email = adm.Email,
                Perfil = adm.Perfil,
                Token = token
            });
        }
        else 
            return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");



app.MapGet("/administradores",([FromQuery] int pagina, IAdministradorServico administradorServico)=> 
{       
        var adms = new List<AdministradorModelView>();

        var administradores = administradorServico.Todos(pagina);

        foreach(var adm in administradores)
        {
            adms.Add(new AdministradorModelView {
                Id = adm.Id,
                Email = adm.Email,
                Perfil = adm.Perfil
            });
        }

        return Results.Ok(adms);
        
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administradores");




app.MapGet("/administrador/{id}",([FromRoute] int Id, IAdministradorServico administradorServico)=> 
{    
            var administrador = administradorServico.BuscaPorId(Id);
            if (administrador == null) 
                return Results.NotFound();
            
            return Results.Ok(new AdministradorModelView {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            }); 
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administradores");




app.MapPost("/administradores",([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico)=> 
{     
    var validacao = new ErrosValidacao 
     {
        Mensagens= new List<string>()
     };

     if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio");

    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("Senha não pode ser vazia");

    
    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Perfil não pode ser vazio");
    
    if (validacao.Mensagens.Count>0)
        return Results.BadRequest(validacao);


    	var administrador = new Administrador
        {
            Email = administradorDTO.Email,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil.ToString()
        };
        administradorServico.Incluir(administrador);
        return Results.Created($"/administrador/{administrador.Id}",new AdministradorModelView {
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            }); 
    	
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
.WithTags("Administradores");

#endregion



#region Veiculos 

ErrosValidacao validaDTO(VeiculoDTO veiculoDTO)
{
     var validacao = new ErrosValidacao
     {
        Mensagens = new List<string>()
     };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome não pode ser vazio");

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A Marca não ficar em branco");

    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores a 1950");
    
    return validacao;
}


app.MapPost("/veiculos",([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)=> 
{      
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count>0)
        return Results.BadRequest(validacao);

    	var veiculo = new Veiculo
        {
            Nome = veiculoDTO.Nome,
            Marca = veiculoDTO.Marca,
            Ano = veiculoDTO.Ano
        };
        veiculoServico.Incluir(veiculo);
        return Results.Created($"/veiculo/{veiculo.Id}",veiculo); 
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm,Editor"})
.WithTags("Veiculos");

// Retornar todos os veiculos
app.MapGet("/veiculos",([FromQuery] int pagina, IVeiculoServico veiculoServico)=> 
{    
            var veiculos = veiculoServico.Todos(pagina);
            
            return Results.Ok(veiculos); 
}).RequireAuthorization().WithTags("Veiculos");

// retornar um veiculo
app.MapGet("/veiculos/{id}",([FromRoute] int Id, IVeiculoServico veiculoServico)=> 
{    
            var veiculo = veiculoServico.BuscaPorId(Id);
            if (veiculo == null) 
                return Results.NotFound();
            
            return Results.Ok(veiculo); 
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm,Editor"})
.WithTags("Veiculos");

// PUT para atualizar veiculo
app.MapPut("/veiculos/{id}",([FromRoute] int Id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)=> 
{    
        var veiculo = veiculoServico.BuscaPorId(Id);
        if (veiculo == null) return Results.NotFound();
        
        var validacao = validaDTO(veiculoDTO);
        if (validacao.Mensagens.Count>0)
            return Results.BadRequest(validacao);
        
        veiculo.Nome = veiculoDTO.Nome;
        veiculo.Marca = veiculoDTO.Marca;
        veiculo.Ano = veiculoDTO.Ano;

        veiculoServico.Atualizar(veiculo);

        return Results.Ok(veiculo); 

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Veiculos");



app.MapDelete("/veiculos/{Id}",([FromRoute] int Id, IVeiculoServico veiculoServico)=> 
{    
        var veiculo = veiculoServico.BuscaPorId(Id);
        if (veiculo == null) return Results.NotFound();

        veiculoServico.Apagar(veiculo);
        return Results.NoContent(); 

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Veiculos");


#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
    c.RoutePrefix = string.Empty; // Configura Swagger para a rota raiz se desejar
}); 

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion
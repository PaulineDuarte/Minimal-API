using MinimalAPI.Dominio.Entidades;
using Minimal_API.Dominio.Servicos;
using Microsoft.Extensions.Configuration;
using MinimalAPI.Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.DTOs;
using System.Reflection;


namespace Test.Domain.Entidades;


[TestClass]
public class AdministradorServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        var AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(AssemblyPath ?? "", "..", "..", ".."));

        //Configurar o ConfigurationBuilder
        var builder = new ConfigurationBuilder()
        .SetBasePath(path ?? Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();


        var configuration = builder.Build();

        return new DbContexto(configuration);



    }

    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        //Arrage
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");


        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";


        var administradorServico = new AdministradorServico(context);

        //Act 
        administradorServico.Incluir(adm);

        // Assert 
        Assert.AreEqual(1, administradorServico.Todos(1).Count());

    }


    [TestMethod]
    public void TestandoBuscaPorId()
    {
        //Arrage
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");


        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";


        var administradorServico = new AdministradorServico(context);

        //Act 
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscaPorId(adm.Id);

        // Assert 
        Assert.AreEqual(1, admDoBanco.Id);

    }



    [TestMethod]
    public void TestandoLogin()
    {
        //Arrage
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");


        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var loginDTO = new LoginDTO
        {
        Email = "teste@teste.com",
        Senha = "teste"
        };

    var administradorServico = new AdministradorServico(context);

    // Act
    // Incluir o administrador na base de dados
    administradorServico.Incluir(adm);

    // Tentar realizar o login
    var admLogado = administradorServico.Login(loginDTO);

    // Assert
    Assert.IsNotNull(admLogado);
    Assert.AreEqual(adm.Email, admLogado.Email);  
    Assert.AreEqual(adm.Senha, admLogado.Senha);  
}

}
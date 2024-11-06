using Microsoft.EntityFrameworkCore;
using MinimalAPI.Dominio.Entidades;



namespace MinimalAPI.Infraestrutura.Db; 

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
    public DbSet<Administrador> Administradores {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var stringConexao= _configuracaoAppSettings.GetConnectionString("ConexaoString").ToString();
        if(!string.IsNullOrEmpty(stringConexao))
        {
            optionsBuilder.UseMySql(stringConexao,ServerVersion.AutoDetect(stringConexao));
        } 
        
    }
}
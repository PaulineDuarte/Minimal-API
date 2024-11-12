using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.interfaces;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.Infraestrutura.Db;

namespace Minimal_API.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto; 
    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto ; 
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
         _contexto.SaveChanges();
    }

    public Veiculo BuscaPorId(int Id)
    {
       return _contexto.Veiculos.Where(v => v.Id == Id).FirstOrDefault(); 
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public List<Veiculo> Todos(int pagina, string nome, string marca)
    {
       var query = _contexto.Veiculos.AsQueryable(); 
       if (!string.IsNullOrEmpty(nome))
       {
        query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(),$"%{nome.ToLower()}%"));
       }

       int itensPorPagina = 10; 

       query = query.Skip((pagina-1) * itensPorPagina).Take(itensPorPagina);

       return query.ToList() ; 
    }
}
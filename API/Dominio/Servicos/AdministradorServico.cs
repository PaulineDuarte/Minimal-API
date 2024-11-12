using System.Data.Common;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Minimal_API.Dominio.interfaces;
using MinimalAPI.Dominio.Entidades;
using MinimalAPI.DTOs;
using MinimalAPI.Infraestrutura.Db;

namespace Minimal_API.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto; 
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto ; 
    }

    public Administrador BuscaPorId(int id)
    {
        return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
    }

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public Administrador Login(LoginDTO loginDTO)
    {
       var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
       return adm ; 
    }

    public List<Administrador> Todos(int pagina)
    {
        var query = _contexto.Administradores.AsQueryable(); 
       
       int itensPorPagina = 10; 

       query = query.Skip(((int)pagina-1) * itensPorPagina).Take(itensPorPagina);

       return query.ToList() ; 
    }
}
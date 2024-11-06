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
    public Administrador Login(LoginDTO loginDTO)
    {
       var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
       return adm ; 
    }
}
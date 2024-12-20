using MinimalAPI.Dominio.Entidades;
using MinimalAPI.DTOs;

namespace Minimal_API.Dominio.interfaces ;

public interface IAdministradorServico
{
    Administrador Login(LoginDTO loginDTO);
    
    Administrador Incluir(Administrador administrador);

    Administrador BuscaPorId(int id );

    List<Administrador>Todos(int pagina);
}
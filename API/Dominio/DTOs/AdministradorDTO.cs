
using MinimalAPI.Dominio.Enuns;

namespace MinimalAPI.DTOs; 
public class AdministradorDTO 
{
    public string Email { get; set; }
    public string Senha { get; set; }
    public Perfil Perfil { get; set; }
}
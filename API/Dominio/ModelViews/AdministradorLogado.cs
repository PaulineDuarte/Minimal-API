namespace Minimal_API.Dominio.ModelViews
{
    public record AdministradorLogado
    {
        public string Email { get; set; }

        public string Perfil { get; set; }
        public string Token { get; set; }
    }
}
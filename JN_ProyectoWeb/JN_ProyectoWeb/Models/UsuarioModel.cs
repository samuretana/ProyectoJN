namespace JN_ProyectoWeb.Models
{
    public class UsuarioModel
    {
        public long Id { get; set; }
        public string? Identificacion { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Correo { get; set; }
        public string? Contrasenna { get; set; }
        public long IdPerfil { get; set; }
        public string? NombrePerfil { get; set; }
        public string? Token { get; set; }
    }
}

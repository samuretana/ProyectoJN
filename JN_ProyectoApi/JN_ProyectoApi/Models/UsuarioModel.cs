namespace JN_ProyectoApi.Models
{
    public class UsuarioModel
    {
        public long Id { get; set; }
        public string? Identificacion { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Contrasenna { get; set; }
        public string? Token { get; set; }
    }
}

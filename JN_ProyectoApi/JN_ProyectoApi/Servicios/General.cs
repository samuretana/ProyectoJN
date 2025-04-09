using System.Security.Claims;

namespace JN_ProyectoApi.Servicios
{
    public class General : IGeneral
    {
        public long ObtenerUsuarioFromToken(IEnumerable<Claim> valores)
        {
            if (valores.Any())
            {
                var IdUsuario = valores.FirstOrDefault(x => x.Type == "Id")?.Value;

                return long.Parse(IdUsuario!);
            }
            return 0;
        }

        public bool ValidarUsuarioReclutadorFromToken(IEnumerable<Claim> valores)
        {
            if (valores.Any())
            {
                var IdPerfil = valores.FirstOrDefault(x => x.Type == "IdPerfil")?.Value;

                return (IdPerfil == "1");
            }
            return false;
        }
    }
}

using System.Security.Claims;

namespace JN_ProyectoApi.Servicios
{
    public interface IGeneral
    {
        long ObtenerUsuarioFromToken(IEnumerable<Claim> valores);
    }
}

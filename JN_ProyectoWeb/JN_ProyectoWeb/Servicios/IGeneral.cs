using JN_ProyectoWeb.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace JN_ProyectoWeb.Servicios
{
    public interface IGeneral
    {
        List<PuestosModel> ConsultarDatosPuestos(long Id);
        HttpResponseMessage ConsultarDatosOfertas(long Id);

        string Encrypt(string texto);
    }
}

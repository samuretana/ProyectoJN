using JN_ProyectoWeb.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace JN_ProyectoWeb.Servicios
{
    public interface IGeneral
    {
        HttpResponseMessage ConsultarDatosPuestos(long Id);
        HttpResponseMessage ConsultarDatosOfertas(long Id);
        HttpResponseMessage ConsultarDatosOfertasDisponibles();
        HttpResponseMessage ConsultarDatosOfertasAplicadas();
        HttpResponseMessage ConsultarDatosEstados();

        string Encrypt(string texto);
    }
}

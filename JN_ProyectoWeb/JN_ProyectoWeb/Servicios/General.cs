using JN_ProyectoWeb.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JN_ProyectoWeb.Servicios
{
    public class General : IGeneral
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuratrion;
        private readonly IHttpContextAccessor _contextAccessor;
        public General(IHttpClientFactory httpClient, IConfiguration configuratrion, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _configuratrion = configuratrion;
            _contextAccessor = contextAccessor;
        }

        public List<PuestosModel> ConsultarDatosPuestos(long Id)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Puestos/ConsultarPuestos?Id=" + Id;

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        return JsonSerializer.Deserialize<List<PuestosModel>>((JsonElement)result!.Datos!)!;
                    }
                }
                return new List<PuestosModel>();
            }
        }

        public List<OfertasModel> ConsultarDatosOfertas(long Id)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Ofertas/Consultarofertas?Id=" + Id;

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        return JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result!.Datos!)!;
                    }
                }
            }

            return new List<OfertasModel>();

        }

    }
}

using JN_ProyectoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JN_ProyectoWeb.Controllers
{
    public class OfertasController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuratrion;
        public OfertasController(IHttpClientFactory httpClient, IConfiguration configuratrion)
        {
            _httpClient = httpClient;
            _configuratrion = configuratrion;

        }
        public IActionResult ConsultarOfertas()
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Ofertas/Consultarofertas";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        var datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result!.Datos!);
                        return View(datosResult);
                    }
                }
            }

            return View(new List<OfertasModel>());
        }

        [HttpGet]
        public IActionResult RegistrarOfertas()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarOfertas(OfertasModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Ofertas/RegistrarOferta";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ConsultarOfertas", "Ofertas");
            }

            return View();
        }
    }
}

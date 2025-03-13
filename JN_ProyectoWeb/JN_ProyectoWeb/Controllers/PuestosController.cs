using JN_ProyectoWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace JN_ProyectoWeb.Controllers
{
    public class PuestosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuratrion;
        public PuestosController(IHttpClientFactory httpClient, IConfiguration configuratrion)
        {
            _httpClient = httpClient;
            _configuratrion = configuratrion;

        }
        public IActionResult ConsultarPuestos()
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Puestos/ConsultarPuestos";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        var datosResult = JsonSerializer.Deserialize<List<PuestosModel>>((JsonElement)result!.Datos!);
                        return View(datosResult);
                    }
                }
            }

            return View(new List<PuestosModel>());
        }


        [HttpGet]
        public IActionResult RegistrarPuestos()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarPuestos(PuestosModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Puestos/RegistrarPuestos";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ConsultarPuestos", "Puestos");
            }

            return View();
        }
    }
}

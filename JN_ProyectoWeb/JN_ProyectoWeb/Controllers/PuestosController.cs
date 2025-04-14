using JN_ProyectoWeb.Models;
using JN_ProyectoWeb.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace JN_ProyectoWeb.Controllers
{
    [FiltroSesion]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class PuestosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuratrion;
        private readonly IGeneral _general;
        public PuestosController(IHttpClientFactory httpClient, IConfiguration configuratrion, IGeneral general)
        {
            _httpClient = httpClient;
            _configuratrion = configuratrion;
            _general = general;
        }
        public IActionResult ConsultarPuestos()
        {            
             var datosResult = _general.ConsultarDatosPuestos(0);
             return View(datosResult);
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
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Puestos/RegistrarPuesto";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ConsultarPuestos", "Puestos");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ActualizarPuestos(long Id)
        {
            var response = _general.ConsultarDatosPuestos(Id);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<PuestosModel>>((JsonElement)result.Datos!);
                    return View(datosResult!.FirstOrDefault());
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";

            return View(new PuestosModel());
        }

        [HttpPost]
        public IActionResult ActualizarPuestos(PuestosModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Puestos/ActualizarPuesto";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ConsultarPuestos", "Puestos");
            }

            return View();
        }
    }
}

using JN_ProyectoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace JN_ProyectoWeb.Controllers
{
    [FiltroSesion]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UsuariosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuratrion;
        public UsuariosController(IHttpClientFactory httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuratrion = configuration;
            
        }

        [HttpGet]
        public IActionResult ActualizarDatos(long Id)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Usuarios/ConsultarUsuario?Id=0";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        var datos = JsonSerializer.Deserialize<UsuarioModel>((JsonElement)result!.Datos!)!;
                        return View(datos);
                    }
                }
                return View(new UsuarioModel());
            }
        }

        [HttpPost]
        public IActionResult ActualizarDatos(UsuarioModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Usuarios/ActualizarUsuario";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    HttpContext.Session.SetString("Nombre", model.NombreUsuario!);
                    return RedirectToAction("ActualizarDatos", "Usuarios");
            }            
        }
    }
}

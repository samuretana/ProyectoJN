using JN_ProyectoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using JN_ProyectoWeb.Servicios;

namespace JN_ProyectoWeb.Controllers
{
    [FiltroSesion]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UsuariosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGeneral _general;
        public UsuariosController(IHttpClientFactory httpClient, IConfiguration configuration, IGeneral general)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _general = general;
            
        }

        [HttpGet]
        public IActionResult ActualizarDatos(long Id)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuration.GetSection("Variables:urlWebApi").Value + "Usuarios/ConsultarUsuario?Id=0";

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
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Usuarios/ActualizarUsuario";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        HttpContext.Session.SetString("Nombre", model.NombreUsuario!);
                        return RedirectToAction("Principal", "Login");
                    }
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    ViewBag.Msj = "No se pudo completar su petición";
            }

            return View();
        }

        [HttpGet]
        public IActionResult ActualizarContrasenna()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ActualizarContrasenna(UsuarioModel model)
        {
            model.Contrasenna = _general.Encrypt(model.Contrasenna!);
            model.ContrasennaAnterior = _general.Encrypt(model.ContrasennaAnterior!);

            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Usuarios/ActualizarContrasenna";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        return RedirectToAction("ActualizarDatos", "Usuarios");
                    }
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    ViewBag.Msj = "No se pudo completar su petición";
            }

            return View();
        }
    }
}

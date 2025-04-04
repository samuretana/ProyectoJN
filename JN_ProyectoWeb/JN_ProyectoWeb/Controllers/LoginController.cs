using JN_ProyectoWeb.Models;
using JN_ProyectoWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace JN_ProyectoWeb.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class LoginController : Controller
    {

        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGeneral _general;
        public LoginController(IHttpClientFactory httpClient, IConfiguration configuratrion, IGeneral general)
        {
            _httpClient = httpClient;
            _configuration = configuratrion;
            _general = general;
        }

        #region RegistrarCuenta

        [HttpGet]
        public IActionResult RegistrarCuenta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarCuenta(UsuarioModel model)
        {
            model.Contrasenna = _general.Encrypt(model.Contrasenna!);

            using (var http = _httpClient.CreateClient())
            {
                string url = _configuration.GetSection("Variables:urlWebApi").Value + "Login/RegistrarCuenta";
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                        return RedirectToAction("IniciarSesion", "Login");
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    ViewBag.Msj = "No se pudo completar su petición";
                    
            }

            return View();
        }

        #endregion

        #region IniciarSesion

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(UsuarioModel model)
        {
            model.Contrasenna = _general.Encrypt(model.Contrasenna!);


            using (var http = _httpClient.CreateClient())
            {
                string url = _configuration.GetSection("Variables:urlWebApi").Value + "Login/IniciarSesion";
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if(result != null && result.Indicador)
                    {
                        var datosResult = JsonSerializer.Deserialize<UsuarioModel>((JsonElement)result!.Datos!);

                        HttpContext.Session.SetString("Token",datosResult!.Token!);
                        HttpContext.Session.SetString("Nombre",datosResult!.NombreUsuario!);
                        HttpContext.Session.SetString("NombrePerfil",datosResult!.NombrePerfil!);
                        HttpContext.Session.SetString("IdPerfil", datosResult!.IdPerfil.ToString());
                        return RedirectToAction("Principal", "Login");
                    }
                    else
                    {
                        ViewBag.Msj = result!.Mensaje;
                    }
                }
                else
                {
                    ViewBag.Msj = "No se pudo completar su petición";
                }
            }

            return View();
        }

        #endregion

        #region RecuperarContrasenna
        [HttpGet]
        public IActionResult RecuperarContrasenna()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarContrasenna(UsuarioModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuration.GetSection("Variables:urlWebApi").Value + "Login/RecuperarContrasenna";
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                        return RedirectToAction("IniciarSesion", "Login");
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    ViewBag.Msj = "No se pudo completar su petición";

            }

            return View();
        }
        #endregion

        [FiltroSesion]
        [HttpGet]
        public IActionResult Principal()
        {
            return View();
        }

        [FiltroSesion]
        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("IniciarSesion", "Login");
        }
    }
}

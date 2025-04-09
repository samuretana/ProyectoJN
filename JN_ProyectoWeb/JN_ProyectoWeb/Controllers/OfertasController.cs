using JN_ProyectoWeb.Models;
using JN_ProyectoWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace JN_ProyectoWeb.Controllers
{
    [FiltroSesion]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class OfertasController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IGeneral _general;
        public OfertasController(IHttpClientFactory httpClient, IConfiguration configuratrion, IGeneral general)
        {
            _httpClient = httpClient;
            _configuration = configuratrion;
            _general = general;
        }

        [HttpGet]
        public IActionResult ConsultarOfertas()
        {
            var response = _general.ConsultarDatosOfertas(0);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result.Datos!);
                    return View(datosResult);
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";

            return View(new List<OfertasModel>());
        }

        [HttpGet]
        public IActionResult RegistrarOfertas()
        {
            CargarComboPuestos();
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarOfertas(OfertasModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Ofertas/RegistrarOferta";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                        return RedirectToAction("ConsultarOfertas", "Ofertas");
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    ViewBag.Msj = "No se pudo completar su petición";
            }

            return View();
        }

        [HttpGet]
        public IActionResult ActualizarOfertas(long Id)
        {
            CargarComboPuestos();
            var response = _general.ConsultarDatosOfertas(Id);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result.Datos!);
                    return View(datosResult!.FirstOrDefault());
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";

            return View(new OfertasModel());


        }

        [HttpPost]
        public IActionResult ActualizarOfertas(OfertasModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuration.GetSection("Variables:urlWebApi").Value + "Ofertas/ActualizarOferta";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ConsultarOfertas", "Ofertas");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ObtenerOfertasUsuario()
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Ofertas/ConsultarUsuariosOfertas";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        var datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result.Datos!);
                        return Json(datosResult);
                    }
                        
                }
                
            }


            return Json(null);
        }

        [HttpGet]
        public IActionResult ConsultarOfertasDisponibles()
        {

            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Ofertas/ConsultarOfertasDisponibles";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result.Datos!);
                    return View(datosResult);
                }
                else
                    ViewBag.Msj = result!.Mensaje;
            }
            else
                ViewBag.Msj = "No se pudo completar su petición";
            }

            return View(new List<OfertasModel>());
        }

        private void CargarComboPuestos()
        {
            var datosResult = _general.ConsultarDatosPuestos(0);
            var puestosSelect = new List<SelectListItem>();
            puestosSelect.Add(new SelectListItem { Text = "-- Seleccione --", Value = string.Empty });

            foreach (var datos in datosResult)
            {
                puestosSelect.Add(new SelectListItem { Text = datos.Nombre, Value = datos.Id.ToString() });

            }

            ViewBag.Puestos = puestosSelect;
        }
    }
}

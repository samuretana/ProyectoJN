using JN_ProyectoWeb.Models;
using JN_ProyectoWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JN_ProyectoWeb.Controllers
{
    [FiltroSesion]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class OfertasController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuratrion;
        private readonly IGeneral _general;
        public OfertasController(IHttpClientFactory httpClient, IConfiguration configuratrion, IGeneral general)
        {
            _httpClient = httpClient;
            _configuratrion = configuratrion;
            _general = general;
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
            CargarComboPuestos();
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

        [HttpGet]
        public IActionResult ActualizarOfertas(long Id)
        {
            CargarComboPuestos();
            var datosResult = _general.ConsultarDatosOfertas(Id).FirstOrDefault();
            return View(datosResult);

            
        }

        [HttpPost]
        public IActionResult ActualizarOfertas(OfertasModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Ofertas/ActualizarOferta";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ConsultarOfertas", "Ofertas");
            }

            return View();
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

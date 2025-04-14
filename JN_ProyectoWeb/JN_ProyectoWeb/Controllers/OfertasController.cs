using JN_ProyectoWeb.Models;
using JN_ProyectoWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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

        // Utilizado para llenar el formulario ObtenerOfertasUsuario()
        [HttpPost]
        public IActionResult ObtenerOfertasUsuario()
        {
            var response = _general.ConsultarDatosOfertasAplicadas();

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result.Datos!);
                    return Json(datosResult);
                }

            }

            return Json(null);
        }

        [HttpGet]
        public IActionResult ConsultarOfertasDisponibles()
        {
            var response = _general.ConsultarDatosOfertasDisponibles();

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
        public IActionResult ConsultarOfertasAplicadas()
        {
            var response = _general.ConsultarDatosOfertasAplicadas();

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

        public IActionResult SeguimientoOfertas()
        {
            CargarComboEstados();

            var response = _general.ConsultarDatosOfertasAplicadas();

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

        [HttpPost]
        public IActionResult AplicarOferta(OfertasModel model)
        {

            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Ofertas/AplicarOferta";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        //Pantalla de Ofertas Aplicadas
                        return RedirectToAction("Principal", "Login");
                    }
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    ViewBag.Msj = "No se pudo completar su petición";
            }

            //En caso de no aplicar correctamente
            

            var responseConsulta = _general.ConsultarDatosOfertasDisponibles();
            var datosResult = new List<OfertasModel>();

            if (responseConsulta.IsSuccessStatusCode)
            {
                var result = responseConsulta.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result.Datos!);
                }
            }

            return View("ConsultarOfertasDisponibles", datosResult);
        }

        [HttpPost]
        public IActionResult ActualizarProcesoOferta(OfertasModel model)
        {
            using (var http = _httpClient.CreateClient())
            {
                var url = _configuration.GetSection("Variables:urlWebApi").Value + "Ofertas/ActualizarProcesoOferta";

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("Token"));
                var response = http.PutAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        //Pantalla de Ofertas Aplicadas
                        return RedirectToAction("SeguimientoOfertas", "Ofertas");
                    }
                    else
                        ViewBag.Msj = result!.Mensaje;
                }
                else
                    ViewBag.Msj = "No se pudo completar su petición";
            }

            //En caso de no aplicar correctamente
            CargarComboEstados();

            var responseConsulta = _general.ConsultarDatosOfertasAplicadas();
            var datosResult = new List<OfertasModel>();

            if (responseConsulta.IsSuccessStatusCode)
            {
                var result = responseConsulta.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    datosResult = JsonSerializer.Deserialize<List<OfertasModel>>((JsonElement)result.Datos!);
                }
            }

            return View("SeguimientoOfertas", datosResult);
        }

        private void CargarComboPuestos()
        {
            var response = _general.ConsultarDatosPuestos(0);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {
                    var datosResult = JsonSerializer.Deserialize<List<PuestosModel>>((JsonElement)result.Datos!);

                    if (datosResult != null && datosResult.Any())
                    {
                        ViewBag.Puestos = new SelectList(datosResult, "Id", "Nombre");
                    }
                }
            }
        }

        private void CargarComboEstados()
        {
            var response = _general.ConsultarDatosEstados();

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                if (result != null && result.Indicador)
                {

                    var datosResult = JsonSerializer.Deserialize<List<EstadosModel>>((JsonElement)result.Datos!);

                    if (datosResult != null && datosResult.Any())
                    {
                        datosResult.Add(new EstadosModel { Id = 0, EstadoParticipacion = "..." });
                        ViewBag.Estados = new SelectList(datosResult.OrderBy(x => x.Id), "Id", "EstadoParticipacion");
                    }
                }
            }
        }
    }
}

using JN_ProyectoWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoWeb.Controllers
{
    public class LoginController : Controller
    {

        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuratrion;
        public LoginController(IHttpClientFactory httpClient, IConfiguration configuratrion)
        {
            _httpClient = httpClient;
            _configuratrion = configuratrion;
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
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Login/RegistrarCuenta";
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("IniciarSesion", "Login");
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
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuratrion.GetSection("Variables:urlWebApi").Value + "Login/IniciarSesion";
                var response = http.PostAsJsonAsync(url, model).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Principal", "Login");
            }

            return View();
        }

        #endregion

        [HttpGet]
        public IActionResult Principal()
        {
            return View();
        }

      

        [HttpGet]

        public IActionResult RecuperarContrasenna()
        {
            return View();
        }
    }
}

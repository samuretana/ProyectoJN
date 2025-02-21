using JN_ProyectoWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
            model.Contrasenna = Encrypt(model.Contrasenna!);
            
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
            model.Contrasenna = Encrypt(model.Contrasenna!);


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

        private string Encrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_configuratrion.GetSection("Variables:llaveCifrado").Value!);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(texto);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
    }
}

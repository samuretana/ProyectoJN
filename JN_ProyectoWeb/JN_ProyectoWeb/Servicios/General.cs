using JN_ProyectoWeb.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace JN_ProyectoWeb.Servicios
{
    public class General : IGeneral
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public General(IHttpClientFactory httpClient, IConfiguration configuratrion, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuratrion;
            _contextAccessor = contextAccessor;
        }

        public List<PuestosModel> ConsultarDatosPuestos(long Id)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuration.GetSection("Variables:urlWebApi").Value + "Puestos/ConsultarPuestos?Id=" + Id;

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaModel>().Result;

                    if (result != null && result.Indicador)
                    {
                        return JsonSerializer.Deserialize<List<PuestosModel>>((JsonElement)result!.Datos!)!;
                    }
                }
                return new List<PuestosModel>();
            }
        }

        public HttpResponseMessage ConsultarDatosOfertas(long Id)
        {
            using (var http = _httpClient.CreateClient())
            {
                string url = _configuration.GetSection("Variables:urlWebApi").Value + "Ofertas/Consultarofertas?Id=" + Id;

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext!.Session.GetString("Token"));
                var response = http.GetAsync(url).Result;

                return response;

            }
        }

        public string Encrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_configuration.GetSection("Variables:llaveCifrado").Value!);
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

        public string Decrypt(string texto)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(texto);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_configuration.GetSection("Variables:llaveCifrado").Value!);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}

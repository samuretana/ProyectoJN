using Dapper;
using JN_ProyectoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JN_ProyectoApi.Controllers
{
    [AllowAnonymous] // Todos los métodos de este controlador son anónimos
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("RegistrarCuenta")]
        public IActionResult RegistrarCuenta(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("RegistrarCuenta", //Procedimiento Almacenado
                    new { model.Identificacion, model.NombreUsuario, model.Correo, model.Contrasenna });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha registrado correctamente";
                }

                return Ok(respuesta);
            }
        }

        [HttpPost]
        [Route("IniciarSesion")]
        public IActionResult IniciarSesion(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.QueryFirstOrDefault<UsuarioModel>("IniciarSesion",
                    new { model.Identificacion, model.Contrasenna });

                var respuesta = new RespuestaModel();

                if (result != null)
                {
                    result.Token = GenerarToken(result.Id, result.IdPerfil);

                    respuesta.Indicador = true;
                    respuesta.Datos = result;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha validado correctamente";
                }

                return Ok(respuesta);
            }
        }

        //Recuperar la contraseña
        [HttpPut]
        [Route("RecuperarContrasenna")]

        public IActionResult RecuperarContrasenna(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.QueryFirstOrDefault<UsuarioModel>("ValidarUsuarioCorreo",
                    new { model.Correo });

                var respuesta = new RespuestaModel();

                if (result != null)
                {

                    var Codigo = GenerarCodigo();
                    var Contrasenna = Encrypt(Codigo);
                    var ContrasennaAnterior = string.Empty;

                    //Actualizar contraseña
                    context.Execute("ActualizarContrasenna", new { result.Id, Contrasenna, ContrasennaAnterior });

                    //Notificar al Usuario
                    string Contenido = "Hola " + result.NombreUsuario + ". Se ha generado el siguiente código de seguridad: " + Codigo;
                    EnviarCorreo(result.Correo!, "Actualización de Acceso", Contenido);

                    respuesta.Indicador = true;
                    respuesta.Datos = result;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha validado correctamente";
                }

                return Ok(respuesta);
            }
        }

        private string GenerarToken(long Id, long IdPerfi)
        {
            string SecretKey = _configuration.GetSection("Variables:llaveToken").Value!;

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", Id.ToString()));
            claims.Add(new Claim("IdPerfi", IdPerfi.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: cred);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerarCodigo()
        {
            int length = 8;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ012456789";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private string Encrypt(string texto)
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

        private string Decrypt(string texto)
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

        private void EnviarCorreo(string destino, string asunto, string contenido)
        {
            string cuenta = _configuration.GetSection("Variables:CorreoEmail").Value!;
            string contrasenna = _configuration.GetSection("Variables:ClaveEmail").Value!;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(cuenta);
            message.To.Add(new MailAddress(destino));
            message.Subject = asunto;
            message.Body = contenido;
            message.Priority = MailPriority.Normal;
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
            client.Credentials = new System.Net.NetworkCredential(cuenta, contrasenna);
            client.EnableSsl = true;

            //Esto es para que no se intente enviar el correo si no hay una contraseña
            if (!string.IsNullOrEmpty(contrasenna))
            {
                client.Send(message);
            }
        }
    }
}
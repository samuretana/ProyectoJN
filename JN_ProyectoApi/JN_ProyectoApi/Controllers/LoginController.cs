using Dapper;
using JN_ProyectoApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JN_ProyectoApi.Controllers
{
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
        [Route ("RegistrarCuenta")]
        public IActionResult RegistrarCuenta(UsuarioModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {

                var result = context.Execute("RegistrarCuenta",
                    new { model.Identificacion, model.Nombre, model.Correo, model.Contrasenna });

                var respuesta = new RespuestaModel();

                if(result > 0)                
                    respuesta.Indicador = true;                
                else
                {
                    respuesta.Indicador= false;
                    respuesta.Mensaje = "Su información no se ha registrado corectamente";
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
                    respuesta.Indicador = true;
                    respuesta.Datos = result;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Su información no se ha validado corectamente";
                }

                return Ok(respuesta);

            }

        }
    }
}

using Dapper;
using JN_ProyectoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace JN_ProyectoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PuestosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PuestosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        [HttpGet]
        [Route("ConsultarPuestos")]
        public IActionResult ConsultarPuestos(long Id)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<PuestosModel>("ConsultarPuestos", //Este context.Query<PuestosModel> devuelve una lista
                    new { Id });

                var respuesta = new RespuestaModel();

                if (result != null)
                {
                    respuesta.Indicador = true;
                    respuesta.Datos = result;
                }
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No hay información registrada";
                }

                return Ok(respuesta);
            }
        }

        [HttpPost]
        [Route("RegistrarPuesto")]
        public IActionResult RegistrarPuesto(PuestosModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("RegistrarPuesto", //Procedimiento Almacenado
                    new { model.Nombre, model.Descripcion });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "El puesto no se ha registrado correctamente";
                }

                return Ok(respuesta);
            }
        }

        [HttpPut]
        [Route("ActualizarPuesto")]
        public IActionResult ActualizarPuesto(PuestosModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("ActualizarPuesto", //Procedimiento Almacenado
                    new { model.Id, model.Nombre, model.Descripcion });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "El puesto no se ha actualizado correctamente";
                }

                return Ok(respuesta);
            }
        }
    }
}
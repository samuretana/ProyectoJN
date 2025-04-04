using Dapper;
using JN_ProyectoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JN_ProyectoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OfertasController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public OfertasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        [HttpGet]
        [Route("ConsultarOfertas")]
        public IActionResult ConsultarOfertas(long Id)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<OfertasModel>("ConsultarOfertas",
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
        [Route("RegistrarOferta")]
        public IActionResult RegistrarOferta(OfertasModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("RegistrarOferta", //Procedimiento Almacenado
                    new { model.IdPuesto, model.Salario, model.Horario, model.Cantidad });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "La oferta no se ha registrado correctamente";
                }

                return Ok(respuesta);
            }
        }

        [HttpPut]
        [Route("ActualizarOferta")]
        public IActionResult ActualizarOferta(OfertasModel model)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("ActualizarOferta", //Procedimiento Almacenado
                    new { model.Id, model.IdPuesto, model.Salario, model.Horario, model.Cantidad, model.Estado });

                var respuesta = new RespuestaModel();

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "La oferta no se ha actualizado correctamente";
                }

                return Ok(respuesta);
            }
        }

        [HttpGet]
        [Route("ConsultarUsuariosOfertas")]
        public IActionResult ConsultarUsuariosOfertas()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<OfertasModel>("ConsultarUsuariosOfertas",
                    new {  });

                var respuesta = new RespuestaModel();

                if (result.Any())
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

    }
}

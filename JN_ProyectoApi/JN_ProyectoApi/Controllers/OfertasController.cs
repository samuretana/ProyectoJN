using Dapper;
using JN_ProyectoApi.Models;
using JN_ProyectoApi.Servicios;
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
        private readonly IGeneral _general;
        public OfertasController(IConfiguration configuration, IGeneral general)
        {
            _configuration = configuration;
            _general = general;
        }

        
        [HttpGet]
        [Route("ConsultarOfertas")]
        public IActionResult ConsultarOfertas(long Id)
        {
            var respuesta = new RespuestaModel();

            if (!_general.ValidarUsuarioReclutadorFromToken(User.Claims))
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = "No tiene permisos para acceder a esta información";
                return Ok(respuesta);
            }
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<OfertasModel>("ConsultarOfertas",
                    new { Id });

                

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
            var respuesta = new RespuestaModel();

            if (!_general.ValidarUsuarioReclutadorFromToken(User.Claims))
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = "No tiene permisos para acceder a esta información";
                return Ok(respuesta);
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("RegistrarOferta", //Procedimiento Almacenado
                    new { model.IdPuesto, model.Salario, model.Horario, model.Cantidad });

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
            var respuesta = new RespuestaModel();

            if (!_general.ValidarUsuarioReclutadorFromToken(User.Claims))
            {
                respuesta.Indicador = false;
                respuesta.Mensaje = "No tiene permisos para acceder a esta información";
                return Ok(respuesta);
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("ActualizarOferta", //Procedimiento Almacenado
                    new { model.Id, model.IdPuesto, model.Salario, model.Horario, model.Cantidad, model.Estado });

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

        [HttpGet]
        [Route("ConsultarOfertasDisponibles")]
        public IActionResult ConsultarOfertasDisponibles()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<OfertasModel>("ConsultarOfertasDisponibles",
                    new { });

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

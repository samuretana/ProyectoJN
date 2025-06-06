﻿using Dapper;
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
            var IdUsuario = _general.ObtenerUsuarioFromToken(User.Claims);

            if (_general.ValidarUsuarioReclutadorFromToken(User.Claims))
                IdUsuario = -1;

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<OfertasModel>("ConsultarUsuariosOfertas",
                    new { IdUsuario });

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

        [HttpGet]
        [Route("ConsultarEstados")]
        public IActionResult ConsultarEstados()
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value))
            {
                var result = context.Query<EstadosModel>("ConsultarEstados",
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

        [HttpPost]
        [Route("AplicarOferta")]
        public IActionResult AplicarOferta (OfertasModel model)
        {
            var respuesta = new RespuestaModel();            

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var IdUsuario = _general.ObtenerUsuarioFromToken(User.Claims);


                var result = context.Execute("AplicarOferta", //Procedimiento Almacenado
                    new { IdUsuario, model.IdOferta });

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "Ya se encuentra participando en la oferta #" + model.IdOferta;
                }

                return Ok(respuesta);
            }
        }

        [HttpPut]
        [Route("ActualizarProcesoOferta")]
        public IActionResult ActualizarProcesoOferta(OfertasModel model)
        {
            var respuesta = new RespuestaModel();

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:BDConnection").Value)) //Conexion a la BD
            {
                var result = context.Execute("ActualizarProcesoOferta", //Procedimiento Almacenado
                    new { model.Id, model.EstadoOferta });

                if (result > 0)
                    respuesta.Indicador = true;
                else
                {
                    respuesta.Indicador = false;
                    respuesta.Mensaje = "No fue posible actualizaer el estado de la Aplicación #" + model.IdOferta;
                }

                return Ok(respuesta);
            }
        }

    }
}

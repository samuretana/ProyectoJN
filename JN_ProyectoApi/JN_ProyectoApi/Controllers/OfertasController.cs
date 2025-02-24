﻿using Dapper;
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
    }
}

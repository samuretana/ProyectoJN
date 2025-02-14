using JN_ProyectoApi.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {

        public IActionResult CapturarError()
        {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var respuesta = new RespuestaModel();

            
                respuesta.Indicador = false;
                respuesta.Mensaje = "Se presento un problema en el sistema";            

            return Ok(respuesta);
        }
    }
}

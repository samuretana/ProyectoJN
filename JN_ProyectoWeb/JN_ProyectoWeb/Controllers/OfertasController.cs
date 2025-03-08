using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoWeb.Controllers
{
    public class OfertasController : Controller
    {
        public IActionResult ConsultarOfertas()
        {
            return View();
        }
    }
}

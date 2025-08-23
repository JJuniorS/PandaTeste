using Microsoft.AspNetCore.Mvc;

namespace pandaTeste.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ViagensController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

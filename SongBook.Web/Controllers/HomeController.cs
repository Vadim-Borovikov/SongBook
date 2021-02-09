using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SongBook.Web.Models;

namespace SongBook.Web.Controllers
{
    [Route("")]
    public sealed class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index([FromServices]Manager manager) => View(new SongViewModel(manager.Songs[0]));

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

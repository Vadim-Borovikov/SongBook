using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SongBook.Web.Models;

namespace SongBook.Web.Controllers
{
    [Route("")]
    public sealed class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromServices]Manager manager)
        {
            manager.LoadIndex();
            return View(manager.Songs);
        }

        [HttpGet]
        [Route("song")]
        public IActionResult SongView(int id, sbyte? semitones, [FromServices]Manager manager)
        {
            Song song = manager.Songs[id];

            if (!semitones.HasValue)
            {
                manager.LoadSong(song);
                semitones = (sbyte) song.GetDefaultTune();
            }

            song.TransposeTo(semitones.Value);
            var songViewModel = new SongViewModel(song, id);
            return View(songViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

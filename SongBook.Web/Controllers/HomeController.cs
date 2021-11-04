using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SongBook.Web.Models;

namespace SongBook.Web.Controllers
{
    [Route("")]
    public sealed class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromServices]Manager manager, bool? roll)
        {
            manager.LoadIndex();
            if (roll.HasValue && roll.Value)
            {
                manager.Roll();
            }
            var songsViewModel = new SongsViewModel(manager.Songs, manager.SaveData);
            return View(songsViewModel);
        }

        [HttpGet]
        [Route("roll")]
        public IActionResult Roll([FromServices]Manager manager)
        {
            manager.LoadIndex();
            manager.Roll();
            return Redirect("/");
        }

        [HttpGet]
        [Route("song")]
        public IActionResult SongView(byte id, sbyte? delta, bool? showRepeats, bool? autotune, [FromServices]Manager manager)
        {
            Song song = manager.Songs[id];
            if (autotune == true)
            {
                Tune easiest = song.GetEasiestTune();
                song.TransposeTo(easiest);
            }
            else if (delta.HasValue)
            {
                song.TransposeBy(delta.Value);
            }
            else
            {
                manager.LoadSong(song);
                song.TransposeTo(song.DefaultTune);
            }
            var songViewModel = new SongViewModel(song, id, showRepeats);
            return View(songViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

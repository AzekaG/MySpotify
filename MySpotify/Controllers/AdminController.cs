using AutoMapper;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MySpotify.Models;
using MySpotify.Repository;

namespace MySpotify.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepositoryMedia _contextMedia;
        private readonly IRepositoryUser _contextUser;
        private readonly IWebHostEnvironment _environment;



        public AdminController(IRepositoryMedia context, IRepositoryUser repositoryUser, IWebHostEnvironment _Environment)
        {
            _contextMedia = context;
            _contextUser = repositoryUser;
            _environment = _Environment;
        }



        public ActionResult Index()
        {

            return View();
        }


        //start working with genre
        [HttpGet]
        public ActionResult GetGenreList()
        {
            var GenrList = _contextMedia.GetGenresList().Result;
            return Json(GenrList);
        }
        // Genres/Edit/
        #region genreworking
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = _contextMedia.GetGenresList().Result.FirstOrDefault(i => i.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            return PartialView("EditGenre", genre);
        }
        // post: Genres/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Genre genre)
        {
            if (id != genre.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _contextMedia.UpdateGenre(genre);
                    await _contextMedia.SaveDb();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_contextMedia.GenreExist(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = _contextMedia.GetGenresList().Result.FirstOrDefault(i => i.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            return PartialView("DeleteGenre", genre);
        }
        //removeJenre
        [HttpPost]
        public async Task<IActionResult> Delete(int id, Genre genre)
        {

            
            if (id != genre.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _contextMedia.RemoveGenre(genre);
                    await _contextMedia.SaveDb();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_contextMedia.GenreExist(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return RedirectToAction("Index");
        }

        //createGenre
        public IActionResult Create()
        {
            return PartialView("CreateGenre");
        }

        //createGenre
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _contextMedia.CreateGenre(genre);
                await _contextMedia.SaveDb();
                return Ok();
            }
            return PartialView("CreateGenre");
        }

        #endregion genreworking

        // start work with media>

        //GetMedia

        [HttpGet]
        public async Task<ActionResult> GetMediaList()
        {

            var mediaList = await _contextMedia.GetMediaList();
            var config = new MapperConfiguration(cnf => cnf.CreateMap<Media, MediaForMapping>()
                .ForMember("Genre", opt => opt.MapFrom(c => c.Name)));
            var mapper = new Mapper(config);
            var res = mapper.Map<IEnumerable<Media>, IEnumerable<MediaForMapping>>(await _contextMedia.GetMediaList());

            return Json(res);
        }

        //EditMedia
        [HttpGet]
        public async Task<IActionResult> EditMedia(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaCollection = await _contextMedia.GetMediaList();
            var media = mediaCollection.FirstOrDefault(i => i.Id == id);
            var genres = _contextMedia.GetGenresList().Result;
            Genre genr;
            if (media.Genre != null)
            {
                 genr = genres.Where(i => i.Id == media.Genre.Id).First();
            }
            else
            {
                 genr = new Genre() { Name = "null", Id = 33 };
                 media.Genre = genr;
            }
            ViewBag.Genres = new SelectList(genres, "Id", "Name" , genr.Id);
   
            Media tempMedia = new Media() { Artist = media.Artist , FileAdress = media.FileAdress , Genre = media.Genre,
            User = media.User , Name = media.Name , Poster = media.Poster , TypeMedia = media.TypeMedia , Id = media.Id};
            
            if (media == null)
            {
                return NotFound();
            }
           
            return PartialView("EditMedia", tempMedia);
        }
        // post: Genres/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditMedia(int Id, Media media , int Genre , int User)
        {
            if (Genre != 0)
            {
                Genre gen = _contextMedia.GetGenresList().Result.Where(x => x.Id == Genre).FirstOrDefault();
                media.Genre = gen;
            }
            
            if (Id != media.Id)
            {
                return NotFound();
            }
            if (media.TypeMedia != null && media.Artist != null && media.FileAdress != null && media.Name != null && media.Genre != null && media.Poster != null)
            {
                try
                {
                    await _contextMedia.UpdateMedia(media);
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_contextMedia.GenreExist(Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return BadRequest();
           
        }


        //remove media
        public async Task<IActionResult> DeleteMedia(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaCollection = await _contextMedia.GetMediaList();
            var media = mediaCollection.FirstOrDefault(x => x.Id == id);
            if (media == null)
            {
                return NotFound();
            }
            return PartialView("DeleteMedia", media);
        }
        //removeMedia
        [HttpPost]
        public async Task<IActionResult> DeleteMedia(int id, Media media)
        {
           
                try
                {
                    await _contextMedia.DeleteMedia(id);
                    await _contextMedia.SaveDb();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_contextMedia.GetMediaList().Result.Any(x => x.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            
           
        }



        //createGenre  не работает загрузка файла , проблема с аяксом! не передает upPoster и upMediaFile в контроллер
      
    }
}

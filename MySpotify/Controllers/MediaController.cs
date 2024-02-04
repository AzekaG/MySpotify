using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySpotify.Models;
using MySpotify.Repository;

namespace MySpotify.Controllers
{/*  HttpContext.Session.SetString("Id", user.id.ToString());*/
    public class MediaController : Controller
    {
        private readonly IRepositoryMedia _contextMedia;
        private readonly IRepositoryUser _contextUser;
        private readonly IWebHostEnvironment _environment;


        public MediaController(IRepositoryMedia context, IRepositoryUser repositoryUser, IWebHostEnvironment _Environment)
        {
            _contextMedia =  context;
            _contextUser = repositoryUser;
            _environment = _Environment;
        }


        bool isLogged()
        {
            if (HttpContext.Session.GetString("Id") != null)
            {
                return true;
            }
            return false;
        }
       


        public IActionResult AddMedia()
        {
            if (!isLogged())
            {
                return RedirectToAction("Login", "Users");
            }

            var genres = _contextMedia.GetGenresList().Result.Select(x=>x.Name);

            ViewBag.Genres = new SelectList(genres);

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1000000000)]
        public async Task<ActionResult> AddMedia(MediaAdd media, IFormFile? upPoster , IFormFile? upMediaFile , string genre)
        {
            if (!isLogged())
            {
                return RedirectToAction("Login", "Users");
            }


            string MediaAdress = String.Empty;
            string PosterAdress = String.Empty; ;
            if (upPoster != null && upMediaFile != null) 
            {
                MediaAdress = "/Media/Music/" + upMediaFile.FileName;
                PosterAdress = "/Media/Poster/" + upPoster.FileName;
            }
            media.Poster = PosterAdress;
            media.FileAdress = MediaAdress;
            media.genre = _contextMedia.GetGenresList().Result.Where(x=>x.Name == genre).FirstOrDefault();
         
           if(ModelState.IsValid)
            {
                var user = _contextUser.GetUserList().Result.Where(x => x.id == int.Parse(HttpContext.Session.GetString("Id"))).FirstOrDefault();

                await UpLoadMedia(upMediaFile, MediaAdress);
                await UpLoadMedia(upPoster, PosterAdress);
                Media newMedia = new Media();
                newMedia.Name = media.Name;
                newMedia.Artist = media.Artist;
                newMedia.FileAdress = media.FileAdress;
                newMedia.Poster = media.Poster;
                newMedia.TypeMedia = media.typeMedia;
                newMedia.User = user;
                newMedia.Genre = media.genre;

                await _contextMedia.CreateMedia(newMedia);
                await _contextMedia.SaveDb();
                await _contextUser.SaveDb();
               
                return Redirect("/Media/Index");
               
            }
            //return View("/Users/Index");
            return View(media);
        }
        

        async Task UpLoadMedia(IFormFile? formFile , string MediaPath)
        {
            if (!isLogged())
            {
                 RedirectToAction("Login", "Users");
            }

            using (var fileStream = new FileStream(_environment.WebRootPath + MediaPath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);   //ошибка
            };
        }
      
        // GET: Media
        
        public ActionResult Index()
        {

            if (!isLogged())
            {
               return RedirectToAction("Login", "Users");
            }
            int idUser = int.Parse(HttpContext.Session.GetString("Id"));

         


            ViewBag.MusicRecomended = _contextMedia.GetMediaList().Result;
            /*Здесь можно вставить и проработать алгоритм добавления песен , основываясь на последних прослушиваниях
             параметрах поиска , добавленных жанров и артистов , но для упрощения мы просто будем выводить загруженные песни сервера*/
			


            var user = _contextUser.GetUserList().Result.Where(x=>x.id==idUser).FirstOrDefault();
            return View(user);
        }








        [HttpGet]
        public async Task<ActionResult> GetMediaList()
        {
            int userId = int.Parse(HttpContext.Session.GetString("Id"));
            var media = await _contextMedia.GetMediaList();
            var mediaList = media.Where(x => x.User.id == userId);
            var config = new MapperConfiguration(cnf => cnf.CreateMap<Media, MediaForMapping>()
                .ForMember("Genre", opt => opt.MapFrom(c => c.Name)));
            var mapper = new Mapper(config);
            var res = mapper.Map<IEnumerable<Media>, IEnumerable<MediaForMapping>>(mediaList);

            return Json(res);
        }

        public IActionResult MediaSettings()
        {
            int idUser = int.Parse(HttpContext.Session.GetString("Id"));
            var user = _contextUser.GetUserList().Result.Where(x => x.id == idUser).FirstOrDefault();
           
            return View("MediaSettings" , user);
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
            ViewBag.Genres = new SelectList(genres, "Id", "Name", genr.Id);



            if (media == null)
            {
                return NotFound();
            }

            return PartialView("EditMedia", media);
        }
        // post: Genres/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditMedia(int Id, Media media, int Genre, int User)
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
            if (media.TypeMedia != null && media.Artist != null && media.FileAdress != null && media.Name != null && media.Poster != null)
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





    }
}

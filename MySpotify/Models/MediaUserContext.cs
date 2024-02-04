using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;

namespace MySpotify.Models
{
    public class MediaUserContext : DbContext
    {   

        public DbSet<User> Users {  get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public MediaUserContext(DbContextOptions<MediaUserContext> options) : base(options)
        {
            if (Database.EnsureCreated())
            {
                /*pass == 1111*/
                

                Users?.Add(new User { FirstName = "Sergio", LastName = "Adminos", Email = "azekaggg@gmail.com", Status = Status.admin, Active = true , Password = "938E3782FCDEC612B9A5BB5FD9EB94E9", Salt = "4BFEA494ACEE92BB6ADBD53348F5CD52" });
                SaveChanges();

                Genres?.AddRange(new Genre { Name = "Pop" }, new Genre { Name = "Rock" }, new Genre { Name = "Rap" },
                    new Genre { Name = "Metal" }, new Genre { Name = "House" }, new Genre { Name = "Tehno" },
                    new Genre { Name = "Indi" }, new Genre { Name = "Lo-fi" }, new Genre { Name = "Reggae" });
                SaveChanges();

                Medias?.Add(new Media { Name = "Слышишь", Artist = "Dante", Genre = Genres?.First(x => x.Name == "Pop"), TypeMedia = TypeMedia.Audio, Poster = "/Media/Poster/Hear.jpg", FileAdress = "/Media/Music/Vladimir Dantes-Чуєш.mp3", User = Users.FirstOrDefault(x=>x.Status == Status.admin)});
                Medias?.Add(new Media { Name = "Ohne dich", Artist = "Rammstein", Genre = Genres?.First(x => x.Name == "Rock"), TypeMedia = TypeMedia.Audio, Poster = "/Media/Poster/Rammstein.jpg", FileAdress = "Media/Music/ramshtajn_rammstein_-_ohne_dich_(z3.fm).mp3" , User = Users.FirstOrDefault(x => x.Status == Status.admin) });
                Medias?.Add(new Media { Name = "Слышишь", Artist = "ДзиДзьо", Genre = Genres?.First(x => x.Name == "Pop"), TypeMedia = TypeMedia.Audio, Poster = "/Media/Poster/DziDzio.jpg", FileAdress = "/Media/Music/DziDzio-pavuk.mp3", User = Users.FirstOrDefault(x => x.Status == Status.admin) });

                SaveChanges();
            };
            /* 
            */
        }

    }
}

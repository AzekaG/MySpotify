using Microsoft.EntityFrameworkCore;
using MySpotify.Models;
using NuGet.Packaging.Signing;

namespace MySpotify.Repository
{
    public class MediaRepository : IRepositoryMedia
    {
        readonly MediaUserContext _mediauserContext;
        public MediaRepository(MediaUserContext mediaUserContext) => _mediauserContext = mediaUserContext;

        public async Task<List<Media>> GetMediaList() => await _mediauserContext.Medias.AsNoTracking().Include(x=>x.Genre).Include(p=>p.User).ToListAsync();

        public async Task CreateMedia(Media media) => await _mediauserContext.Medias.AddAsync(media);

        public async Task SaveDb() => await _mediauserContext.SaveChangesAsync();
    
        public async Task<List<Genre>> GetGenresList() => await _mediauserContext.Genres.ToListAsync();
        public async Task DeleteMedia(int id)
        {
            var media =await _mediauserContext.Medias.FindAsync(id);
            if(media != null)
                _mediauserContext.Medias.Remove(media);
        }
        public  void UpdateMedia(Media media)
        {
            try
            {
                var MediaSave = _mediauserContext.Medias.FirstOrDefault(x => x.Id == media.Id);
                MediaSave.Poster = media.Poster;
                MediaSave.FileAdress = media.FileAdress;
                MediaSave.Name = media.Name;
                MediaSave.Genre = media.Genre;
                MediaSave.Artist = media.Artist;
                _mediauserContext.SaveChangesAsync();
            }
            catch(Exception ex) 
            {
                     Console.WriteLine(ex.Message + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }




        }
       
        public void UpdateGenre(Genre genre) => _mediauserContext.Entry(genre).State = EntityState.Modified;
        public async Task CreateGenre(Genre genre) => await _mediauserContext.Genres.AddAsync(genre);
        public void RemoveGenre(Genre genre)
        {
            var media = _mediauserContext.Medias.Where(x => x.Genre == genre);  //из-за того что связь каскадная - удаляем ее вручную , потому тчо мы так и не дошли до материала , где показывалось как убрать связь по дефолту( 
            foreach(var item in media)
            {
                item.Genre = null;
            }
            _mediauserContext.SaveChanges();
            _mediauserContext.Genres.Remove(genre);
        }
        public bool GenreExist(int id) => (_mediauserContext.Genres?.Any(e=>e.Id == id)).GetValueOrDefault();
    
    }
}

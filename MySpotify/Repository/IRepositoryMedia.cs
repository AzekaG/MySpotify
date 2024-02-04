using Microsoft.EntityFrameworkCore;
using MySpotify.Models;

namespace MySpotify.Repository
{
    public interface IRepositoryMedia
    {
        Task<List<Media>> GetMediaList();
        Task CreateMedia(Media media);
        Task SaveDb();
        Task DeleteMedia(int id);
        Task UpdateMedia(Media MediaFile);

        void UpdateGenre(Genre genre);
        Task<List<Genre>> GetGenresList();

        bool GenreExist(int id);
        void RemoveGenre(Genre genre);

        Task CreateGenre(Genre genre);
        
    }
}
